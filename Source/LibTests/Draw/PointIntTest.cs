using Point = Lib.PointInt;
using System.Text.Json;

namespace LibTests;

[TestClass]
public class PointIntTest
{
    [TestMethod]
    public void Construction()
    {
        var p1 = new Point();
        var p2 = new Point(0, 0);
        var p3 = JsonSerializer.Deserialize<Point>("{ \"X\": 0, \"Y\": 0 }");

        Assert.AreEqual(Point.ORIGIN, p1);
        Assert.AreEqual(Point.ORIGIN, p2);
        Assert.AreEqual(Point.ORIGIN, p3);

        var p4 = new Point(1, 2);
        var p5 = JsonSerializer.Deserialize<Point>("{ \"X\": 1, \"Y\": 2 }");

        Assert.AreNotEqual(Point.ORIGIN, p4);
        Assert.AreNotEqual(Point.ORIGIN, p5);
        Assert.AreEqual(p4, p5);
    }

    [TestMethod]
    public void Deconstruct()
    {
        var (x, y) = new Point(1, 2);
        Assert.AreEqual(1, x);
        Assert.AreEqual(2, y);
    }

    [TestMethod]
    public void Clone()
    {
        var p = new Point(1, 2);
        var clone = p with { };
        Assert.AreEqual(p, clone);
        Assert.AreNotSame(p, clone);
    }

    [TestMethod]
    public void Add()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var add = p1 + p2;
        Assert.AreEqual(new Vec(4, 6), add);

        var v = new Vec(5, 6);
        var add2 = p1 + v;
        Assert.AreEqual(new Point(6, 8), add2);

        var add3 = v + p1;
        Assert.AreEqual(new Point(6, 8), add3);
    }

    [TestMethod]
    public void Subtract()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var sub = p2 - p1;
        Assert.AreEqual(new Vec(2, 2), sub);

        var v = new Vec(5, 6);
        var sub2 = p2 - v;
        Assert.AreEqual(new Point(-2, -2), sub2);
    }

    [TestMethod]
    public void Equality()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(1, 2);
        var p3 = new Point(3, 4);

        Assert.IsTrue(p1 == p2);
        Assert.IsFalse(p1 == p3);

        Assert.IsFalse(p1 != p2);
        Assert.IsTrue(p1 != p3);
        
        Assert.IsTrue(p1.Equals(p2));
        Assert.IsFalse(p1.Equals(p3));
        Assert.IsFalse(p1.Equals("p1"));
    }

    [TestMethod]
    public void String()
    {
        var p = new Point(1, 2);
        var str = p.ToString();
        Assert.AreEqual("(1, 2)", str);
    }

    [TestMethod]
    public void DistSq()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var dist = p1.DistSq(p2);
        Assert.AreEqual(8, dist);
    }

    [TestMethod]
    public void Dist()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var dist = p1.Dist(p2);
        Assert.AreEqual(Math.Sqrt(8), dist);
    }

    [TestMethod]
    public void Manhattan()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var dist = p1.Manhattan(p2);
        Assert.AreEqual(4, dist);
    }

    [TestMethod]
    public void Mid()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var mid = p1.Mid(p2);
        Assert.AreEqual(new Point(2, 3), mid);
    }
}