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
using System.ComponentModel;

using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

using IMatrix2D = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// The base class for 2D feature renderers.
    /// </summary>
    /// <typeparam name="TStyle">The type of style to use.</typeparam>
    /// <typeparam name="TRenderObject">The type of render object produced.</typeparam>
    public abstract class FeatureRenderer2D<TStyle, TRenderObject> 
        : IFeatureRenderer<PositionedRenderObject2D<TRenderObject>>
        where TStyle : class, IStyle
    {
        private Matrix2D _viewMatrix;
        private StyleRenderingMode _renderMode;
        private VectorRenderer2D<TRenderObject> _vectorRenderer;
        private ITheme _theme;
        private bool _disposed;

        #region Object Construction/Destruction
        protected FeatureRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer)
            : this(vectorRenderer, null)
        {
        }

        protected FeatureRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer, ITheme theme)
        {
            _vectorRenderer = vectorRenderer;
            _theme = theme;
        }

        ~FeatureRenderer2D()
        {
            Dispose(false);
        }

        #region Dispose Pattern
        #region IDisposable Members

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
        }

        protected bool Disposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }
        #endregion
        #endregion

        public VectorRenderer2D<TRenderObject> VectorRenderer
        {
            get { return _vectorRenderer; }
        }

        #region Events
		/// <summary>
		/// Event fired when a feature is about to render to the render stream.
		/// </summary>
		public event CancelEventHandler FeatureRendering;

        /// <summary>
        /// Event fired when a feature has been rendered.
        /// </summary>
        public event EventHandler FeatureRendered;
        #endregion

        #region IFeatureRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,PositionedRenderObject2D<TRenderObject>> Members
        /// <summary>
        /// Renders a feature into displayable render objects.
        /// </summary>
        /// <param name="feature">The feature to render.</param>
        /// <returns>An enumeration of positioned render objects for display.</returns>
        public IEnumerable<PositionedRenderObject2D<TRenderObject>> RenderFeature(FeatureDataRow feature)
        {
            return RenderFeature(feature, Theme == null ? null : Theme.GetStyle(feature) as TStyle);
        }

        /// <summary>
        /// Renders a feature into displayable render objects.
        /// </summary>
        /// <param name="feature">The feature to render.</param>
        /// <param name="style">The style to use to render the feature.</param>
        /// <returns>An enumeration of positioned render objects for display.</returns>
		public IEnumerable<PositionedRenderObject2D<TRenderObject>> RenderFeature(
            FeatureDataRow feature, TStyle style)
		{
			bool cancel = false;

			OnFeatureRendering(ref cancel);

			if (cancel)
			{
				yield break;
			}

			IEnumerable<PositionedRenderObject2D<TRenderObject>> renderedObjects 
                = DoRenderFeature(feature, style);

            OnFeatureRendered();

			foreach (PositionedRenderObject2D<TRenderObject> renderObject in renderedObjects)
			{
				yield return renderObject;
			}
        }

        /// <summary>
        /// Gets or sets the theme used to generate styles for rendered features.
        /// </summary>
		public ITheme Theme
		{
            get { return _theme; }
            set { _theme = value; }
		}

        /// <summary>
        /// Render whether smoothing (antialiasing) is applied to lines 
        /// and curves and the edges of filled areas.
        /// </summary>
        public StyleRenderingMode StyleRenderingMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        /// <summary>
        /// Gets or sets a matrix used to transform world 
        /// coordinates to graphical display coordinates.
        /// </summary>
        public Matrix2D ViewTransform
        {
            get { return _viewMatrix; }
            set { _viewMatrix = value; }
        }

        /// <summary>
        /// Template method to perform the actual geometry rendering.
        /// </summary>
        /// <param name="feature">Feature to render.</param>
        /// <param name="style">Style to use in rendering geometry.</param>
        /// <returns></returns>
		protected abstract IEnumerable<PositionedRenderObject2D<TRenderObject>> DoRenderFeature(
            FeatureDataRow feature, TStyle style);
        #endregion

        #region Private helper methods
        /// <summary>
        /// Called when a feature is rendered.
        /// </summary>
        private void OnFeatureRendered()
        {
            EventHandler @event = FeatureRendered;
            
            if (@event != null)
            {
                @event(this, EventArgs.Empty); //Fire event
            }
        }

        /// <summary>
        /// Called when a feature is rendered.
        /// </summary>
        private void OnFeatureRendering(ref bool cancel)
        {
            CancelEventHandler @event = FeatureRendering;

            if (@event != null)
            {
                CancelEventArgs args = new CancelEventArgs(cancel);
                @event(this, args); //Fire event

                cancel = args.Cancel;
            }
        }
        #endregion

        #region Explicit Interface Implementation
        #region IRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,PositionedRenderObject2D<TRenderObject>> Members

        IMatrix2D IRenderer.RenderTransform
        {
            get
            {
                return ViewTransform;
            }
            set
            {
                if (!(value is Matrix2D))
                {
                    throw new NotSupportedException("Only a ViewMatrix2D is supported on a FeatureRenderer2D.");
                }

                ViewTransform = value as Matrix2D;
            }
        }

        #endregion

        #region IFeatureRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,PositionedRenderObject2D<TRenderObject>> Members

        IEnumerable<PositionedRenderObject2D<TRenderObject>> 
            IFeatureRenderer<PositionedRenderObject2D<TRenderObject>>.RenderFeature(
            FeatureDataRow feature, IStyle style)
        {
            return RenderFeature(feature, style as TStyle);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
        #endregion
    }
}
