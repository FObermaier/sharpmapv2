// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using GeoAPI.Geometries;
using System.Globalization;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    /// <summary>
    /// Interface for feature data providers.
    /// </summary>
    public interface IFeatureLayerProvider : ILayerProvider
    {
        /// <summary>
        /// Begins to retrieve the features which match the specified 
        /// <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Feature spatial query to execute.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query, 
            FeatureDataSet dataSet, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the features which match the specified 
        /// <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Feature spatial query to execute.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query, 
            FeatureDataTable table, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, 
            FeatureDataSet dataSet, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet,
            QueryExecutionOptions options, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, 
            AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table,
            QueryExecutionOptions options, AsyncCallback callback, Object asyncState);

        IAsyncResult BeginGetFeatures(IEnumerable oids, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Creates a new <see cref="FeatureDataTable"/> from the data source's 
        /// schema.
        /// </summary>
        /// <returns>
        /// A <see cref="FeatureDataTable"/> which is configured for the 
        /// data source's schema.
        /// </returns>
        FeatureDataTable CreateNewTable();

        /// <summary>
        /// Ends a retrieval of the features, waiting on the 
        /// <see cref="IAsyncResult.AsyncWaitHandle"/> if the operation is not complete.
        /// </summary>
        void EndExecuteFeatureQuery(IAsyncResult asyncResult);

        IEnumerable<IFeatureDataRecord> EndGetFeatures(IAsyncResult asyncResult);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteFeatureQuery(FeatureSpatialExpression query);

        /// <summary>
        /// Retrieves features into a <see cref="FeatureDataSet"/> that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataSet dataSet);

        /// <summary>
        /// Retrieves features into a <see cref="FeatureDataTable"/> that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataTable table);

        /// <summary>
        /// Gets the geometries within the specified 
        /// <see cref="GeoAPI.Geometries.IExtents"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <returns>
        /// An enumeration of features within the specified 
        /// <see cref="GeoAPI.Geometries.IExtents"/>.
        /// </returns>
        IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IExtents bounds);

        /// <summary>
        /// Gets the geometries within the specified 
        /// <see cref="GeoAPI.Geometries.IGeometry"/>.
        /// </summary>
        /// <param name="geometry">
        /// <see cref="IGeometry"/> to intersect with.
        /// </param>
        /// <returns>
        /// An enumeration of features within the specified 
        /// <see cref="GeoAPI.Geometries.IExtents"/>.
        /// </returns>
        IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IGeometry geometry);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <returns>An <see cref="IFeatureDataReader"/> to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IGeometry geometry);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <returns>An <see cref="IFeatureDataReader"/> to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IGeometry geometry, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataSet dataSet);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataSet dataSet, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataTable table);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataTable table, QueryExecutionOptions options);

        IGeometryFactory GeometryFactory { get; set; }

        /// <summary>
        /// Returns a <see cref="IFeatureDataReader"/> for obtaining features
        /// from a set of feature Object identifiers (oids).
        /// </summary>
        /// <param name="oids">A set of Object ids (OIDs) of the features.</param>
        /// <returns>
        /// A set of features corresponding one-to-one to the given <paramref name="oids"/>.
        /// </returns>
        IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable oids);

        /// <summary>
        /// Returns the number of features in the entire data source.
        /// </summary>
        /// <returns>Count of the features in the entire data source.</returns>
        Int32 GetFeatureCount();

        /// <summary>
        /// Returns a <see cref="DataTable"/> with rows describing the columns in the schema
        /// for the configured provider. Provides the same result as 
        /// <see cref="IDataReader.GetSchemaTable"/>.
        /// </summary>
        /// <seealso cref="IDataReader.GetSchemaTable"/>
        /// <returns>A DataTable that describes the column metadata.</returns>
        DataTable GetSchemaTable();

        /// <summary>
        /// Gets the locale of the data as a CultureInfo.
        /// </summary>
        CultureInfo Locale { get; }

        /// <summary>
        /// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
        /// present in the IProvider with the given connection.
        /// </summary>
        /// <param name="table">The FeatureDataTable to configure the schema of.</param>
        void SetTableSchema(FeatureDataTable table);
    }
}