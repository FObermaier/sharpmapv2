// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "LineStyleType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (DataType))]
    [XmlInclude(typeof (AbstractTimePrimitiveType))]
    [XmlInclude(typeof (SchemaDataType))]
    [XmlInclude(typeof (ItemIconType))]
    [XmlInclude(typeof (AbstractLatLonBoxType))]
    [XmlInclude(typeof (OrientationType))]
    [XmlInclude(typeof (AbstractStyleSelectorType))]
    [XmlInclude(typeof (ResourceMapType))]
    [XmlInclude(typeof (LocationType))]
    [XmlInclude(typeof (AbstractSubStyleType))]
    [XmlInclude(typeof (RegionType))]
    [XmlInclude(typeof (AliasType))]
    [XmlInclude(typeof (AbstractViewType))]
    [XmlInclude(typeof (AbstractFeatureType))]
    [XmlInclude(typeof (AbstractGeometryType))]
    [XmlInclude(typeof (BasicLinkType))]
    [XmlInclude(typeof (PairType))]
    [XmlInclude(typeof (ImagePyramidType))]
    [XmlInclude(typeof (ScaleType))]
    [XmlInclude(typeof (LodType))]
    [XmlInclude(typeof (ViewVolumeType))]
    public class LineStyleType : AbstractColorStyleType
    {
        [XmlIgnore] private List<LineStyleObjectExtensionGroup> _LineStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> _LineStyleSimpleExtensionGroup;
        [XmlIgnore] private double _width;

        [XmlIgnore] public bool _widthSpecified;

        public LineStyleType()
        {
            width = 1.0;
        }


        [XmlElement(ElementName = "width", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double width
        {
            get { return _width; }
            set
            {
                _width = value;
                _widthSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "LineStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LineStyleSimpleExtensionGroup
        {
            get
            {
                if (_LineStyleSimpleExtensionGroup == null) _LineStyleSimpleExtensionGroup = new List<string>();
                return _LineStyleSimpleExtensionGroup;
            }
            set { _LineStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LineStyleObjectExtensionGroup), ElementName = "LineStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LineStyleObjectExtensionGroup> LineStyleObjectExtensionGroup
        {
            get
            {
                if (_LineStyleObjectExtensionGroup == null)
                    _LineStyleObjectExtensionGroup = new List<LineStyleObjectExtensionGroup>();
                return _LineStyleObjectExtensionGroup;
            }
            set { _LineStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}