// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software � 2008 Newgrove Consultants Limited, 
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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "OperationParameterGroupType", Namespace = Declarations.SchemaVersion)]
    public class OperationParameterGroupType : AbstractGeneralOperationParameterType
    {
        [XmlIgnore] private string _maximumOccurs;
        [XmlIgnore] private List<ParameterProperty> _parameter;

        [XmlElement(ElementName = "maximumOccurs", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "positiveInteger", Namespace = Declarations.SchemaVersion)]
        public string MaximumOccurs
        {
            get { return _maximumOccurs; }
            set { _maximumOccurs = value; }
        }

        [XmlElement(Type = typeof (ParameterProperty), ElementName = "parameter", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ParameterProperty> Parameter
        {
            get
            {
                if (_parameter == null)
                {
                    _parameter = new List<ParameterProperty>();
                }
                return _parameter;
            }
            set { _parameter = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (ParameterProperty _c in Parameter)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}