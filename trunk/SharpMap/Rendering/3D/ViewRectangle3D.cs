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

using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering3D
{
    [Serializable]
    public struct ViewRectangle3D : IMatrixD, IComparable<ViewRectangle3D>
    {
        private double _xMin;
        private double _yMin;
        private double _xMax;
        private double _yMax;
        private double _zMin;
        private double _zMax;
        private bool _hasValue;

        public static readonly ViewRectangle3D Empty = new ViewRectangle3D();
        public static readonly ViewRectangle3D Zero = new ViewRectangle3D(0, 0, 0, 0, 0, 0);

        public ViewRectangle3D(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
        {
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
            _zMin = zMin;
            _zMax = zMax;
            _hasValue = true;
        }

        public ViewRectangle3D(ViewPoint3D location, ViewSize3D size)
        {
            _xMin = location.X;
            _yMin = location.Y;
            _zMin = location.Z;
            _xMax = _xMin + size.Width;
            _yMax = _yMin + size.Height;
            _zMax = _zMin + size.Depth;
            _hasValue = true;
        }

        public double X
        {
            get { return _xMin; }
        }

        public double Y
        {
            get { return _yMin; }
        }

        public double Z
        {
            get { return _zMin; }
        }

        public double Left
        {
            get { return _xMin; }
        }

        public double Top
        {
            get { return _yMin; }
        }

        public double Right
        {
            get { return _xMax; }
        }

        public double Bottom
        {
            get { return _yMax; }
        }

        public double Back
        {
            get { return _zMax; }
        }

        public double Front
        {
            get { return _zMin; }
        }

        public ViewPoint3D Center
        {
            get { return new ViewPoint3D(X + Width / 2, Y + Height / 2, Z + Depth / 2); }
        }

        public ViewPoint3D Location
        {
            get { return new ViewPoint3D(_xMin, _yMin, _zMin); }
        }

        public ViewPoint3D Size
        {
            get { return new ViewPoint3D(Width, Height, Depth); }
        }

        public double Width
        {
            get { return Math.Abs(_xMax - _xMin); }
        }

        public double Height
        {
            get { return Math.Abs(_yMax - _yMin); }
        }

        public double Depth
        {
            get { return Math.Abs(_zMax - _zMin); }
        }

        public static bool operator ==(ViewRectangle3D rect1, ViewRectangle3D rect2)
        {
            return rect1.Left == rect2.Left &&
                rect1.Right == rect2.Right &&
                rect1.Top == rect2.Top &&
                rect1.Bottom == rect2.Bottom &&
                rect1.Back == rect2.Back &&
                rect1.Front == rect2.Front;
        }

        public static bool operator !=(ViewRectangle3D rect1, ViewRectangle3D rect2)
        {
            return !(rect1 == rect2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ViewRectangle3D))
            {
                return false;
            }

            ViewRectangle3D other = (ViewRectangle3D)obj;

            return other == this;
        }

        public override int GetHashCode()
        {
            return unchecked((int)Left ^ (int)Right ^ (int)Top ^ (int)Bottom ^ (int)Back ^ (int)Front);
        }

        public override string ToString()
        {
            return String.Format("ViewRectangle - Left: {0:N3}; Top: {1:N3}; Right: {2:N3}; Bottom: {3:N3}; Back: {4:N3}; Front: {5:N3}", 
                Left, Top, Right, Top, Bottom, Back, Front);
        }

        /// <summary>
        /// Determines whether this <see cref="Rectangle"/> intersects another.
        /// </summary>
        /// <param name="rectangle"><see cref="Rectangle"/> to check intersection with.</param>
        /// <returns>True if there is intersection, false if not.</returns>
        public bool Intersects(ViewRectangle3D rectangle)
        {
            return !(rectangle.Left > Right ||
                     rectangle.Right < Left ||
                     rectangle.Bottom > Top ||
                     rectangle.Top < Bottom ||
                     rectangle.Front > Back ||
                     rectangle.Back < Front);
        }

        #region IComparable<Rectangle> Members

        /// <summary>
        /// Compares this <see cref="Rectangle"/> instance with another instance.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="other">Rectangle to perform intersection test with.</param>
        /// <returns>
        /// Returns 0 if the <see cref="Rectangle"/> instances intersect each other,
        /// 1 if this Rectangle is located to the right or down from the <paramref name="other"/>
        /// Rectange, and -1 if this Rectangle is located to the left or up from the other.
        /// </returns>
        public int CompareTo(ViewRectangle3D other)
        {
            if (this.Intersects(other))
                return 0;

            if (other.Left > Right || other.Top > Bottom || other.Front > Back)
                return -1;

            return 1;
        }

        #endregion

        #region IViewMatrix Members

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Invert()
        {
            throw new NotSupportedException();
        }

        public bool IsInvertible
        {
            get { return false; }
        }

        public bool IsEmpty
        {
            get { return _hasValue; }
        }

        public double[,] Elements
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void Rotate(double degreesTheta)
        {
            throw new NotSupportedException();
        }

        public void RotateAt(double degreesTheta, IVectorD center)
        {
            throw new NotSupportedException();
        }

        public double GetOffset(int dimension)
        {
            throw new NotSupportedException();
        }

        public void Offset(IVectorD offsetVector)
        {
            Translate(offsetVector);
        }

        public void Multiply(IMatrixD matrix)
        {
            throw new NotSupportedException();
        }

        public void Scale(double scaleAmount)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Scale(IVectorD scaleVector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(double translationAmount)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(IVectorD translationVector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IVectorD Transform(IVectorD vector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public double[] Transform(params double[] vector)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEquatable<IViewMatrix> Members

        public bool Equals(IMatrixD other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}