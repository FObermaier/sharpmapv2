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

using System;
using System.Xml.Serialization;
using GeoAPI.DataStructures;
using GeoAPI.DataStructures.Collections.Generic;
using SharpMap.Data;
using SharpMap.Expressions;

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "FillType")]
    [XmlRoot("Fill", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Fill
    {
        private GraphicFill _graphicFill;
        private readonly HybridDictionary<String, SvgParameter> _svgParameterMap
            = new HybridDictionary<String, SvgParameter>();

        public GraphicFill GraphicFill
        {
            get { return _graphicFill; }
            set { _graphicFill = value; }
        }

        [XmlElement("SvgParameter")]
        public SvgParameter[] SvgParameter
        {
            get { return Enumerable.ToArray(_svgParameterMap.Values); }
            set
            {
                foreach (SvgParameter parameter in value)
                {
                    _svgParameterMap[parameter.Name] = parameter;
                }
            }
        }

        public StyleColor GetColor(IFeatureDataRecord feature)
        {
            Expression expression = _svgParameterMap["fill"].Expression;
            String result = feature.EvaluateFor<String>();
            return result == null ? StyleColor.Transparent : StyleColor.FromArgbString(result);
        }

        public Single GetOpacity(IFeatureDataRecord feature)
        {
            return feature.Evaluate<Single>(_svgParameterMap["fill-opacity"]);
        }
    }
}