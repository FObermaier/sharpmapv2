// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of Proj.Net.
// Proj.Net is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Proj.Net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Proj.Net; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Globalization;
using System.Text;
using GeoAPI.CoordinateSystems;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// A meridian used to take longitude measurements from.
    /// </summary>
    public class PrimeMeridian : Info, IPrimeMeridian
    {
        private readonly Double _longitude;
        private readonly IAngularUnit _angularUnit;

        /// <summary>
        /// Initializes a new instance of a prime meridian
        /// </summary>
        /// <param name="longitude">Longitude of prime meridian</param>
        /// <param name="angularUnit">Angular unit</param>
        /// <param name="name">Name</param>
        /// <param name="authority">Authority name</param>
        /// <param name="authorityCode">Authority-specific identification code.</param>
        /// <param name="alias">Alias</param>
        /// <param name="abbreviation">Abbreviation</param>
        /// <param name="remarks">Provider-supplied remarks</param>
        internal PrimeMeridian(Double longitude, IAngularUnit angularUnit, String name, String authority,
                               String authorityCode, String alias, String abbreviation, String remarks)
            :
                base(name, authority, authorityCode, alias, abbreviation, remarks)
        {
            _longitude = longitude;
            _angularUnit = angularUnit;
        }

        #region Predefined prime meridans

        /// <summary>
        /// Greenwich prime meridian
        /// </summary>
        public static PrimeMeridian Greenwich
        {
            get
            {
                return
                    new PrimeMeridian(0.0, CoordinateSystems.AngularUnit.Degrees, "Greenwich", "EPSG", "8901",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Lisbon prime meridian
        /// </summary>
        public static PrimeMeridian Lisbon
        {
            get
            {
                return
                    new PrimeMeridian(-9.0754862, CoordinateSystems.AngularUnit.Degrees, "Lisbon", "EPSG", "8902",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Paris prime meridian.
        /// Value adopted by IGN (Paris) in 1936. Equivalent to 2 deg 20min 14.025sec. Preferred by EPSG to earlier value of 2deg 20min 13.95sec (2.596898 grads) used by RGS London.
        /// </summary>
        public static PrimeMeridian Paris
        {
            get
            {
                return
                    new PrimeMeridian(2.5969213, CoordinateSystems.AngularUnit.Degrees, "Paris", "EPSG", "8903",
                                      String.Empty, String.Empty,
                                      "Value adopted by IGN (Paris) in 1936. Equivalent to 2 deg 20min 14.025sec. Preferred by EPSG to earlier value of 2deg 20min 13.95sec (2.596898 grads) used by RGS London.");
            }
        }

        /// <summary>
        /// Bogota prime meridian
        /// </summary>
        public static PrimeMeridian Bogota
        {
            get
            {
                return
                    new PrimeMeridian(-74.04513, CoordinateSystems.AngularUnit.Degrees, "Bogota", "EPSG", "8904",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Madrid prime meridian
        /// </summary>
        public static PrimeMeridian Madrid
        {
            get
            {
                return
                    new PrimeMeridian(-3.411658, CoordinateSystems.AngularUnit.Degrees, "Madrid", "EPSG", "8905",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Rome prime meridian
        /// </summary>
        public static PrimeMeridian Rome
        {
            get
            {
                return
                    new PrimeMeridian(12.27084, CoordinateSystems.AngularUnit.Degrees, "Rome", "EPSG", "8906",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Bern prime meridian.
        /// 1895 value. Newer value of 7 deg 26 min 22.335 sec E determined in 1938.
        /// </summary>
        public static PrimeMeridian Bern
        {
            get
            {
                return
                    new PrimeMeridian(7.26225, CoordinateSystems.AngularUnit.Degrees, "Bern", "EPSG", "8907", String.Empty,
                                      String.Empty,
                                      "1895 value. Newer value of 7 deg 26 min 22.335 sec E determined in 1938.");
            }
        }

        /// <summary>
        /// Jakarta prime meridian
        /// </summary>
        public static PrimeMeridian Jakarta
        {
            get
            {
                return
                    new PrimeMeridian(106.482779, CoordinateSystems.AngularUnit.Degrees, "Jakarta", "EPSG", "8908",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Ferro prime meridian.
        /// Used in Austria and former Czechoslovakia.
        /// </summary>
        public static PrimeMeridian Ferro
        {
            get
            {
                return
                    new PrimeMeridian(-17.4, CoordinateSystems.AngularUnit.Degrees, "Ferro", "EPSG", "8909", String.Empty,
                                      String.Empty, "Used in Austria and former Czechoslovakia.");
            }
        }

        /// <summary>
        /// Brussels prime meridian
        /// </summary>
        public static PrimeMeridian Brussels
        {
            get
            {
                return
                    new PrimeMeridian(4.220471, CoordinateSystems.AngularUnit.Degrees, "Brussels", "EPSG", "8910",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Stockholm prime meridian
        /// </summary>
        public static PrimeMeridian Stockholm
        {
            get
            {
                return
                    new PrimeMeridian(18.03298, CoordinateSystems.AngularUnit.Degrees, "Stockholm", "EPSG", "8911",
                                      String.Empty, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Athens prime meridian.
        /// Used in Greece for older mapping based on Hatt projection.
        /// </summary>
        public static PrimeMeridian Athens
        {
            get
            {
                return
                    new PrimeMeridian(23.4258815, CoordinateSystems.AngularUnit.Degrees, "Athens", "EPSG", "8912",
                                      String.Empty, String.Empty,
                                      "Used in Greece for older mapping based on Hatt projection.");
            }
        }

        /// <summary>
        /// Oslo prime meridian.
        /// Formerly known as Kristiania or Christiania.
        /// </summary>
        public static PrimeMeridian Oslo
        {
            get
            {
                return
                    new PrimeMeridian(10.43225, CoordinateSystems.AngularUnit.Degrees, "Oslo", "EPSG", "8913",
                                      String.Empty, String.Empty, "Formerly known as Kristiania or Christiania.");
            }
        }

        #endregion

        #region IPrimeMeridian Members

        /// <summary>
        /// Gets or sets the longitude of the prime meridian (relative to the Greenwich prime meridian).
        /// </summary>
        public Double Longitude
        {
            get { return _longitude; }
        }

        /// <summary>
        /// Gets or sets the AngularUnits.
        /// </summary>
        public IAngularUnit AngularUnit
        {
            get { return _angularUnit; }
        }

        /// <summary>
        /// Returns the Well-Known Text for this object
        /// as defined in the simple features specification.
        /// </summary>
        public override String Wkt
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "PRIMEM[\"{0}\", {1}", Name, Longitude);
                if (!String.IsNullOrEmpty(Authority) && !String.IsNullOrEmpty(AuthorityCode))
                {
                    sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
                }
                sb.Append("]");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets an XML representation of this object
        /// </summary>
        public override String Xml
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture.NumberFormat,
                                     "<CS_PrimeMeridian Longitude=\"{0}\" >{1}{2}</CS_PrimeMeridian>", Longitude,
                                     InfoXml, AngularUnit.Xml);
            }
        }

        public override Boolean EqualParams(IInfo other)
        {
            PrimeMeridian p = other as PrimeMeridian;

            if (ReferenceEquals(p, null))
            {
                return false;
            }

            return p.AngularUnit.EqualParams(AngularUnit)
                   && p.Longitude == Longitude;
        }

        #endregion
    }
}