// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

using SharpMap.Styles;

using IMatrix2D = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface to a graphical renderer.
    /// </summary>
    public interface IRenderer : IDisposable
    {
        /// <summary>
        /// Gets or sets a matrix used to transform world coordinates to graphical display coordinates.
        /// </summary>
        IMatrix2D ViewTransform { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="StyleRenderingMode"/> value used to render objects.
        /// </summary>
        StyleRenderingMode StyleRenderingMode { get; set; }
    }
}
