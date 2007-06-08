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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using SharpMap.CoordinateSystems;
using SharpMap.Indexing;
using SharpMap.Indexing.RTree;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Shapefile geometry type.
    /// </summary>
    public enum ShapeType : int
    {
        /// <summary>
        /// Null shape with no geometric data
        /// </summary>
        Null = 0,
        /// <summary>
        /// A point consists of a pair of double-precision coordinates.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.Point"/>
        /// </summary>
        Point = 1,
        /// <summary>
        /// PolyLine is an ordered set of vertices that consists of one or more parts. A part is a
        /// connected sequence of two or more points. Parts may or may not be connected to one
        ///	another. Parts may or may not intersect one another.
        /// SharpMap interpretes this as either <see cref="SharpMap.Geometries.LineString"/> or <see cref="SharpMap.Geometries.MultiLineString"/>
        /// </summary>
        PolyLine = 3,
        /// <summary>
        /// A polygon consists of one or more rings. A ring is a connected sequence of four or more
        /// points that form a closed, non-self-intersecting loop. A polygon may contain multiple
        /// outer rings. The order of vertices or orientation for a ring indicates which side of the ring
        /// is the interior of the polygon. The neighborhood to the right of an observer walking along
        /// the ring in vertex order is the neighborhood inside the polygon. Vertices of rings defining
        /// holes in polygons are in a counterclockwise direction. Vertices for a single, ringed
        /// polygon are, therefore, always in clockwise order. The rings of a polygon are referred to
        /// as its parts.
        /// SharpMap interpretes this as either <see cref="SharpMap.Geometries.Polygon"/> or <see cref="SharpMap.Geometries.MultiPolygon"/>
        /// </summary>
        Polygon = 5,
        /// <summary>
        /// A MultiPoint represents a set of points.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.MultiPoint"/>
        /// </summary>
        MultiPoint = 8,
        /// <summary>
        /// A PointZ consists of a triplet of double-precision coordinates plus a measure.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.Point"/>
        /// </summary>
        PointZ = 11,
        /// <summary>
        /// A PolyLineZ consists of one or more parts. A part is a connected sequence of two or
        /// more points. Parts may or may not be connected to one another. Parts may or may not
        /// intersect one another.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.LineString"/> or <see cref="SharpMap.Geometries.MultiLineString"/>
        /// </summary>
        PolyLineZ = 13,
        /// <summary>
        /// A PolygonZ consists of a number of rings. A ring is a closed, non-self-intersecting loop.
        /// A PolygonZ may contain multiple outer rings. The rings of a PolygonZ are referred to as
        /// its parts.
        /// SharpMap interpretes this as either <see cref="SharpMap.Geometries.Polygon"/> or <see cref="SharpMap.Geometries.MultiPolygon"/>
        /// </summary>
        PolygonZ = 15,
        /// <summary>
        /// A MultiPointZ represents a set of <see cref="PointZ"/>s.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.MultiPoint"/>
        /// </summary>
        MultiPointZ = 18,
        /// <summary>
        /// A PointM consists of a pair of double-precision coordinates in the order X, Y, plus a measure M.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.Point"/>
        /// </summary>
        PointM = 21,
        /// <summary>
        /// A shapefile PolyLineM consists of one or more parts. A part is a connected sequence of
        /// two or more points. Parts may or may not be connected to one another. Parts may or may
        /// not intersect one another.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.LineString"/> or <see cref="SharpMap.Geometries.MultiLineString"/>
        /// </summary>
        PolyLineM = 23,
        /// <summary>
        /// A PolygonM consists of a number of rings. A ring is a closed, non-self-intersecting loop.
        /// SharpMap interpretes this as either <see cref="SharpMap.Geometries.Polygon"/> or <see cref="SharpMap.Geometries.MultiPolygon"/>
        /// </summary>
        PolygonM = 25,
        /// <summary>
        /// A MultiPointM represents a set of <see cref="PointM"/>s.
        /// SharpMap interpretes this as <see cref="SharpMap.Geometries.MultiPoint"/>
        /// </summary>
        MultiPointM = 28,
        /// <summary>
        /// A MultiPatch consists of a number of surface patches. Each surface patch describes a
        /// surface. The surface patches of a MultiPatch are referred to as its parts, and the type of
        /// part controls how the order of vertices of an MultiPatch part is interpreted.
        /// SharpMap doesn't support this feature type.
        /// </summary>
        MultiPatch = 31
    };

    /// <summary>
    /// Shapefile dataprovider
    /// </summary>
    /// <remarks>
    /// <para>The ShapeFile provider is used for accessing ESRI ShapeFiles. The ShapeFile should at least contain the
    /// [filename].shp and, if feature-data is to be used, also [filename].dbf file.</para>
    /// <para>The first time the ShapeFile is accessed, SharpMap will automatically create a spatial index
    /// of the Shapefile, and save it as [filename].shp.sidx. If you change or update the contents of the .shp file,
    /// delete the .sidx file to force SharpMap to rebuild it. In web applications, the index will automatically
    /// be cached to memory for faster access, so to reload the index, you will need to restart the web application
    /// as well.</para>
    /// <para>
    /// M and Z values in a shapefile is ignored by SharpMap.
    /// </para>
    /// </remarks>
    /// <example>
    /// Adding a datasource to a layer:
    /// <code lang="C#">
    /// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
    /// myLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\data\MyShapeData.shp");
    /// </code>
    /// </example>
    public class ShapeFile : SharpMap.Data.Providers.IWritableProvider<uint>, IDisposable
    {
        private static readonly int HeaderSizeBytes = 100;
        private static readonly int HeaderStartCode = 9994;
        private static readonly int VersionCode = 1000;
        private static readonly int ShapeRecordHeaderByteLength = 8;
        private static readonly int BoundingBoxFieldByteLength = 32;
        public static readonly string IdColumnName = "OID";

        private FilterMethod _filterDelegate;
        private ShapeType _shapeType;
        private int _srid = -1;
        private string _filename;
        private BoundingBox _envelope = BoundingBox.Empty;
        private DbaseReader _dbaseReader;
        private DbaseWriter _dbaseWriter;
        private FileStream _shapeFileStream;
        private BinaryReader _shapeFileReader;
        private BinaryWriter _shapeFileWriter;
        private bool _fileBasedIndex;
        private bool _isOpen;
        private bool _coordsysReadFromFile = false;
        private bool _exclusiveMode = false;
        private ICoordinateSystem _coordinateSystem;
        Dictionary<uint, IndexEntry> _shapeIndex = new Dictionary<uint, IndexEntry>();

        /// <summary>
        /// Tree used for fast query of data
        /// </summary>
        private DynamicRTree<uint> _tree;

        #region Public Methods and Properties (SharpMap Shapefile API)

        public static ShapeFile Create(string directory, string layerName, ShapeType type)
        {
            return Create(directory, layerName, type, null);
        }

        public static ShapeFile Create(string directory, string layerName, ShapeType type, FeatureDataTable model)
        {
            if (String.IsNullOrEmpty(directory) || directory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new ArgumentException("Parameter must be a valid path", "path");
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            return Create(directoryInfo, layerName, type, model);
        }

        public static ShapeFile Create(DirectoryInfo directory, string layerName, ShapeType type, FeatureDataTable model)
        {
            if (String.IsNullOrEmpty(layerName))
            {
                throw new ArgumentNullException("layerName");
            }

            if (layerName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("Parameter cannot have invalid filename characters", "layerName");
            }

            if (!String.IsNullOrEmpty(Path.GetExtension(layerName)))
            {
                layerName = Path.GetFileNameWithoutExtension(layerName);
            }

            System.Data.DataTable schema = null;

            if (model != null)
            {
                schema = DbaseSchema.DeriveSchemaTable(model);
            }

            string shapeFile = Path.Combine(directory.FullName, layerName + ".shp");

            using (MemoryStream buffer = new MemoryStream(100))
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write(GetBigEndian(HeaderStartCode));
                writer.Write(new byte[20]);
                writer.Write(GetBigEndian(HeaderSizeBytes / 2));
                writer.Write(GetLittleEndian(VersionCode));
                writer.Write(GetLittleEndian((int)type));
                writer.Write(GetLittleEndian(0.0));
                writer.Write(GetLittleEndian(0.0));
                writer.Write(GetLittleEndian(0.0));
                writer.Write(GetLittleEndian(0.0));
                writer.Write(new byte[32]); // Z-values and M-values

                byte[] header = buffer.ToArray();

                using (FileStream shape = File.Create(shapeFile))
                {
                    shape.Write(header, 0, header.Length);
                }

                using (FileStream index = File.Create(Path.Combine(directory.FullName, layerName + ".shx")))
                {
                    index.Write(header, 0, header.Length);
                }
            }

            if (schema != null)
            {
                using (FileStream dbf = File.Create(Path.Combine(directory.FullName, layerName + ".dbf")))
                {
                    DbaseWriter dbaseWriter = new DbaseWriter(dbf, schema);
                    dbaseWriter.Close();
                }
            }

            return new ShapeFile(shapeFile);
        }

        /// <summary>
        /// Initializes a ShapeFile DataProvider without a file-based spatial index.
        /// </summary>
        /// <param name="filename">Path to shape file</param>
        public ShapeFile(string filename) : this(filename, false) { }

        /// <summary>
        /// Initializes a ShapeFile DataProvider.
        /// </summary>
        /// <remarks>
        /// <para>If FileBasedIndex is true, the spatial index will be read from a local copy. If it doesn't exist,
        /// it will be generated and saved to [filename] + '.sidx'.</para>
        /// <para>Using a file-based index is especially recommended for ASP.NET applications which will speed up
        /// start-up time when the cache has been emptied.
        /// </para>
        /// </remarks>
        /// <param name="filename">Path to shape file</param>
        /// <param name="fileBasedIndex">Use file-based spatial index</param>
        public ShapeFile(string filename, bool fileBasedIndex)
        {
            _filename = filename;
            _fileBasedIndex = fileBasedIndex;

            // Initialize DBF
            if (HasDbf)
            {
                _dbaseReader = new DbaseReader(DbfFilename);
            }
        }

        /// <summary>
        /// Forces a rebuild of the spatial index. If the instance of the ShapeFile provider
        /// uses a file-based index the file is rewritten to disk.
        /// </summary>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is executed and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public void RebuildSpatialIndex()
        {
            checkOpen();
            enableReading();

            if (this._fileBasedIndex)
            {
                if (System.IO.File.Exists(_filename + ".sidx"))
                {
                    System.IO.File.Delete(_filename + ".sidx");
                }

                _tree = CreateSpatialIndexFromFile(_filename);
            }
            else
            {
                _tree = CreateSpatialIndex();
            }

            // TODO: Remove this when connection pooling is implemented:
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Cache.Insert(_filename, _tree, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
            }
        }

        /// <summary>
        /// Gets or sets the coordinate system of the ShapeFile. If a shapefile has 
        /// a corresponding [filename].prj file containing a Well-Known Text 
        /// description of the coordinate system this will automatically be read.
        /// If this is not the case, the coordinate system will default to null.
        /// </summary>
        /// <exception cref="InvalidShapeFileOperationException">Thrown if property is set and the coordinate system is read from file.</exception>
        public ICoordinateSystem CoordinateSystem
        {
            get { return _coordinateSystem; }
            set
            {
                //checkOpen();
                if (_coordsysReadFromFile)
                {
                    throw new InvalidShapeFileOperationException("Coordinate system is specified in projection file and is read only");
                }

                _coordinateSystem = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="SharpMap.Data.Providers.ShapeType">shape geometry type</see> in this shapefile.
        /// </summary>
        /// <remarks>
        /// The property isn't set until the first time the datasource has been opened,
        /// and will throw an exception if this property has been called since initialization. 
        /// <para>All the non-Null shapes in a shapefile are required to be of the same shape
        /// type.</para>
        /// </remarks>
        /// <exception cref="InvalidShapefileOperationException">Thrown if property is read and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public ShapeType ShapeType
        {
            get
            {
                checkOpen();
                return _shapeType;
            }
        }


        /// <summary>
        /// Gets or sets the filename of the shapefile
        /// </summary>
        /// <remarks>If the filename changes, indexes will be rebuilt</remarks>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is executed and the shapefile is open. Check <see cref="IsOpen"/> before calling.</exception>
        /// <exception cref="FileNotFoundException">Thrown if setting and the specified filename can't be found.</exception>
        /// <exception cref="InvalidShapeFileException">Thrown if the shapefile cannot be opened</exception>
        public string Filename
        {
            get { return _filename; }
            set
            {
                if (value != _filename)
                {
                    if (this.IsOpen)
                    {
                        throw new InvalidShapeFileOperationException("Cannot change filename while datasource is open");
                    }

                    if (!File.Exists(value))
                    {
                        throw new FileNotFoundException("Can't find the shapefile specified", value);
                    }

                    if (Path.GetExtension(value).ToLower() != ".shp")
                    {
                        throw new InvalidShapeFileException("Invalid shapefile filename: " + value);
                    }

                    try
                    {
                        File.OpenRead(value).Close();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidShapeFileException("Can't open shapefile", ex);
                    }

                    _filename = value;
                }
            }
        }

        /// <summary>
        /// Gets the record index (.shx file) filename for the given shapefile
        /// </summary>
        public string IndexFilename
        {
            get { return Path.Combine(Path.GetDirectoryName(_filename), Path.GetFileNameWithoutExtension(_filename) + ".shx"); }
        }

        public string DbfFilename
        {
            get { return Path.Combine(Path.GetDirectoryName(_filename), Path.GetFileNameWithoutExtension(_filename) + ".dbf"); }
        }

        public bool HasDbf
        {
            get { return File.Exists(DbfFilename); }
        }

        /// <summary>
        /// Gets or sets the encoding used for parsing strings from the DBase DBF file.
        /// </summary>
        /// <remarks>
        /// The DBase default encoding is <see cref="System.Text.Encoding.UTF7"/>.
        /// </remarks>
        /// <exception cref="InvalidShapefileOperationException">Thrown if property is read or set and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        /// <exception cref="InvalidShapefileException">Thrown if set and there is no Dbase file with this shapefile</exception>
        public System.Text.Encoding Encoding
        {
            get
            {
                checkOpen();

                if (!HasDbf)
                {
                    return System.Text.Encoding.UTF7;
                }

                enableReading();
                return _dbaseReader.Encoding;
            }
            set
            {
                if (!HasDbf)
                {
                    throw new InvalidShapeFileException("The Encoding property can't be set when there is no Dbase file (.dbf) associated with this shapefile");
                }

                checkOpen();
                enableReading();
                _dbaseReader.Encoding = value;
            }
        }

        #region IProvider Members
        /// <summary>
        /// Returns true if the datasource is currently open
        /// </summary>		
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Opens the datasource
        /// </summary>
        public void Open()
        {
            Open(false);
        }

        /// <summary>
        /// Opens the shapefile with optional exclusive access for faster write performance during bulk updates.
        /// </summary>
        /// <param name="exclusive">True if exclusive access is desired, false otherwise</param>
        public void Open(bool exclusive)
        {
            // TODO:
            // Get a Connector.  The connector returned is guaranteed to be connected and ready to go.
            // Pooling.Connector connector = Pooling.ConnectorPool.ConnectorPoolManager.RequestConnector(this,true);

            if (!_isOpen)
            {
                _exclusiveMode = exclusive;

                try
                {
                    enableReading();
                    _isOpen = true;

                    // Parse shape header
                    ParseHeader();

                    // Read projection file
                    ParseProjection();

                    // Read in .shx index
                    ParseIndex();

                    // Load spatial (r-tree) index
                    LoadSpatialIndex(_fileBasedIndex);
                }
                catch (Exception)
                {
                    _isOpen = false;
                    throw;
                }
            }
        }

        /// <summary>
        /// Closes the datasource
        /// </summary>
        public void Close()
        {
            if (!disposed)
            {
                // TODO: (ConnectionPooling)
                /*	if (connector != null)
                    { Pooling.ConnectorPool.ConnectorPoolManager.Release...()
                }*/
                if (_isOpen)
                {
                    if (_shapeFileWriter != null)
                    {
                        _shapeFileWriter.Close();
                    }
                    if (_shapeFileReader != null)
                    {
                        _shapeFileReader.Close();
                    }
                    if (_shapeFileStream != null)
                    {
                        _shapeFileStream.Close();
                    }
                    if (_dbaseReader != null)
                    {
                        _dbaseReader.Close();
                    }

                    _isOpen = false;
                }

                if (_tree != null)
                {
                    _tree.Dispose();
                }
            }
        }

        /// <summary>
        /// Returns geometries whose bounding box intersects 'bbox'
        /// </summary>
        /// <remarks>
        /// <para>Please note that this method doesn't guarantee that the geometries returned actually intersect 'bbox', but only
        /// that their boundingbox intersects 'bbox'.</para>
        /// <para>This method is much faster than the QueryFeatures method, because intersection tests
        /// are performed on objects simplifed by their boundingbox, and using the Spatial Index.</para>
        /// </remarks>
        /// <param name="bbox"><see cref="BoundingBox"/> which determines the view</param>
        /// <returns>A <see cref="List{Geometry}"/> containing the <see cref="Geometry"/> objects which are at least partially contained within the <paramref name="bbox">view</paramref>.</returns>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public ReadOnlyCollection<Geometry> GetGeometriesInView(BoundingBox bbox)
        {
            checkOpen();
            enableReading();

            //Use the spatial index to get a list of features whose boundingbox intersects bbox
            ReadOnlyCollection<uint> objectlist = GetObjectIdsInView(bbox);

            if (objectlist.Count == 0) //no features found. Return an empty set
            {
                return new List<Geometry>().AsReadOnly();
            }

            List<Geometry> geometries = new List<Geometry>(objectlist.Count);

            foreach (uint oid in objectlist)
            {
                Geometry g = GetGeometryById(oid);

                if (!Object.ReferenceEquals(g, null))
                {
                    geometries.Add(g);
                }
            }

            return geometries.AsReadOnly();
        }

        /// <summary>
        /// Returns all objects whose boundingbox intersects bbox.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Please note that this method doesn't guarantee that the geometries returned actually intersect 'bbox', but only
        /// that their boundingbox intersects 'bbox'.
        /// </para>
        /// <para>This method is much faster than the QueryFeatures method, because intersection tests
        /// are performed on objects simplifed by their boundingbox, and using the Spatial Index.</para>
        /// </remarks>
        /// <param name="bbox"><see cref="BoundingBox"/> which determines the view</param>
        /// <param name="ds">The <see cref="SharpMap.Data.FeatureDataSet"/> to fill with features within the <paramref name="bbox">view</paramref>.</param>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public void ExecuteIntersectionQuery(BoundingBox bbox, FeatureDataSet ds)
        {
            checkOpen();
            enableReading();

            //Use the spatial index to get a list of features whose boundingbox intersects bbox
            ReadOnlyCollection<uint> objectlist = GetObjectIdsInView(bbox);
            FeatureDataTable<uint> dt = HasDbf ? _dbaseReader.NewTable : FeatureDataTable<uint>.CreateEmpty(IdColumnName);

            foreach (uint oid in objectlist)
            {
                FeatureDataRow<uint> fdr = HasDbf ? _dbaseReader.GetFeature(oid, dt) : dt.NewRow(oid);

                fdr.Geometry = readGeometry(oid);

                if (fdr.Geometry != null)
                {
                    if (fdr.Geometry.GetBoundingBox().Intersects(bbox))
                    {
                        if (FilterDelegate == null || FilterDelegate(fdr))
                        {
                            dt.AddRow(fdr);
                        }
                    }
                }
            }

            ds.Tables.Add(dt);
        }

        /// <summary>
        /// Returns geometry Object IDs whose bounding box intersects 'bbox'
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public ReadOnlyCollection<uint> GetObjectIdsInView(BoundingBox bbox)
        {
            checkOpen();
            enableReading();

            if (bbox.Contains(_envelope) || bbox == _envelope)
            {
                // Yea, I know creating a list of numbers can be expressed in a 
                // half-line in other languages (Python, Lisp, etc), but not in C#
                List<uint> idList = new List<uint>(16 < _shapeIndex.Count ? _shapeIndex.Count : 16);

                for (uint id = 0; id < _shapeIndex.Count; id++)
                {
                    idList.Add(id);
                }

                return idList.AsReadOnly();
            }

            //Use the spatial index to get a list of features whose boundingbox intersects bbox
            return new List<uint>(getKeysFromIndexEntries(_tree.Search(bbox))).AsReadOnly();
        }

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns><see cref="Geometry"/></returns>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public Geometry GetGeometryById(uint oid)
        {
            checkOpen();
            enableReading();

            if (FilterDelegate != null) //Apply filtering
            {
                FeatureDataRow fdr = GetFeature(oid);

                if (fdr != null)
                {
                    return fdr.Geometry;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return readGeometry(oid);
            }
        }

        /// <summary>
        /// Returns the data associated with all the geometries that are intersected by 'geom'.
        /// Please note that the ShapeFile provider currently doesn't fully support geometryintersection
        /// and thus only BoundingBox/BoundingBox querying are performed. The results are NOT
        /// guaranteed to lie withing 'geom'.
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="ds">FeatureDataSet to fill data into</param>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
        {
            checkOpen();
            enableReading();

            FeatureDataTable<uint> dt = HasDbf ? _dbaseReader.NewTable : FeatureDataTable<uint>.CreateEmpty(IdColumnName);
            BoundingBox boundingBox = geom.GetBoundingBox();

            //Get candidates by intersecting the spatial index tree
            IEnumerable<uint> oidList = getKeysFromIndexEntries(_tree.Search(boundingBox));

            foreach (uint oid in oidList)
            {
                for (uint i = (uint)dt.Rows.Count - 1; i >= 0; i--)
                {
                    FeatureDataRow<uint> fdr = GetFeature(oid, dt);

                    if (fdr.Geometry != null)
                    {
                        if (fdr.Geometry.GetBoundingBox().Intersects(boundingBox))
                        {
                            // TODO: replace above line with this:  if(fdr.Geometry.Intersects(bbox))  when relation model is complete
                            if (FilterDelegate == null || FilterDelegate(fdr))
                            {
                                dt.AddRow(fdr);
                            }
                        }
                    }
                }
            }

            ds.Tables.Add(dt);
        }


        /// <summary>
        /// Returns the total number of features in the datasource (without any filter applied)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public int GetFeatureCount()
        {
            if (IsOpen)
            {
                return _shapeIndex.Count;
            }
            else // Assume the feature count from the fixed record-length .shx file
            {
                FileInfo info = new FileInfo(IndexFilename);
                return (int)((info.Length - HeaderSizeBytes) / ShapeRecordHeaderByteLength);
            }
        }

        /// <summary>
        /// Filter Delegate Method
        /// </summary>
        /// <remarks>
        /// The FilterMethod delegate is used for applying a method that filters data from the dataset.
        /// The method should return 'true' if the feature should be included and false if not.
        /// <para>See the <see cref="FilterDelegate"/> property for more info</para>
        /// </remarks>
        /// <seealso cref="FilterDelegate"/>
        /// <param name="dr"><see cref="SharpMap.Data.FeatureDataRow"/> to test on</param>
        /// <returns>true if this feature should be included, false if it should be filtered</returns>
        public delegate bool FilterMethod(SharpMap.Data.FeatureDataRow dr);

        /// <summary>
        /// Filter Delegate Method for limiting the datasource
        /// </summary>
        /// <remarks>
        /// <example>
        /// Using an anonymous method for filtering all features where the NAME column starts with S:
        /// <code lang="C#">
        /// myShapeDataSource.FilterDelegate = new SharpMap.Data.Providers.ShapeFile.FilterMethod(delegate(SharpMap.Data.FeatureDataRow row) { return (!row["NAME"].ToString().StartsWith("S")); });
        /// </code>
        /// </example>
        /// <example>
        /// Declaring a delegate method for filtering (multi)polygon-features whose area is larger than 5.
        /// <code>
        /// myShapeDataSource.FilterDelegate = CountryFilter;
        /// [...]
        /// public static bool CountryFilter(SharpMap.Data.FeatureDataRow row)
        /// {
        ///		if(row.Geometry.GetType()==typeof(SharpMap.Geometries.Polygon))
        ///			return ((row.Geometry as SharpMap.Geometries.Polygon).Area>5);
        ///		if (row.Geometry.GetType() == typeof(SharpMap.Geometries.MultiPolygon))
        ///			return ((row.Geometry as SharpMap.Geometries.MultiPolygon).Area > 5);
        ///		else return true;
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <seealso cref="FilterMethod"/>
        public FilterMethod FilterDelegate
        {
            get
            {
                return _filterDelegate;
            }
            set
            {
                _filterDelegate = value;
            }
        }

        /*
        /// <summary>
        /// Returns a colleciton of columns from the datasource [NOT IMPLEMENTED]
        /// </summary>
        public System.Data.DataColumnCollection Columns
        {
            get {
                if (dbaseFile != null)
                {
                    System.Data.DataTable dt = dbaseFile.DataTable;
                    return dt.Columns;
                }
                else
                    throw (new ApplicationException("An attempt was made to read DBase data from a shapefile without a valid .DBF file"));
            }
        }*/

        /// <summary>
        /// Gets a feature row from the datasource with the specified id.
        /// </summary>
        /// <param name="oid">Id of the feautre to return.</param>
        /// <returns>The feature corresponding to <paramref name="oid" />, or null if no feature is found.</returns>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public FeatureDataRow<uint> GetFeature(uint oid)
        {
            return GetFeature(oid, null);
        }

        /// <summary>
        /// Gets a datarow from the datasource at the specified index belonging to the specified datatable
        /// </summary>
        /// <param name="RowID">Row number to fetch</param>
        /// <param name="dt">Datatable to feature should belong to.</param>
        /// <returns>Row corresponding to </returns>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public FeatureDataRow<uint> GetFeature(uint oid, FeatureDataTable<uint> dt)
        {
            checkOpen();
            enableReading();

            if (dt == null)
            {
                if (!HasDbf)
                {
                    dt = FeatureDataTable<uint>.CreateEmpty(IdColumnName);
                }
                else
                {
                    dt = _dbaseReader.NewTable;
                }
            }

            FeatureDataRow<uint> dr = HasDbf ? _dbaseReader.GetFeature(oid, dt) : dt.NewRow(oid);
            dr.Geometry = readGeometry(oid);

            if (FilterDelegate == null || FilterDelegate(dr))
            {
                return dr;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the extents of the datasource.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidShapefileOperationException">Thrown if the shapefile hasn't been spatially indexed. Call <see cref="RebuildSpatialIndex"/> to generate spatial index.</exception>
        public BoundingBox GetExtents()
        {
            //checkOpen();
            //if (_tree == null)
            //    throw new InvalidShapeFileOperationException("File hasn't been spatially indexed.");

            if (_tree != null)
            {
                return _tree.Root.BoundingBox;
            }

            if (_envelope != BoundingBox.Empty)
            {
                return _envelope;
            }

            enableReading();
            ParseHeader();
            return _envelope;
        }

        /// <summary>
        /// Gets the connection ID of the datasource.
        /// </summary>
        /// <remarks>
        /// The connection ID of a shapefile is its filename.
        /// </remarks>
        public string ConnectionId
        {
            get { return this._filename; }
        }

        /// <summary>
        /// Gets or sets the spatial reference ID (CRS)
        /// </summary>
        public int Srid
        {
            get { return _srid; }
            set { _srid = value; }
        }

        #endregion

        #region IWritableProvider Members

        /// <summary>
        /// Saves a feature to a shapefile
        /// </summary>
        /// <param name="feature">Feature to save</param>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public void Save(FeatureDataRow<uint> feature)
        {
            //throw new NotImplementedException("Not implemented in this version");

            checkOpen();
            if (feature == null)
                throw new ArgumentNullException("feature");

            enableWriting();

            writeFeatureRow(feature);

            writeIndex();
            writeHeader(_shapeFileWriter);
        }

        /// <summary>
        /// Saves features to a shapefile.
        /// </summary>
        /// <param name="feature">Enumeration of features to save.</param>
        /// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public void Save(IEnumerable<FeatureDataRow<uint>> rows)
        {
            checkOpen();
            if (rows == null)
            {
                throw new ArgumentNullException("rows");
            }

            enableWriting();

            foreach (FeatureDataRow row in rows)
            {
                writeFeatureRow(row);
            }

            writeIndex();
            writeHeader(_shapeFileWriter);
        }

        /// <summary>
        /// Saves features to the shapefile
        /// </summary>
        /// <param name="table">A table containing feature data and geometry</param>
        public void Save(FeatureDataTable<uint> table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            checkOpen();
            enableWriting();

            _shapeFileStream.Position = HeaderSizeBytes;
            foreach (FeatureDataRow row in table.Rows)
            {
                if (row is FeatureDataRow<uint>)
                {
                    _tree.Insert(new RTreeIndexEntry<uint>((row as FeatureDataRow<uint>).Id, row.Geometry.GetBoundingBox()));
                }
                else
                {
                    _tree.Insert(new RTreeIndexEntry<uint>(getNextId(), row.Geometry.GetBoundingBox()));
                }

                writeFeatureRow(row);
            }

            writeIndex();
            writeHeader(_shapeFileWriter);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="feature">Feature to delete.</param>
        public void Delete(FeatureDataRow<uint> feature)
        {
            throw new NotImplementedException("Not implemented in this version");
        }
        #endregion
        #endregion

        #region Disposers and finalizers

        private bool disposed = false;

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Close();
                    //_Envelope = null;
                    _tree = null;
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Finalizes the object
        /// </summary>
        ~ShapeFile()
        {
            Dispose(false);
        }
        #endregion

        #region General helper functions
        private void checkOpen()
        {
            if (!IsOpen)
            {
                throw new InvalidShapeFileOperationException("An attempt was made to access a closed datasource");
            }
        }

        private uint getNextId()
        {
            return (uint)_shapeIndex.Count;
        }

        private int computeFileLengthInWords()
        {
            int length = HeaderSizeBytes / 2;

            foreach (KeyValuePair<uint, IndexEntry> kvp in _shapeIndex)
            {
                length += kvp.Value.Length + ShapeFile.ShapeRecordHeaderByteLength / 2;
            }

            return length;
        }

        private int computeGeometryLengthInWords(Geometry geometry)
        {
            if (geometry == null)
            {
                throw new NotSupportedException("Writing null shapes not supported in this version");
            }

            int byteCount = 0;

            if (geometry is Point)
            {
                byteCount = 20; // ShapeType integer + 2 doubles at 8 bytes each
            }
            else if (geometry is MultiPoint)
            {
                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 /* NumPoints integer */ + 16 * (geometry as MultiPoint).Points.Count;
            }
            else if (geometry is LineString)
            {
                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 + 4 /* NumPoints and NumParts integers */
                    + 4 /* Parts Array 1 integer long */ + 16 * (geometry as LineString).Vertices.Count;
            }
            else if (geometry is MultiLineString)
            {
                int pointCount = 0;

                foreach (LineString line in (geometry as MultiLineString).LineStrings)
                {
                    pointCount += line.Vertices.Count;
                }

                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 + 4 /* NumPoints and NumParts integers */
                    + 4 * (geometry as MultiLineString).LineStrings.Count /* Parts array of integer indexes */
                    + 16 * pointCount;
            }
            else if (geometry is Polygon)
            {
                int pointCount = (geometry as Polygon).ExteriorRing.Vertices.Count;

                foreach (LinearRing ring in (geometry as Polygon).InteriorRings)
                {
                    pointCount += ring.Vertices.Count;
                }

                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 + 4 /* NumPoints and NumParts integers */
                    + 4 * ((geometry as Polygon).InteriorRings.Count + 1 /* Parts array of rings: count of interior + 1 for exterior ring */)
                    + 16 * pointCount;
            }
            else
            {
                throw new NotSupportedException("Currently unsupported geometry type.");
            }

            return byteCount / 2; // number of 16-bit words
        }


        private IEnumerable<uint> getKeysFromIndexEntries(IEnumerable<RTreeIndexEntry<uint>> entries)
        {
            foreach (RTreeIndexEntry<uint> entry in entries)
            {
                yield return entry.Value;
            }
        }
        #endregion

        #region Endian conversion helper routines
        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        private static int GetBigEndian(int value)
        {
            if (BitConverter.IsLittleEndian)
                return SwapByteOrder(value);
            else
                return value;
        }

        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        private static UInt16 GetBigEndian(UInt16 value)
        {
            if (BitConverter.IsLittleEndian)
                return SwapByteOrder(value);
            else
                return value;
        }

        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        private static UInt32 GetBigEndian(UInt32 value)
        {
            if (BitConverter.IsLittleEndian)
                return SwapByteOrder(value);
            else
                return value;
        }

        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        private static double GetBigEndian(double value)
        {
            if (BitConverter.IsLittleEndian)
                return SwapByteOrder(value);
            else
                return value;
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        private static int GetLittleEndian(int value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            else
                return SwapByteOrder(value);
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        private static UInt32 GetLittleEndian(UInt32 value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            else
                return SwapByteOrder(value);
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        private static UInt16 GetLittleEndian(UInt16 value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            else
                return SwapByteOrder(value);
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        private static double GetLittleEndian(double value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            else
                return SwapByteOrder(value);
        }

        ///<summary>
        ///Swaps the byte order of an Int32
        ///</summary>
        ///<param name="value">Int32 to swap</param>
        ///<returns>Byte Order swapped Int32</returns>
        private static int SwapByteOrder(int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }

        ///<summary>
        ///Swaps the byte order of a UInt16
        ///</summary>
        ///<param name="value">UInt16 to swap</param>
        ///<returns>Byte Order swapped UInt16</returns>
        private static UInt16 SwapByteOrder(UInt16 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToUInt16(buffer, 0);
        }

        ///<summary>
        ///Swaps the byte order of a UInt32
        ///</summary>
        ///<param name="value">UInt32 to swap</param>
        ///<returns>Byte Order swapped UInt32</returns>
        private static UInt32 SwapByteOrder(UInt32 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }

        ///<summary>
        ///Swaps the byte order of a Double (double precision IEEE 754)
        ///</summary>
        ///<param name="value">Double to swap</param>
        ///<returns>Byte Order swapped Double</returns>
        private static double SwapByteOrder(double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToDouble(buffer, 0);
        }
        #endregion

        #region Spatial indexing helper functions
        /// <summary>
        /// Loads a spatial index from a file. If it doesn't exist, one is created and saved
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>QuadTree index</returns>
        private DynamicRTree<uint> CreateSpatialIndexFromFile(string filename)
        {
            if (System.IO.File.Exists(filename + ".sidx"))
            {
                try
                {
                    using (FileStream indexStream = new FileStream(filename + ".sidx", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return DynamicRTree<uint>.FromStream(indexStream);
                    }
                }
                catch (ObsoleteIndexFileFormatException)
                {
                    File.Delete(filename + ".sidx");
                    return CreateSpatialIndexFromFile(filename);
                }
                catch (Exception) { throw; }
            }
            else
            {
                DynamicRTree<uint> tree = CreateSpatialIndex();

                using (FileStream indexStream = new FileStream(filename + ".sidx", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    tree.SaveIndex(indexStream);
                }

                return tree;
            }
        }

        /// <summary>
        /// Generates a spatial index for a specified shape file.
        /// </summary>
        /// <param name="filename"></param>
        private DynamicRTree<uint> CreateSpatialIndex()
        {
            // TODO: implement Post-optimization restructure strategy
            IIndexRestructureStrategy restructureStrategy = new NullRestructuringStrategy();
            RestructuringHuristic restructureHeuristic = new RestructuringHuristic(RestructureOpportunity.Default, 4.0);
            IEntryInsertStrategy<RTreeIndexEntry<uint>> insertStrategy = new GuttmanQuadraticInsert<uint>();
            INodeSplitStrategy nodeSplitStrategy = new GuttmanQuadraticSplit<uint>();
            DynamicRTreeBalanceHeuristic indexHeuristic = new DynamicRTreeBalanceHeuristic(4, 10, UInt16.MaxValue);

            DynamicRTree<uint> index = new SelfOptimizingDynamicSpatialIndex<uint>(restructureStrategy, restructureHeuristic, insertStrategy, nodeSplitStrategy, indexHeuristic);

            FeatureDataSet features = new FeatureDataSet();
            ExecuteIntersectionQuery(GetExtents(), features);

            if (features.Tables.Count < 1)
            {
                return index;
            }

            FeatureDataTable<uint> featureTable = features.Tables[0] as FeatureDataTable<uint>;

            foreach (FeatureDataRow<uint> feature in featureTable)
            {
                BoundingBox box = feature.Geometry.GetBoundingBox();

                if (!double.IsNaN(box.Left) && !double.IsNaN(box.Right) && !double.IsNaN(box.Bottom) && !double.IsNaN(box.Top))
                {
                    index.Insert(new RTreeIndexEntry<uint>(feature.Id, box));
                }
            }

            return index;
        }

        private void LoadSpatialIndex() 
        { 
            LoadSpatialIndex(false, false); 
        }

        private void LoadSpatialIndex(bool LoadFromFile) 
        { 
            LoadSpatialIndex(false, LoadFromFile); 
        }

        private void LoadSpatialIndex(bool ForceRebuild, bool LoadFromFile)
        {
            //Only load the tree if we haven't already loaded it, or if we want to force a rebuild
            if (_tree == null || ForceRebuild)
            {
                // Is this a web application? If so lets store the index in the cache so we don't
                // need to rebuild it for each request
                if (System.Web.HttpContext.Current != null)
                {
                    //Check if the tree exists in the cache
                    if (System.Web.HttpContext.Current.Cache[_filename] != null)
                    {
                        _tree = (DynamicRTree<uint>)System.Web.HttpContext.Current.Cache[_filename];
                    }
                    else
                    {
                        if (!LoadFromFile)
                        {
                            _tree = CreateSpatialIndex();
                        }
                        else
                        {
                            _tree = CreateSpatialIndexFromFile(_filename);
                        }

                        //Store the tree in the web cache
                        // TODO: Remove this when connection pooling is implemented
                        System.Web.HttpContext.Current.Cache.Insert(_filename, _tree, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
                    }
                }
                else
                {
                    if (!LoadFromFile)
                    {
                        _tree = CreateSpatialIndex();
                    }
                    else
                    {
                        _tree = CreateSpatialIndexFromFile(_filename);
                    }
                }
            }
        }
        #endregion

        #region Geometry reading helper functions
        /// <summary>
        /// Reads all boundingboxes of features in the shapefile. This is used for spatial indexing.
        /// </summary>
        /// <returns></returns>
        private List<BoundingBox> getAllFeatureBoundingBoxes()
        {
            //int[] offsetOfRecord = ReadIndex(); //Read the whole .idx file

            enableReading();
            List<BoundingBox> boxes = new List<BoundingBox>();

            foreach (KeyValuePair<uint, IndexEntry> kvp in _shapeIndex)
            {
                _shapeFileStream.Seek(kvp.Value.AbsoluteByteOffset + ShapeRecordHeaderByteLength, SeekOrigin.Begin);

                if ((ShapeType)GetLittleEndian(_shapeFileReader.ReadInt32()) != ShapeType.Null)
                {
                    double xMin = GetLittleEndian(_shapeFileReader.ReadDouble());
                    double yMin = GetLittleEndian(_shapeFileReader.ReadDouble());
                    double xMax, yMax;

                    if (_shapeType == ShapeType.Point)
                    {
                        xMax = xMin;
                        yMax = yMin;
                    }
                    else
                    {
                        xMax = GetLittleEndian(_shapeFileReader.ReadDouble());
                        yMax = GetLittleEndian(_shapeFileReader.ReadDouble());
                    }

                    boxes.Add(new BoundingBox(xMin, yMin, yMax, yMax));
                }
            }

            return boxes;
        }

        /// <summary>
        /// Reads and parses the geometry with ID 'oid' from the ShapeFile
        /// </summary>
        /// <remarks><see cref="FilterDelegate">Filtering</see> is not applied to this method</remarks>
        /// <param name="oid">Object ID</param>
        /// <returns><see cref="SharpMap.Geometries.Geometry"/> instance from the Shapefile corresponding to <paramref name="oid"/></returns>
        private Geometry readGeometry(uint oid)
        {
            enableReading();
            _shapeFileReader.BaseStream.Seek(_shapeIndex[oid].AbsoluteByteOffset + ShapeRecordHeaderByteLength, SeekOrigin.Begin);
            ShapeType type = (ShapeType)GetLittleEndian(_shapeFileReader.ReadInt32()); //Shape type

            if (type == ShapeType.Null)
                return null;

            switch (_shapeType)
            {
                case ShapeType.Point:
                    return readPoint();
                case ShapeType.PolyLine:
                    return readPolyLine();
                case ShapeType.Polygon:
                    return readPolygon();
                case ShapeType.MultiPoint:
                    return readMultiPoint();
                case ShapeType.PointZ:
                    return readPointZ();
                case ShapeType.PolyLineZ:
                    return readPolyLineZ();
                case ShapeType.PolygonZ:
                    return readPolygonZ();
                case ShapeType.MultiPointZ:
                    return readMultiPointZ();
                case ShapeType.PointM:
                    return readPointM();
                case ShapeType.PolyLineM:
                    return readPolyLineM();
                case ShapeType.PolygonM:
                    return readPolygonM();
                case ShapeType.MultiPointM:
                    return readMultiPointM();
                default:
                    throw new UnsupportedShapefileGeometryException("Shapefile type " + _shapeType.ToString() + " not supported");
            }
        }

        private Geometry readMultiPointM()
        {
            throw new NotSupportedException("MultiPointM features are not currently supported");
        }

        private Geometry readPolygonM()
        {
            throw new NotSupportedException("PolygonM features are not currently supported");
        }

        private Geometry readMultiPointZ()
        {
            throw new NotSupportedException("MultiPointZ features are not currently supported");
        }

        private Geometry readPolyLineZ()
        {
            throw new NotSupportedException("PolyLineZ features are not currently supported");
        }

        private Geometry readPolyLineM()
        {
            throw new NotSupportedException("PolyLineM features are not currently supported");
        }

        private Geometry readPointM()
        {
            throw new NotSupportedException("PointM features are not currently supported");
        }

        private Geometry readPolygonZ()
        {
            throw new NotSupportedException("PolygonZ features are not currently supported");
        }

        private Geometry readPointZ()
        {
            throw new NotSupportedException("PointZ features are not currently supported");
        }

        private Geometry readPoint()
        {
            Point tempFeature = new Point();
            return new Point(GetLittleEndian(_shapeFileReader.ReadDouble()), GetLittleEndian(_shapeFileReader.ReadDouble()));
        }

        private Geometry readMultiPoint()
        {
            _shapeFileReader.BaseStream.Seek(BoundingBoxFieldByteLength, SeekOrigin.Current); //skip min/max box
            MultiPoint feature = new MultiPoint();
            int nPoints = GetLittleEndian(_shapeFileReader.ReadInt32()); // get the number of points

            if (nPoints == 0)
            {
                return null;
            }

            for (int i = 0; i < nPoints; i++)
            {
                feature.Points.Add(new Point(GetLittleEndian(_shapeFileReader.ReadDouble()), GetLittleEndian(_shapeFileReader.ReadDouble())));
            }

            return feature;
        }

        private void readPolyStructure(out int parts, out int points, out int[] segments)
        {
            _shapeFileReader.BaseStream.Seek(BoundingBoxFieldByteLength, SeekOrigin.Current); //skip min/max box
            parts = GetLittleEndian(_shapeFileReader.ReadInt32()); // get number of parts (segments)
            points = GetLittleEndian(_shapeFileReader.ReadInt32()); // get number of points
            segments = new int[parts + 1];

            //Read in the segment indexes
            for (int b = 0; b < parts; b++)
            {
                segments[b] = GetLittleEndian(_shapeFileReader.ReadInt32());
            }

            //add end point
            segments[parts] = points;
        }

        private Geometry readPolyLine()
        {
            int parts;
            int points;
            int[] segments;
            readPolyStructure(out parts, out points, out segments);

            if (parts == 0)
            {
                return null;
            }

            MultiLineString mline = new MultiLineString();

            for (int lineId = 0; lineId < parts; lineId++)
            {
                LineString line = new LineString();

                for (int i = segments[lineId]; i < segments[lineId + 1]; i++)
                {
                    line.Vertices.Add(new Point(GetLittleEndian(_shapeFileReader.ReadDouble()), GetLittleEndian(_shapeFileReader.ReadDouble())));
                }

                mline.LineStrings.Add(line);
            }

            if (mline.LineStrings.Count == 1)
            {
                return mline[0];
            }

            return mline;
        }

        private Geometry readPolygon()
        {
            int parts;
            int points;
            int[] segments;
            readPolyStructure(out parts, out points, out segments);

            if (parts == 0)
                return null;

            //First read all the rings
            List<LinearRing> rings = new List<LinearRing>();

            for (int ringId = 0; ringId < parts; ringId++)
            {
                LinearRing ring = new LinearRing();

                for (int i = segments[ringId]; i < segments[ringId + 1]; i++)
                {
                    ring.Vertices.Add(new Point(GetLittleEndian(_shapeFileReader.ReadDouble()), GetLittleEndian(_shapeFileReader.ReadDouble())));
                }

                rings.Add(ring);
            }

            bool[] isCounterClockWise = new bool[rings.Count];
            int PolygonCount = 0;

            for (int i = 0; i < rings.Count; i++)
            {
                isCounterClockWise[i] = rings[i].IsCcw();

                if (!isCounterClockWise[i])
                {
                    PolygonCount++;
                }
            }

            if (PolygonCount == 1) //We only have one polygon
            {
                Polygon poly = new Polygon();
                poly.ExteriorRing = rings[0];

                if (rings.Count > 1)
                {
                    for (int i = 1; i < rings.Count; i++)
                    {
                        poly.InteriorRings.Add(rings[i]);
                    }
                }

                return poly;
            }
            else
            {
                MultiPolygon mpoly = new MultiPolygon();
                Polygon poly = new Polygon();
                poly.ExteriorRing = rings[0];

                for (int i = 1; i < rings.Count; i++)
                {
                    if (!isCounterClockWise[i])
                    {
                        mpoly.Polygons.Add(poly);
                        poly = new Polygon(rings[i]);
                    }
                    else
                    {
                        poly.InteriorRings.Add(rings[i]);
                    }
                }

                mpoly.Polygons.Add(poly);
                return mpoly;
            }
        }
        #endregion

        #region File parsing helper functions

        /// <summary>
        /// Reads and parses the header of the .shp index file
        /// </summary>
        /// <remarks>
        /// From ESRI Shapefile Technical Description document
        /// 
        /// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
        /// 
        /// Byte
        /// Position    Field           Value       Type    Order
        /// -----------------------------------------------------
        /// Byte 0      File Code       9994        Integer Big
        /// Byte 4      Unused          0           Integer Big
        /// Byte 8      Unused          0           Integer Big
        /// Byte 12     Unused          0           Integer Big
        /// Byte 16     Unused          0           Integer Big
        /// Byte 20     Unused          0           Integer Big
        /// Byte 24     File Length     File Length Integer Big
        /// Byte 28     Version         1000        Integer Little
        /// Byte 32     Shape Type      Shape Type  Integer Little
        /// Byte 36     Bounding Box    Xmin        Double  Little
        /// Byte 44     Bounding Box    Ymin        Double  Little
        /// Byte 52     Bounding Box    Xmax        Double  Little
        /// Byte 60     Bounding Box    Ymax        Double  Little
        /// Byte 68*    Bounding Box    Zmin        Double  Little
        /// Byte 76*    Bounding Box    Zmax        Double  Little
        /// Byte 84*    Bounding Box    Mmin        Double  Little
        /// Byte 92*    Bounding Box    Mmax        Double  Little
        /// 
        /// * Unused, with value 0.0, if not Measured or Z type
        /// 
        /// The "Integer" type corresponds to the CLS Int32 type, and "Double" to CLS Double (IEEE 754).
        /// </remarks>
        private void ParseHeader()
        {
            _shapeFileReader.BaseStream.Seek(0, SeekOrigin.Begin);
            //Check file header
            if (GetBigEndian(_shapeFileReader.ReadInt32()) != HeaderStartCode)
            {
                throw new InvalidShapeFileException("Invalid Shapefile (.shp)");
            }

            _shapeFileReader.BaseStream.Seek(24, 0); //seek to File Length
            int fileLength = GetBigEndian(_shapeFileReader.ReadInt32()); //Read filelength as big-endian. The length is number of 16-bit words in file

            _shapeFileReader.BaseStream.Seek(32, 0); //seek to ShapeType
            _shapeType = (ShapeType)_shapeFileReader.ReadInt32();

            //Read the spatial bounding box of the contents
            _shapeFileReader.BaseStream.Seek(36, 0); //seek to box
            _envelope = new SharpMap.Geometries.BoundingBox(
                GetLittleEndian(_shapeFileReader.ReadDouble()),
                GetLittleEndian(_shapeFileReader.ReadDouble()),
                GetLittleEndian(_shapeFileReader.ReadDouble()),
                GetLittleEndian(_shapeFileReader.ReadDouble()));
        }

        /// <summary>
        /// Parses the .shx shapefile index file
        /// </summary>
        /// <remarks>
        /// The index file is organized to give a matching offset and content length for each entry in the .shp file.
        /// 
        /// From ESRI Shapefile Technical Description document
        /// 
        /// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
        /// 
        /// Byte
        /// Position    Field           Value           Type    Order
        /// ---------------------------------------------------------
        /// Byte 0      Offset          Offset          Integer Big
        /// Byte 4      Content Length  Content Length  Integer Big
        /// 
        /// The Integer type corresponds to the CLS Int32 type.
        /// </remarks>
        private void ParseIndex()
        {
            FileStream indexStream;
            BinaryReader indexReader;

            using (indexStream = new FileStream(IndexFilename, FileMode.Open, FileAccess.Read))
            using (indexReader = new BinaryReader(indexStream, System.Text.Encoding.Unicode))
            {
                indexStream.Seek(HeaderSizeBytes, SeekOrigin.Begin);
                uint recordNumber = 0;
                while (indexStream.Position < indexStream.Length)
                {
                    IndexEntry entry = new IndexEntry();
                    entry.Offset = GetBigEndian(indexReader.ReadInt32());
                    entry.Length = GetBigEndian(indexReader.ReadInt32());
                    _shapeIndex[recordNumber++] = entry;
                }
            }
        }

        /// <summary>
        /// Reads and parses the projection if a projection file exists
        /// </summary>
        private void ParseProjection()
        {
            string projfile = Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename) + ".prj");

            if (System.IO.File.Exists(projfile))
            {
                try
                {
                    string wkt = System.IO.File.ReadAllText(projfile);
                    _coordinateSystem = (ICoordinateSystem)SharpMap.Converters.WellKnownText.CoordinateSystemWktReader.Parse(wkt);
                    _coordsysReadFromFile = true;
                }
                catch (ArgumentException ex)
                {
                    System.Diagnostics.Trace.TraceWarning("Coordinate system file '" + projfile + "' found, but could not be parsed. WKT parser returned:" + ex.Message);
                    throw new InvalidShapeFileException("Invalid .prj file", ex);
                }
            }
        }
        #endregion

        #region File writing helper functions
        private void writeFeatureRow(FeatureDataRow feature)
        {
            uint recordNumber = addIndexEntry(feature);

            if (HasDbf)
            {
                _dbaseWriter.AddRow(feature);
            }

            writeGeometry(feature.Geometry, recordNumber, _shapeIndex[recordNumber].Length);
        }

        private void writeFeatureRow(FeatureDataRow<uint> feature)
        {
            uint recordNumber = feature.Id;

            if (!_shapeIndex.ContainsKey(recordNumber))
            {
                recordNumber = addIndexEntry(feature);

                if (HasDbf)
                {
                    _dbaseWriter.AddRow(feature);
                }
            }
            else if (HasDbf)
            {
                _dbaseWriter.UpdateRow(recordNumber, feature);
            }

            writeGeometry(feature.Geometry, recordNumber, _shapeIndex[recordNumber].Length);
        }

        private uint addIndexEntry(FeatureDataRow feature)
        {
            IndexEntry entry = new IndexEntry();
            entry.Length = computeGeometryLengthInWords(feature.Geometry);
            entry.Offset = computeFileLengthInWords();
            uint id = getNextId();
            _shapeIndex[id] = entry;
            return id;
        }

        private void enableReading()
        {
            if (_shapeFileReader == null || !_shapeFileStream.CanRead)
            {
                if (_shapeFileStream != null)
                {
                    _shapeFileStream.Close();
                }

                if (_exclusiveMode)
                {
                    _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
                else
                {
                    _shapeFileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                }

                _shapeFileReader = new BinaryReader(_shapeFileStream);
            }

            if (HasDbf)
            {
                if (_dbaseReader == null)
                {
                    if (!_exclusiveMode)
                    {
                        if (_dbaseWriter != null)
                        {
                            _dbaseWriter.Close();
                        }

                        _dbaseWriter = null;
                    }

                    _dbaseReader = new DbaseReader(DbfFilename);
                }

                if (!_dbaseReader.IsOpen)
                {
                    _dbaseReader.Open();
                }
            }
        }

        private void enableWriting()
        {
            if (_shapeFileWriter == null || !_shapeFileStream.CanWrite)
            {
                if (_shapeFileStream != null)
                {
                    _shapeFileStream.Close();
                }

                if (_exclusiveMode)
                {
                    _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
                else
                {
                    _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                }

                _shapeFileWriter = new BinaryWriter(_shapeFileStream);
            }

            if (HasDbf)
            {
                if (_dbaseWriter == null)
                {
                    if (!_exclusiveMode)
                    {
                        if (_dbaseReader != null)
                        {
                            _dbaseReader.Close();
                        }

                        _dbaseReader = null;
                    }

                    // Workaround for trying to open the file for exclusive writing too quickly after disposing the reader
                    int numberAttemptsToOpenDbfForWriting = 0;

                    while (numberAttemptsToOpenDbfForWriting < 3)
                    {
                        try
                        {
                            _dbaseWriter = new DbaseWriter(DbfFilename);
                        }
                        catch (System.IO.IOException)
                        {
                            System.Threading.Thread.Sleep(200);
                            numberAttemptsToOpenDbfForWriting++;
                        }
                    }

                    if (_dbaseWriter == null)
                    {
                        throw new ShapefileException("Can't open Dbase file for writing.");
                    }
                }
            }
        }

        private void writeHeader(BinaryWriter writer)
        {
            BoundingBox boundingBox = GetExtents();
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(GetBigEndian(HeaderStartCode));
            writer.Write(new byte[20]);
            writer.Write(GetBigEndian(computeFileLengthInWords()));
            writer.Write(GetLittleEndian(VersionCode));
            writer.Write(GetLittleEndian((int)ShapeType));
            writer.Write(GetLittleEndian(boundingBox.Left));
            writer.Write(GetLittleEndian(boundingBox.Bottom));
            writer.Write(GetLittleEndian(boundingBox.Right));
            writer.Write(GetLittleEndian(boundingBox.Top));
            writer.Write(new byte[32]); // Z-values and M-values
        }

        private void writeIndex()
        {
            IndexEntry[] indexEntries = new IndexEntry[_shapeIndex.Count];

            foreach (KeyValuePair<uint, IndexEntry> kvp in _shapeIndex)
            {
                indexEntries[kvp.Key] = kvp.Value;
            }

            using (FileStream indexStream = File.Open(IndexFilename, FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter indexWriter = new BinaryWriter(indexStream))
            {
                writeHeader(indexWriter);

                foreach (IndexEntry entry in indexEntries)
                {
                    indexWriter.Write(GetBigEndian(entry.Offset));
                    indexWriter.Write(GetBigEndian(entry.Length));
                }

                indexWriter.Flush();
            }
        }

        private void writeGeometry(Geometry g, uint recordNumber, int recordLengthInWords)
        {
            if (g == null)
            {
                throw new NotSupportedException("Writing null shapes not supported in this version");
            }

            _shapeFileStream.Position = _shapeIndex[recordNumber].AbsoluteByteOffset;
            recordNumber += 1; // Record numbers are 1- based in shapefile
            _shapeFileWriter.Write(GetBigEndian(recordNumber));
            _shapeFileWriter.Write(GetBigEndian(recordLengthInWords));

            if (g is Point)
            {
                writePoint(g as Point);
            }
            else if (g is MultiPoint)
            {
                writeMultiPoint(g as MultiPoint);
            }
            else if (g is LineString)
            {
                writeLineString(g as LineString);
            }
            else if (g is MultiLineString)
            {
                writeMultiLineString(g as MultiLineString);
            }
            else if (g is Polygon)
            {
                writePolygon(g as Polygon);
            }
            else
            {
                throw new NotSupportedException(String.Format("Writing geometry type {0} is not supported in the current version", g.GetType()));
            }
        }

        private void writeCoordinate(double x, double y)
        {
            _shapeFileWriter.Write(GetLittleEndian(x));
            _shapeFileWriter.Write(GetLittleEndian(y));
        }

        private void writePoint(Point point)
        {
            _shapeFileWriter.Write(GetLittleEndian((int)ShapeType.Point));
            writeCoordinate(point.X, point.Y);
        }

        private void writeBoundingBox(BoundingBox box)
        {
            _shapeFileWriter.Write(GetLittleEndian(box.Left));
            _shapeFileWriter.Write(GetLittleEndian(box.Bottom));
            _shapeFileWriter.Write(GetLittleEndian(box.Right));
            _shapeFileWriter.Write(GetLittleEndian(box.Top));
        }

        private void writeMultiPoint(MultiPoint multiPoint)
        {
            _shapeFileWriter.Write(GetLittleEndian((int)ShapeType.MultiPoint));
            writeBoundingBox(multiPoint.GetBoundingBox());
            _shapeFileWriter.Write(GetLittleEndian(multiPoint.Points.Count));

            foreach (Point point in multiPoint.Points)
            {
                writeCoordinate(point.X, point.Y);
            }
        }

        private void writePolySegments(BoundingBox bbox, int[] parts, Point[] points)
        {
            writeBoundingBox(bbox);
            _shapeFileWriter.Write(GetLittleEndian(parts.Length));
            _shapeFileWriter.Write(GetLittleEndian(points.Length));

            foreach (int partIndex in parts)
            {
                _shapeFileWriter.Write(GetLittleEndian(partIndex));
            }

            foreach (SharpMap.Geometries.Point point in points)
            {
                writeCoordinate(point.X, point.Y);
            }
        }

        private void writeLineString(LineString lineString)
        {
            _shapeFileWriter.Write(GetLittleEndian((int)ShapeType.PolyLine));
            writePolySegments(lineString.GetBoundingBox(), new int[] { 0 }, lineString.Vertices.ToArray());
        }

        private void writeMultiLineString(MultiLineString multiLineString)
        {
            int[] parts = new int[multiLineString.LineStrings.Count];
            List<Point> allPoints = new List<Point>();

            int currentPartsIndex = 0;
            foreach (LineString line in multiLineString.LineStrings)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange(line.Vertices);
            }

            _shapeFileWriter.Write(GetLittleEndian((int)ShapeType.PolyLine));
            writePolySegments(multiLineString.GetBoundingBox(), parts, allPoints.ToArray());
        }

        private void writePolygon(Polygon polygon)
        {
            int[] parts = new int[polygon.NumInteriorRing + 1];
            List<Point> allPoints = new List<Point>();
            int currentPartsIndex = 0;
            parts[currentPartsIndex++] = 0;
            allPoints.AddRange(polygon.ExteriorRing.Vertices);

            foreach (LinearRing ring in polygon.InteriorRings)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange(ring.Vertices);
            }

            _shapeFileWriter.Write(GetLittleEndian((int)ShapeType.Polygon));
            writePolySegments(polygon.GetBoundingBox(), parts, allPoints.ToArray());
        }
        #endregion

        #region IndexEntry struct
        /// <summary>
        /// Entry for each feature to determine the position and length of the geometry in the .shp file.
        /// </summary>
        private struct IndexEntry
        {
            private int _offset;
            private int _length;

            /// <summary>
            /// Number of 16-bit words taken up by the record
            /// </summary>
            public int Length
            {
                get { return _length; }
                set { _length = value; }
            }

            /// <summary>
            /// Offset of the record in 16-bit words from the beginning of the shapefile
            /// </summary>
            public int Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }

            /// <summary>
            /// Number of bytes in the record
            /// </summary>
            public int ByteLength
            {
                get { return _length * 2; }
            }

            /// <summary>
            /// Record offest in bytes from the beginning of the shapefile
            /// </summary>
            public int AbsoluteByteOffset
            {
                get { return _offset * 2; }
            }

            public override string ToString()
            {
                return String.Format("[IndexEntry] Offset: {0}; Length: {1}; Stream Position: {2}", Offset, Length, AbsoluteByteOffset);
            }
        }
        #endregion

        #region Obsolete

        ///// <summary>
        ///// Returns all objects within a distance of a geometry
        ///// </summary>
        ///// <param name="geom"></param>
        ///// <param name="distance"></param>
        ///// <returns></returns>
        ///// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        //[Obsolete("Use ExecuteIntersectionQuery instead")]
        //public SharpMap.Data.FeatureDataTable<uint> QueryFeatures(SharpMap.Geometries.Geometry geom, double distance)
        //{
        //    checkOpen();
        //    EnableReading();

        //    SharpMap.Data.FeatureDataTable<uint> dt = _dbaseReader.NewTable;
        //    SharpMap.Geometries.BoundingBox bbox = geom.GetBoundingBox();
        //    bbox.Min.X -= distance; bbox.Max.X += distance;
        //    bbox.Min.Y -= distance; bbox.Max.Y += distance;
        //    //Get candidates by intersecting the spatial index tree
        //    List<uint> objectlist = _tree.Search(bbox);

        //    if (objectlist.Count == 0)
        //        return dt;

        //    SharpMap.Geometries.Geometry geomBuffer = geom.Buffer(distance);
        //    for (int j = 0; j < objectlist.Count; j++)
        //    {
        //        for (uint i = (uint)dt.Rows.Count - 1; i >= 0; i--)
        //        {
        //            SharpMap.Data.FeatureDataRow fdr = GetFeature(objectlist[j], dt);
        //            if (fdr != null && fdr.Geometry.Intersects(geomBuffer))
        //                dt.Rows.Add(fdr);
        //        }
        //    }
        //    return dt;
        //}

        ///// <summary>
        ///// Returns all objects whose boundingbox intersects bbox - OBSOLETE: Use ExecuteIntersectionQuery(box) instead
        ///// </summary>
        ///// <param name="bbox"><see cref="BoundingBox"/> which determines the view</param>
        ///// <param name="ds"></param>
        ///// <exception cref="InvalidShapefileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        //[Obsolete("Use ExecuteIntersectionQuery(box) instead")]
        //public void GetFeaturesInView(SharpMap.Geometries.BoundingBox bbox, SharpMap.Data.FeatureDataSet ds)
        //{
        //    checkOpen();
        //    EnableReading();

        //    //Use the spatial index to get a list of features whose boundingbox intersects bbox
        //    List<uint> objectlist = GetObjectIDsInView(bbox);
        //    SharpMap.Data.FeatureDataTable<uint> dt = _dbaseReader.NewTable;

        //    foreach (uint key in objectlist)
        //    {
        //        SharpMap.Data.FeatureDataRow<uint> fdr = _dbaseReader.GetFeature(key, dt);
        //        fdr.Geometry = ReadGeometry(key);
        //        if (fdr.Geometry != null)
        //            if (fdr.Geometry.GetBoundingBox().Intersects(bbox))
        //                if (FilterDelegate == null || FilterDelegate(fdr))
        //                    dt.AddRow(fdr);
        //    }

        //    ds.Tables.Add(dt);
        //}
        #endregion
    }

    /// <summary>
    /// Exception thrown during shapefile operations
    /// </summary>
    public class ShapefileException : Exception
    {
        public ShapefileException() : base() { }
        public ShapefileException(string message) : base(message) { }
        public ShapefileException(string message, Exception inner) : base(message, inner) { }
        public ShapefileException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when the shapefile is invalid or corrupt.
    /// </summary>
    public class InvalidShapeFileException : ShapefileException
    {
        public InvalidShapeFileException() : base() { }
        public InvalidShapeFileException(string message) : base(message) { }
        public InvalidShapeFileException(string message, Exception inner) : base(message, inner) { }
        public InvalidShapeFileException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when an operation is attempted which is not defined for the state of the <see cref="ShapeFile"/>
    /// </summary>
    public class InvalidShapeFileOperationException : ShapefileException
    {
        public InvalidShapeFileOperationException() : base() { }
        public InvalidShapeFileOperationException(string message) : base(message) { }
        public InvalidShapeFileOperationException(string message, Exception inner) : base(message, inner) { }
        public InvalidShapeFileOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when a geometry type exists in a shapefile which is not currently supported.
    /// </summary>
    public class UnsupportedShapefileGeometryException : ShapefileException
    {
        public UnsupportedShapefileGeometryException() : base() { }
        public UnsupportedShapefileGeometryException(string message) : base(message) { }
        public UnsupportedShapefileGeometryException(string message, Exception inner) : base(message, inner) { }
        public UnsupportedShapefileGeometryException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}