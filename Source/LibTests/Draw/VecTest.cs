namespace LibTests;

[TestClass]
public class VecTest
{
    [TestMethod]
    public void Construction()
    {
        Assert.AreEqual(Vec.ZERO, new Vec());
        Assert.AreEqual(Vec.ZERO, new Vec(0, 0));
        
        Assert.AreEqual(Vec.NORTH, new Vec(0, 1));
        Assert.AreEqual(Vec.EAST, new Vec(1, 0));
        Assert.AreEqual(Vec.SOUTH, new Vec(0, -1));
        Assert.AreEqual(Vec.WEST, new Vec(-1, 0));

        var v = new Vec(1.1, 2.2);

        Assert.AreNotEqual(Vec.ZERO, v);
    }

    [TestMethod]
    public void Deconstruct()
    {
        var (x, y) = new Vec(1.1, 2.2);
        Assert.AreEqual(1.1, x);
        Assert.AreEqual(2.2, y);
    }

    [TestMethod]
    public void Clone()
    {
        var v = new Vec(1.1, 2.2);
        var clone = v with { };
        Assert.AreEqual(v, clone);
        Assert.AreNotSame(v, clone);
    }

    [TestMethod]
    public void Add()
    {
        var v1 = new Vec(1.1, 2.2);
        var v2 = new Vec(3.3, 4.4);
        var add = v1 + v2;
        Assert.IsTrue(add.IsClose(new Vec(4.4, 6.6)), add.ToString());
    }

    [TestMethod]
    public void Subtract()
    {
        var v1 = new Vec(1, 2);
        var v2 = -new Vec(-3, -4);
        var sub = v2 - v1;
        Assert.IsTrue(sub.IsClose(new Vec(2, 2)), sub.ToString());
    }

    [TestMethod]
    public void Multiply()
    {
        var v = new Vec(1, 2);
        var mul1 = 2 * v;
        Assert.IsTrue(mul1.IsClose(new Vec(2, 4)), mul1.ToString());

        var mul2 = v * 2;
        Assert.IsTrue(mul2.IsClose(new Vec(2, 4)), mul2.ToString());

        var mul3 = v * 2.5;
        Assert.IsTrue(mul3.IsClose(new Vec(2.5, 5)), mul3.ToString());

        var mul4 = 2.5 * v;
        Assert.IsTrue(mul4.IsClose(new Vec(2.5, 5)), mul4.ToString());
    }

    [TestMethod]
    public void Divide()
    {
        var v = new Vec(1, 2);
        var div1 = v / 2;
        Assert.IsTrue(div1.IsClose(new Vec(0.5, 1)), div1.ToString());
    }

    [TestMethod]
    public void Equality()
    {
        var v1 = new Vec(1, 2);
        var v2 = new Vec(1, 2);
        var v3 = new Vec(3, 4);

        Assert.IsTrue(v1 == v2);
        Assert.IsFalse(v1 == v3);

        Assert.IsFalse(v1 != v2);
        Assert.IsTrue(v1 != v3);

        Assert.IsTrue(v1.Equals(v2));
        Assert.IsFalse(v1.Equals(v3));
        Assert.IsFalse(v1.Equals("v1"));
    }

    [TestMethod]
    public void String()
    {
        var v = new Vec(1, 2);
        var str = v.ToString();
        Assert.AreEqual("<1, 2>", str);
    }

    [TestMethod]
    public void MagnitudeSq()
    {
        var v = new Vec(3, 4);
        var magSq = v.MagnitudeSq;
        Assert.AreEqual(25, magSq);
    }

    [TestMethod]
    public void Magnitude()
    {
        var v = new Vec(3, 4);
        var mag = v.Magnitude;
        Assert.AreEqual(5, mag);
    }

    [TestMethod]
    public void Manhattan()
    {
        var v = new Vec(3, 4);
        var manhattan = v.Manhattan;
        Assert.AreEqual(7, manhattan);
    }

    [TestMethod]
    public void CrossProduct()
    {
        var v1 = new Vec(1, 2);
        var v2 = new Vec(3, 4);
        var cross = v1.CrossProduct(v2);
        Assert.AreEqual(-2, cross);
    }

    [TestMethod]
    public void DotProduct()
    {
        var v1 = new Vec(1, 2);
        var v2 = new Vec(3, 4);
        var dot = v1.DotProduct(v2);
        Assert.AreEqual(11, dot);
    }

    [TestMethod]
    public void Normalized()
    {
        var v = new Vec(3, 4);
        var norm = v.Normalized();
        Assert.IsTrue(norm.IsClose(new Vec(0.6, 0.8)), norm.ToString());
    }

    [TestMethod]
    public void RotateCCW()
    {
        var v = new Vec(1, 2);
        var rotated = v.RotateCCW();
        Assert.IsTrue(rotated.IsClose(new Vec(-2, 1)), rotated.ToString());
    }
}