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

using System.Collections.Generic;
using SharpMap.Data;
using SharpMap.Styles;
using SharpMap.Layers;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface to a graphical renderer of feature data yielding 
    /// strongly-typed render objects.
    /// </summary>
    /// <typeparam name="TRenderObject">
    /// Type of object used by the graphics library to perform drawing.
    /// </typeparam>
    public interface IFeatureRenderer<TRenderObject> : IFeatureRenderer
	{
		/// <summary>
		/// Renders the attributes and/or spatial data in the <paramref name="feature"/>.
		/// </summary>
		/// <param name="feature">
		/// A <see cref="IFeatureDataRecord"/> instance with spatial data.
		/// </param>
		/// <returns>
		/// An enumeration of <typeparamref name="TRenderObject"/> instances 
		/// used to draw the spatial data.
		/// </returns>
		new IEnumerable<TRenderObject> RenderFeature(IFeatureDataRecord feature);

        /// <summary>
        /// Renders the attributes and/or spatial data in the <paramref name="feature"/>.
        /// </summary>
        /// <param name="feature">
        /// A <see cref="IFeatureDataRecord"/> instance with spatial data.
        /// </param>
        /// <param name="style">
        /// Style used to render the feature, overriding theme. 
        /// Use null if no style is desired or to use <see cref="IFeatureRenderer.Theme"/>.
        /// </param>
        /// <returns>
        /// An enumeration of <typeparamref name="TRenderObject"/> instances 
        /// used to draw the spatial data.
        /// </returns>
        new IEnumerable<TRenderObject> RenderFeature(IFeatureDataRecord feature, IStyle style, RenderState renderState, ILayer layer);
    }
}