// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Data;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using System.Globalization;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.FeatureProvider
{
    /// <summary>
    /// In-memory provider for arbitrary feature data.
    /// </summary>
	public class FeatureProvider : IWritableFeatureProvider<Guid>
	{
        internal readonly static String OidColumnName = "Oid";
        private FeatureDataTable<Guid> _features;
		private ICoordinateTransformation _transform;
        private ICoordinateSystem _spatialReference;
        private IGeometryFactory _geoFactory;

        /// <summary>
        /// Creates a new FeatureProvider with the given columns as a schema.
        /// </summary>
        /// <param name="columns">
        /// The schema to create the FeatureProvider with.
        /// </param>
		public FeatureProvider(IGeometryFactory factory, params DataColumn[] columns)
		{
            _geoFactory = factory;
            _features = new FeatureDataTable<Guid>(OidColumnName, GeometryFactory);

            foreach (DataColumn column in columns)
            {
                String keyColumnName = _features.PrimaryKey[0].ColumnName;
                if (String.Compare(keyColumnName, column.ColumnName) != 0)
                {
                    _features.Columns.Add(column);
                }
            }
		}

		#region IWritableFeatureProvider<Guid> Members

		public void Insert(FeatureDataRow<Guid> feature)
		{
			_features.ImportRow(feature);
		}

		public void Insert(IEnumerable<FeatureDataRow<Guid>> features)
		{
			foreach (FeatureDataRow<Guid> feature in features)
			{
				Insert(feature);
			}
		}

		public void Update(FeatureDataRow<Guid> feature)
		{
			throw new NotImplementedException();
		}

		public void Update(IEnumerable<FeatureDataRow<Guid>> features)
		{
			throw new NotImplementedException();
		}

		public void Delete(FeatureDataRow<Guid> feature)
		{
			throw new NotImplementedException();
		}

		public void Delete(IEnumerable<FeatureDataRow<Guid>> features)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IFeatureProvider<Guid> Members

        public IEnumerable<Guid> ExecuteOidQuery(SpatialBinaryExpression query)
		{
			throw new NotImplementedException();
		}

		public IGeometry GetGeometryByOid(Guid oid)
		{
			throw new NotImplementedException();
		}

		public IFeatureDataRecord GetFeatureByOid(Guid oid)
		{
			throw new NotImplementedException();
        }

		public void SetTableSchema(FeatureDataTable<Guid> table)
		{
			throw new NotImplementedException();
        }

        public void SetTableSchema(FeatureDataTable<Guid> table, SchemaMergeAction schemaMergeAction)
        {
            _features.MergeSchema(table, schemaMergeAction);
        }

		#endregion

        #region IFeatureProvider Members

        public FeatureDataTable CreateNewTable()
        {
            FeatureDataTable table = new FeatureDataTable(GeometryFactory);
            SetTableSchema(table);
            return table;
        }

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query)
        {
            if (query == null) throw new ArgumentNullException("query");
            
            FeatureDataReader reader = new FeatureDataReader(_geoFactory, 
                                                             _features,
                                                             query,
                                                             FeatureQueryExecutionOptions.FullFeature);
            return reader;
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression bounds, 
                                                      FeatureQueryExecutionOptions options)
        {
            FeatureDataReader reader = new FeatureDataReader(_geoFactory, _features, bounds, options);
            return reader;
        }

        public IGeometryFactory GeometryFactory
        {
            get { return _geoFactory; }
            set { _geoFactory = value; }
        }

        /// <summary>
        /// Returns the number of features in the entire dataset.
        /// </summary>
        /// <returns>Count of the features in the entire dataset.</returns>
	    public Int32 GetFeatureCount()
		{
			return _features.FeatureCount;
        }

        /// <summary>
        /// Returns a <see cref="DataTable"/> with rows describing the columns in the schema
        /// for the configured provider. Provides the same result as 
        /// <see cref="IDataReader.GetSchemaTable"/>.
        /// </summary>
        /// <seealso cref="IDataReader.GetSchemaTable"/>
        /// <returns>A DataTable that describes the column metadata.</returns>
	    public DataTable GetSchemaTable()
	    {
	        throw new NotImplementedException();
	    }

        /// <summary>
        /// Gets the locale of the data as a CultureInfo.
        /// </summary>
	    public CultureInfo Locale
	    {
	        get { throw new NotImplementedException(); }
	    }

        /// <summary>
        /// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
        /// present in the IProvider with the given connection.
        /// </summary>
        /// <param name="table">The FeatureDataTable to configure the schema of.</param>
	    public void SetTableSchema(FeatureDataTable table)
		{
			_features.MergeSchema(table);
        }

		#endregion

		#region IProvider Members

		public void Close()
		{
			// Do nothing...
		}

		public String ConnectionId
		{
			get { return String.Empty; }
		}

		public ICoordinateTransformation CoordinateTransformation
		{
			get { return _transform; }
			set { _transform = value; }
        }

        public Object ExecuteQuery(Expression query)
        {
            throw new NotImplementedException();
            //FeatureDataReader reader = new FeatureDataReader(_geoFactory, _features, bounds, options);
            //return reader;
        }

		public IExtents GetExtents()
		{
			return _features.Extents;
		}

		public Boolean IsOpen
		{
			get { return true; }
		}

		public void Open()
		{
			// Do nothing...
		}

		public ICoordinateSystem SpatialReference
		{
			get { return _spatialReference;  }
		}

		public Int32? Srid
		{
			get { return null; }
			set {  }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if(_features != null)
			{
				_features.Dispose();
				_features = null;
			}
		}

		#endregion
    }
}