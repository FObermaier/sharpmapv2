using System;
using NUnit.Framework;
using SharpMap.Rendering.Rendering2D;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Size2D

    [TestFixture]
    public class Size2DTests
    {
        [Test]
        public void Size2DEqualityTests()
        {
            Size2D s1 = new Size2D();
            Size2D s2 = Size2D.Empty;
            Size2D s3 = Size2D.Zero;
            Size2D s4 = new Size2D(0, 0);
            Size2D s5 = new Size2D(9, 10);

            Assert.AreEqual(s1, s2);
            Assert.AreNotEqual(s1, s3);
            Assert.AreEqual(s3, s4);
            Assert.AreNotEqual(s1, s5);
            Assert.AreNotEqual(s3, s5);

            IVectorD v1 = s1;
            IVectorD v2 = s2;
            IVectorD v3 = s3;
            IVectorD v4 = s4;
            IVectorD v5 = s5;

            Assert.AreEqual(v1, v2);
            Assert.AreNotEqual(v1, v3);
            Assert.AreEqual(v3, v4);
            Assert.AreNotEqual(v1, v5);
            Assert.AreNotEqual(v3, v5);

            Assert.AreEqual(v5, s5);
        }

        [Test]
        public void IVectorDTests1()
        {
            IVectorD s1 = Point2D.Empty;
            IVectorD s2 = Point2D.Zero;
            IVectorD s3 = new Point2D(9, 10);

            Assert.AreEqual(0, s1.ComponentCount);
            Assert.AreEqual(2, s2.ComponentCount);
            Assert.AreEqual(9, (Double) s3[0], TestConstants.Epsilon);
            Assert.AreEqual(10, (Double) s3[1], TestConstants.Epsilon);
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void IVectorDTests2()
        {
            IVectorD s1 = new Point2D(9, 10);

            Assert.AreEqual(10, (Double) s1[2], TestConstants.Epsilon);
        }

        [Test]
        public void CloneTest()
        {
            Point2D s1 = new Point2D(1.1, 2.2);
            Point2D s2 = s1.Clone();

            Assert.AreEqual(s1, s2);
            Assert.AreNotSame(s1, s2);
        }

        [Test]
        public void IEnumerableTest()
        {
            Point2D s1 = new Point2D(1.1, 2.2);

            Int32 index = 0;
            foreach (Double component in s1)
            {
                Assert.AreEqual(s1[index++], component);
            }
        }
    }

    #endregion
}