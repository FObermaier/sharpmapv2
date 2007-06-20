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
using System.IO;
using System.Text;

using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a 2 dimensional graphic used for point data on a map.
    /// </summary>
    public sealed class Symbol2D : ICloneable, IDisposable
    {
        private ColorMatrix _colorTransform = ColorMatrix.Identity;
        private ViewMatrix2D _affineTransform = ViewMatrix2D.Identity;
        private Stream _symbolData;
        private ViewRectangle2D _symbolBox;
        private string _symbolDataHash;
        private bool _disposed;

        #region Object Construction/Destruction
        public Symbol2D(ViewSize2D size)
        {
            _symbolData = new MemoryStream(new byte[] { 0x0, 0x0, 0x0, 0x0 });
        }

        public Symbol2D(Stream symbolData, ViewSize2D size)
        {
            if (!symbolData.CanSeek)
            {
                if (symbolData.Position != 0)
                {
                    throw new InvalidOperationException("Symbol data stream isn't at the beginning, and it can't be repositioned");
                }

                MemoryStream copy = new MemoryStream();

                using (BinaryReader reader = new BinaryReader(symbolData))
                {
                    copy.Write(reader.ReadBytes((int)symbolData.Length), 0, (int)symbolData.Length);
                }

                symbolData = copy;
            }

            _symbolData = symbolData;
            _symbolDataHash = Hashing.Hash(_symbolData);
        }

        public Symbol2D(byte[] symbolData, ViewSize2D size)
            : this(new MemoryStream(symbolData), size) { }

        ~Symbol2D()
        {
            dispose(false);
        }

        #region Dispose Pattern
        #region IDisposable Members
        public void Dispose()
        {
            if (!_disposed)
            {
                dispose(true);
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        private void dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_symbolData != null)
                {
                    _symbolData.Dispose();
                }
            }
        }
        #endregion
        #endregion

        public ViewSize2D Size
        {
            get { return _symbolBox.Size; }
            set { _symbolBox = new ViewRectangle2D(new ViewPoint2D(0, 0), value); }
        }

        /// <summary>
        /// Gets a stream containing the <see cref="Symbol2D"/> data.
        /// </summary>
        /// <remarks>
        /// This is often a bitmap or a vector-based image.
        /// </remarks>
        public Stream SymbolData
        {
            get { return _symbolData; }
            private set { _symbolData = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="ViewMatrix2D"/> object used to transform this <see cref="Symbol2D"/>.
        /// </summary>
        public ViewMatrix2D AffineTransform
        {
            get { return _affineTransform; }
            set { _affineTransform = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="ColorMatrix"/> used to change the color of this symbol.
        /// </summary>
        public ColorMatrix ColorTransform
        {
            get { return _colorTransform; }
            set { _colorTransform = value; }
        }

        /// <summary>
        /// Gets or sets a vector by which to offset the symbol.
        /// </summary>
        public ViewPoint2D Offset
        {
            get { return new ViewPoint2D(_affineTransform.W1, _affineTransform.W2); }
            set { _affineTransform.W1 = value.X; _affineTransform.W2 = value.Y; }
        }

        // TODO: do the math to get the rotation out of the matrix
        /// <summary>
        /// Gets or sets a value by which to rotate this symbol.
        /// </summary>
        public double Rotation
        {
            get { return 0.0; }
            set { }
        }

        // TODO: do the math to get the scale out of the matrix
        /// <summary>
        /// Gets or sets a value by which to scale this symbol.
        /// </summary>
        public double Scale
        {
            get { return 0.0; }
            set { }
        }

        /// <summary>
        /// Returns a string description of this symbol.
        /// </summary>
        /// <returns>A string representing the value of this <see cref="Symbol2D"/>.</returns>
        public override string ToString()
        {
            return String.Format("[{0}] Size: {1}; Data Hash: {2}; Affine Transform: {3}; Color Transform: {4}; Offset: {5}; Rotation: {6:N}; Scale: {7:N}",
                GetType(), Size, _symbolDataHash, AffineTransform, ColorTransform, Offset, Rotation, Scale);
        }

        #region ICloneable Members

        /// <summary>
        /// Clones this symbol.
        /// </summary>
        /// <returns>A duplicate of this <see cref="Symbol2D"/>.</returns>
        public Symbol2D Clone()
        {
            Symbol2D clone = new Symbol2D(_symbolBox.Size);
            
            MemoryStream copy = new MemoryStream();
            long streamPos = _symbolData.Position;
            _symbolData.Seek(0, SeekOrigin.Begin);
            using(BinaryReader reader = new BinaryReader(_symbolData))
                copy.Write(reader.ReadBytes((int)_symbolData.Length), 0, (int)_symbolData.Length);
            _symbolData.Position = streamPos;

            clone.SymbolData = copy;
            clone._symbolDataHash = _symbolDataHash;
            clone.ColorTransform = this.ColorTransform.Clone();
            clone.AffineTransform = this.AffineTransform.Clone();
            return clone;
        }

        /// <summary>
        /// Clones this symbol.
        /// </summary>
        /// <returns>A duplicate of this <see cref="Symbol2D"/> as an object reference.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion
    }
}
