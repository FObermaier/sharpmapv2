using System.Collections.Generic;
using System.IO;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.IO.WellKnownText;
using GisSharpBlog.NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using Coordinate2D = NetTopologySuite.Coordinates.BufferedCoordinate2D;
using Coordinate2DFactory = NetTopologySuite.Coordinates.BufferedCoordinate2DFactory;
using Coordinate2DSequenceFactory = NetTopologySuite.Coordinates.BufferedCoordinate2DSequenceFactory;

namespace ProjNet.UnitTests
{
    internal class SRIDReader
    {
        private const string filename = @"..\..\SRID.csv";

        public struct WKTstring {
            /// <summary>
            /// Well-known ID
            /// </summary>
            public int WKID;
            /// <summary>
            /// Well-known Text
            /// </summary>
            public string WKT;
        }

        /// <summary>
        /// Enumerates all SRID's in the SRID.csv file.
        /// </summary>
        /// <returns>Enumerator</returns>
        public static IEnumerable<WKTstring> GetSRIDs()
        {
            using (StreamReader sr = File.OpenText(filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int split = line.IndexOf(';');
                    if (split > -1)
                    {
                        WKTstring wkt = new WKTstring();
                        wkt.WKID = int.Parse(line.Substring(0, split));
                        wkt.WKT = line.Substring(split + 1);
                        yield return wkt;
                    }
                }
                sr.Close();
            }
        }
        /// <summary>
        /// Gets a coordinate system from the SRID.csv file
        /// </summary>
        /// <param name="id">EPSG ID</param>
        /// <returns>Coordinate system, or null if SRID was not found.</returns>
		public static ICoordinateSystem<Coordinate2D> GetCSbyID(int id)
        {
			ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
			IGeometryFactory<Coordinate2D> gf = new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
			CoordinateSystemFactory<Coordinate2D> fac = new CoordinateSystemFactory<Coordinate2D>(cf, gf);
			
			foreach (WKTstring wkt in GetSRIDs())
            {
                if (wkt.WKID == id)
                {
					return WktReader<Coordinate2D>.ToCoordinateSystemInfo(wkt.WKT, fac) as ICoordinateSystem<Coordinate2D>;
                }
            }
            return null;
        }
    }
}
