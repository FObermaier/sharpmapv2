﻿// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

using SharpMap.Layers;
using SharpMap.Rendering;

namespace SharpMap.Styles
{
    /// <summary>
    /// Represents a style for a <see cref="RasterLayer"/>.
    /// </summary>
    public class RasterStyle : Style
    {
        private ColorMatrix _colorTransform;

        /// <summary>
        /// Gets or sets a color transform matrix used to recolor the 
        /// raster data.
        /// </summary>
        public ColorMatrix ColorTransform 
        {
            get { return _colorTransform; }
            set 
            {
                if (_colorTransform == value)
                {
                    return;
                }

                if(_colorTransform == null)
                {
                    _colorTransform = ColorMatrix.Identity;
                }

                _colorTransform = value; 
            }
        }
    }
}
