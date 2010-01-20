﻿// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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

#region Namespace imports

using System;
using System.Xml.Serialization;

#endregion

namespace SharpMap.Styles.Symbology.Capabilities
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot(ElementName = "Filter_Capabilities", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class FilterCapabilities
    {
        private Object[] _idCapabilities;
        private ScalarCapabilities _scalarCapabilities;

        public ScalarCapabilities ScalarCapabilities
        {
            get { return _scalarCapabilities; }
            set { _scalarCapabilities = value; }
        }

        [XmlArrayItem("EID", typeof (Eid), IsNullable = false)]
        [XmlArrayItem("FID", typeof (Fid), IsNullable = false)]
        public Object[] IdCapabilities
        {
            get { return _idCapabilities; }
            set { _idCapabilities = value; }
        }
    }
}