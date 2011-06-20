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
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "BuildingNameType", Namespace = Declarations.SchemaVersion), Serializable]
    public class BuildingNameType
    {
        [XmlIgnore] private string _code;
        [XmlIgnore] private string _type;

        [XmlIgnore] private Occurrence _typeOccurrence;

        [XmlIgnore] public bool _typeOccurrenceSpecified;
        [XmlIgnore] private string _value;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "TypeOccurrence")]
        public Occurrence TypeOccurrence
        {
            get { return _typeOccurrence; }
            set
            {
                _typeOccurrence = value;
                _typeOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}