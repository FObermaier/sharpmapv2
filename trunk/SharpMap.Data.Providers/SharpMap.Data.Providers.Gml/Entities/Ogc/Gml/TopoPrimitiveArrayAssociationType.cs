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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlInclude(typeof (FaceType)), XmlInclude(typeof (NodeType)),
     XmlType(TypeName = "TopoPrimitiveArrayAssociationType", Namespace = "http://www.opengis.net/gml/3.2"),
     XmlInclude(typeof (EdgeType)), XmlInclude(typeof (TopoSolidType))]
    public class TopoPrimitiveArrayAssociationType
    {
        [XmlIgnore] private List<AbstractTopoPrimitive> _abstractTopoPrimitive;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] public bool OwnsSpecified;

        public TopoPrimitiveArrayAssociationType()
        {
            Owns = false;
        }

        [XmlElement(Type = typeof (AbstractTopoPrimitive), ElementName = "AbstractTopoPrimitive", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<AbstractTopoPrimitive> AbstractTopoPrimitive
        {
            get
            {
                if (_abstractTopoPrimitive == null)
                {
                    _abstractTopoPrimitive = new List<AbstractTopoPrimitive>();
                }
                return _abstractTopoPrimitive;
            }
            set { _abstractTopoPrimitive = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractTopoPrimitive.Count; }
        }

        [XmlIgnore]
        public AbstractTopoPrimitive this[int index]
        {
            get { return AbstractTopoPrimitive[index]; }
        }

        [XmlAttribute(AttributeName = "owns", DataType = "boolean")]
        public bool Owns
        {
            get { return _owns; }
            set
            {
                _owns = value;
                OwnsSpecified = true;
            }
        }

        public void Add(AbstractTopoPrimitive obj)
        {
            AbstractTopoPrimitive.Add(obj);
        }

        public void Clear()
        {
            AbstractTopoPrimitive.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractTopoPrimitive.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractTopoPrimitive _c in AbstractTopoPrimitive)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractTopoPrimitive obj)
        {
            return AbstractTopoPrimitive.Remove(obj);
        }

        public AbstractTopoPrimitive Remove(int index)
        {
            AbstractTopoPrimitive obj = AbstractTopoPrimitive[index];
            AbstractTopoPrimitive.Remove(obj);
            return obj;
        }
    }
}