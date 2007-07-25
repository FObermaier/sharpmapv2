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
using NPack;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a 2 dimensional affine transform matrix (a 3x3 matrix).
    /// </summary>
    [Serializable]
    public class Matrix2D : AffineMatrix<DoubleComponent>
    {
        public new readonly static Matrix2D Identity
            = new Matrix2D(
                1, 0, 0,
                0, 1, 0,
                0, 0, 1);

        public new readonly static Matrix2D Zero
            = new Matrix2D(
                0, 0, 0,
                0, 0, 0,
                0, 0, 0);

        #region Constructors
        public Matrix2D()
            : this(Identity) { }

        public Matrix2D(double x1, double x2, double offsetX,
            double y1, double y2, double offsetY)
            : this(x1, x2, offsetX, y1, y2, offsetY, 0, 0, 1)
        {
        }

        public Matrix2D(double x1, double x2, double offsetX,
            double y1, double y2, double offsetY,
            double w1, double w2, double w3)
            :base(MatrixFormat.RowMajor, 3)
        {
            X1 = x1; X2 = x2; OffsetX = offsetX;
            Y1 = y1; Y2 = y2; OffsetY = offsetY;
            W1 = w1; W2 = w2; W3 = w3;
        }

        public Matrix2D(IMatrixD matrixToCopy)
            : base(MatrixFormat.RowMajor, 3)
        {
            if (matrixToCopy == null) throw new ArgumentNullException("matrixToCopy");

            for (int i = 0; i < RowCount; i++)
            {
                Array.Copy(matrixToCopy.Elements, Elements, matrixToCopy.Elements.Length);
            }
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return String.Format("[ViewMatrix2D] [ [{0:N3}, {1:N3}, {2:N3}], [{3:N3}, {4:N3}, {5:N3}], [{6:N3}, {7:N3}, {8:N3}] ]",
                X1, Y1, W1, X2, Y2, W2, OffsetX, OffsetY, W3);
        }
        #endregion

        #region GetHashCode
        public override int GetHashCode()
        {
            return unchecked(this[0, 0].GetHashCode() + 24243 ^ this[0, 1].GetHashCode() + 7318674
                ^ this[0, 2].GetHashCode() + 282 ^ this[1, 0].GetHashCode() + 54645
                ^ this[1, 1].GetHashCode() + 42 ^ this[1, 2].GetHashCode() + 244892
                ^ this[2, 0].GetHashCode() + 8464 ^ this[2, 1].GetHashCode() + 36565 ^ this[2, 2].GetHashCode() + 3210186);
        }
        #endregion

        public new Matrix2D Clone()
        {
            return new Matrix2D(this);
        }

        public new Matrix2D Inverse
        {
            get
            {
                return new Matrix2D(base.Inverse);
            }
		}

        public void Scale(double x, double y)
        {
            base.Scale(new Point2D(x, y));
		}

		public void ScalePrepend(double x, double y)
		{
			base.Scale(new Point2D(x, y), MatrixOperationOrder.Prepend);
		}

        public void Translate(double x, double y)
        {
            base.Translate(new Point2D(x, y));
		}

		public void TranslatePrepend(double x, double y)
		{
			base.Translate(new Point2D(x, y), MatrixOperationOrder.Prepend);
		}

        private readonly DoubleComponent[] _transferPoints = new DoubleComponent[3];

        public Point2D TransformVector(double x, double y)
        {
            _transferPoints[0] = x;
            _transferPoints[1] = y;
            _transferPoints[2] = 1;
            MatrixProcessor<DoubleComponent>.Instance.Operations.Multiply(_transferPoints, this);
            return new Point2D((double)_transferPoints[0], (double)_transferPoints[1]);
        }

        #region Equality Computation

        public override bool Equals(object obj)
        {
            if (obj is Matrix2D)
            {
                return Equals(obj as Matrix2D);
            }

            if (obj is IMatrixD)
            {
                return Equals(obj as IMatrixD);
            }

            return false;
        }

        #region IEquatable<ViewMatrix2D> Members

        public bool Equals(Matrix2D other)
        {
            return X1 == other.X1 &&
                X2 == other.X2 &&
                OffsetX == other.OffsetX && 
                Y1 == other.Y1 &&
                Y2 == other.Y2 &&
                OffsetY == other.OffsetY &&
                W1 == other.W1 &&
                W2 == other.W2 &&
                W3 == other.W3;
        }

        #endregion
        #endregion

        #region Properties
        public double X1
        {
            get { return (double)this[0, 0]; }
            set { this[0, 0] = value; }
        }

        public double X2
        {
            get { return (double)this[1, 0]; }
            set { this[1, 0] = value; }
        }

        public double OffsetX
        {
            get { return (double)this[2, 0]; }
            set { this[2, 0] = value; }
        }

        public double Y1
        {
            get { return (double)this[0, 1]; }
            set { this[0, 1] = value; }
        }

        public double Y2
        {
            get { return (double)this[1, 1]; }
            set { this[1, 1] = value; }
        }

        public double OffsetY
        {
            get { return (double)this[2, 1]; }
            set { this[2, 1] = value; }
        }

        public double W1
        {
            get { return (double)this[0, 2]; }
            set { this[0, 2] = value; }
        }

        public double W2
        {
            get { return (double)this[1, 2]; }
            set { this[1, 2] = value; }
        }

        public double W3
        {
            get { return (double)this[2, 2]; }
            set { this[2, 2] = value; }
        }
        #endregion
    }
}
