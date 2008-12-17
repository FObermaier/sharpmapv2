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
using SharpMap.Expressions;

#endregion

namespace SharpMap.Styles.Symbology.Capabilities
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "Scalar_CapabilitiesType")]
    public class ScalarCapabilities
    {
        private object[] _arithmeticOperators;
        private ComparisonOperatorCapabilities _comparisonOperators;
        private LogicalOperatorsCapabilities _logicalOperators;

        public LogicalOperatorsCapabilities LogicalOperators
        {
            get { return _logicalOperators; }
            set { _logicalOperators = value; }
        }

        public ComparisonOperatorCapabilities ComparisonOperators
        {
            get { return _comparisonOperators; }
            set { _comparisonOperators = value; }
        }

        [XmlArrayItem("Functions", typeof (FunctionsCapabilities), IsNullable = false)]
        [XmlArrayItem("SimpleArithmetic", typeof (SimpleArithmetic), IsNullable = false)]
        public object[] ArithmeticOperators
        {
            get { return _arithmeticOperators; }
            set { _arithmeticOperators = value; }
        }
    }
}