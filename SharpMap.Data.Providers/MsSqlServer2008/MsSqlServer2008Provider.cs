﻿/*
 *  The attached / following is part of SharpMap.Data.Providers.MsSqlServer2008
 *  SharpMap.Data.Providers.MsSqlServer2008 is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */

using System;
using System.Collections.Generic;
using System.Data;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.MsSqlServer2008;
using SharpMap.Data.Providers.MsSqlServer2008.Expressions;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public enum SqlServer2008ExtentsMode
    {
        /// <summary>
        /// Requires no additional components but can be very slow for large datasets
        /// </summary>
        QueryIndividualFeatures = 0,
        /// <summary>
        /// Requires SqlSpatialTools be installed on the db server
        /// </summary>
        UseSqlSpatialTools = 1,

        /// <summary>
        /// Requires no additional components but does require additional columns in the form of [GeomColumnName]_Envelope_MinX, [GeomColumnName]_Envelope_MinY, [GeomColumnName]_Envelope_MaxX, [GeomColumnName]_Envelope_MaxY
        /// Initial tests seem to show this is the fastest.
        /// </summary>
        UseEnvelopeColumns = 2
    }

    public class MsSqlServer2008Provider<TOid>
        : SpatialDbProviderBase<TOid>
    {
        public MsSqlServer2008Provider(IGeometryFactory geometryFactory,
                                       String connectionString,
                                       String tableName)
            : this(geometryFactory, connectionString, null, tableName, null, null)
        {
        }

        public MsSqlServer2008Provider(IGeometryFactory geometryFactory,
                                       String connectionString,
                                       String tableSchema,
                                       String tableName,
                                       String oidColumn,
                                       String geometryColumn)
            : base(new SqlServerDbUtility(),
                   geometryFactory,
                   connectionString,
                   tableSchema,
                   tableName,
                   oidColumn,
                   geometryColumn)
        {
        }

        public override String GeometryColumnConversionFormatString
        {
            get { return "{0}.STAsBinary()"; }
        }

        public override string GeomFromWkbFormatString
        {
            get { throw new NotImplementedException(); }
        }

        public override IExtents GetExtents()
        {
            SqlServer2008ExtentsMode server2008ExtentsCalculationMode =
                GetProviderPropertyValue<MsSqlServer2008ExtentsModeExpression, SqlServer2008ExtentsMode>(
                    DefaultProviderProperties.ProviderProperties.Collection,
                    SqlServer2008ExtentsMode.QueryIndividualFeatures);

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                cmd.Connection = conn;
                switch (server2008ExtentsCalculationMode)
                {
                    case SqlServer2008ExtentsMode.UseSqlSpatialTools:
                        {
                            cmd.CommandText =
                                string.Format(
                                    @"
    declare @envelope Geometry
    select @envelope = dbo.GeometryEnvelopeAggregate({0}) from {1}.{2} {3}
    select 
        @envelope.STPointN(2).STX as MinX, 
        @envelope.STPointN(2).STY as MinY, 
        @envelope.STPointN(4).STX as MaxX, 
        @envelope.STPointN(4).STY as MaxY",
                                    GeometryColumn, TableSchema, Table,
                                    GetWithClause(DefaultProviderProperties.ProviderProperties.Collection));
                            break;
                        }
                    case SqlServer2008ExtentsMode.UseEnvelopeColumns:
                        {
                            cmd.CommandText = string.Format(
                                "SELECT MIN({0}_Envelope_MinX), MIN({0}_Envelope_MinY), MAX({0}_Envelope_MaxX), MAX({0}_Envelope_MaxY) FROM {1}.{2} {3}",
                                GeometryColumn, TableSchema, Table,
                                GetWithClause(DefaultProviderProperties.ProviderProperties.Collection));
                            break;
                        }
                    default:
                        {
                            cmd.CommandText =
                                string.Format(
                                    @"
    select 
	    Min(Geom.STEnvelope().STPointN(1).STX)as MinX, 
	    Min(Geom.STEnvelope().STPointN(1).STY) as MinY,  
	    Max(Geom.STEnvelope().STPointN(3).STX) as MaxX, 
	    Max(Geom.STEnvelope().STPointN(3).STY) as MaxY from {0}.{1} {2}",
                                    TableSchema, Table,
                                    GetWithClause(DefaultProviderProperties.ProviderProperties.Collection));
                            break;
                        }
                }

                cmd.CommandType = CommandType.Text;
                double xmin, ymin, xmax, ymax;
                conn.Open();
                using (IDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (r.Read())
                    {
                        xmin = r.GetDouble(0);
                        ymin = r.GetDouble(1);
                        xmax = r.GetDouble(2);
                        ymax = r.GetDouble(3);
                        return GeometryFactory.CreateExtents2D(xmin, ymin, xmax, ymax);
                    }
                }
            }

            return GeometryFactory.CreateExtents();
        }

        protected override ExpressionTreeToSqlCompilerBase CreateSqlCompiler(Expression expression)
        {
            return new MsSqlServer2008ExpressionTreeToSqlCompiler(DbUtility, SelectAllColumnNames,
                                                                  GeometryColumnConversionFormatString, expression,
                                                                  TableSchema, Table, OidColumn,
                                                                  GeometryColumn, Srid);
        }


        /* TODO: Add paging, order by etc from ProviderPropertyExpression */

        protected override string GenerateSql(IList<ProviderPropertyExpression> properties,
                                              ExpressionTreeToSqlCompilerBase compiler)
        {
            return string.Format(" {0} SELECT {1}  FROM {2}{6} {3} {4} {5}",
                                 compiler.SqlParamDeclarations,
                                 string.IsNullOrEmpty(compiler.SqlColumns)
                                     ? string.Join(",", Enumerable.ToArray(SelectAllColumnNames()))
                                     : compiler.SqlColumns,
                                 compiler.QualifiedTableName,
                                 compiler.SqlJoinClauses,
                                 string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                 compiler.SqlWhereClause,
                                 GetWithClause(properties));
        }

        protected string GetWithClause(IEnumerable<ProviderPropertyExpression> properties)
        {
            bool withNoLock = GetProviderPropertyValue<WithNoLockExpression, bool>(properties, false);

            IEnumerable<string> indexNames = GetProviderPropertyValue<IndexNamesExpression, IEnumerable<string>>(
                properties, new string[] {});


            bool forceIndex = Enumerable.Count(indexNames) > 0 &&
                              GetProviderPropertyValue<ForceIndexExpression, bool>(properties, false);
            ;

            if (!withNoLock && !forceIndex)
                return "";

            if (withNoLock && !forceIndex)
                return " WITH(NOLOCK) ";

            if (forceIndex && !withNoLock)
                return string.Format(" WITH(INDEX({0})) ", string.Join(",", Enumerable.ToArray(indexNames)));

            return string.Format(" WITH(NOLOCK,INDEX({0})) ", string.Join(",", Enumerable.ToArray(indexNames)));
        }


        public override DataTable GetSchemaTable()
        {
            DataTable dt = base.GetSchemaTable(true);
            dt.Columns[GeometryColumn].DataType = typeof (byte[]);
            //the natural return type is the native sql Geometry we need to override this to avoid a schema merge exception
            return dt;
        }
    }
}