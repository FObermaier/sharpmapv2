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
using System.Collections.Generic;
using System.Drawing;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Gdi;
using SharpMap.Presentation;
using SharpMap.Utilities;

namespace SharpMap.Presentation.WinForms
{
    public class MapPresenter : MapPresenter2D
    {
        public MapPresenter(Map map, MapViewControl mapView)
            : base(map, mapView)
        {
            RegisterRenderer<VectorLayer, PositionedRenderObject2D<GdiRenderObject>>(new GdiVectorRenderer());
            IRenderer labelRenderer = new GdiLabelRenderer(new GdiVectorRenderer()) as IRenderer;
            RegisterRenderer<LabelLayer<Label2D>, PositionedRenderObject2D<GdiRenderObject>>(labelRenderer);
        }
    }
}