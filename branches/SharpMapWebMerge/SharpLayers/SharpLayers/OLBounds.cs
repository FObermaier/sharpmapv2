﻿/*
 *  The attached / following is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
namespace SharpLayers
{
    public class OLBounds
        : ISerializeToOLJson, IOLClass
    {
        [OLJsonSerialization(SerializedName = "left")]
        public double? Left { get; set; }


        [OLJsonSerialization(SerializedName = "bottom")]
        public double? Bottom { get; set; }

        [OLJsonSerialization(SerializedName = "right")]
        public double? Right { get; set; }

        [OLJsonSerialization(SerializedName = "top")]
        public double? Top { get; set; }

        #region IOLClass Members

        public string JsClass
        {
            get { return "OpenLayers.Bounds"; }
        }

        #endregion
    }
}