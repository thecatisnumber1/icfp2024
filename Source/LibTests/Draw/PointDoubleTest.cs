using Point = Lib.PointDouble;
using System.Text.Json;

namespace LibTests;

[TestClass]
public class PointDoubleTest
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

        var p6 = new Point(0.0, 0.0);
        var p7 = JsonSerializer.Deserialize<Point>("{ \"X\": 0.0, \"Y\": 0.0 }");

        Assert.AreEqual(Point.ORIGIN, p6);
        Assert.AreEqual(Point.ORIGIN, p7);

        var p8 = new Point(1.1, 2.2);
        var p9 = JsonSerializer.Deserialize<Point>("{ \"X\": 1.1, \"Y\": 2.2 }");

        Assert.AreNotEqual(Point.ORIGIN, p8);
        Assert.AreNotEqual(Point.ORIGIN, p9);
        Assert.AreEqual(p8, p9);
    }

    [TestMethod]
    public void Deconstruct()
    {
        var (x, y) = new Point(1, 2);
        Assert.AreEqual(1, x);
        Assert.AreEqual(2, y);

        var (x2, y2) = new Point(1.1, 2.2);
        Assert.AreEqual(1.1, x2);
        Assert.AreEqual(2.2, y2);
    }

    [TestMethod]
    public void Clone()
    {
        var p = new Point(1, 2);
        var clone = p with { };
        Assert.AreEqual(p, clone);
        Assert.AreNotSame(p, clone);

        var p2 = new Point(1.1, 2.2);
        var clone2 = p2 with { };
        Assert.AreEqual(p2, clone2);
        Assert.AreNotSame(p2, clone2);
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

        var p4 = new Point(1.1, 2.2);
        var p5 = new Point(3.3, 4.4);
        var add4 = p4 + p5;
        Assert.IsTrue(add4.IsClose(new Vec(4.4, 6.6)), add4.ToString());

        var v2 = new Vec(5.5, 6.6);
        var add5 = p4 + v2;
        Assert.IsTrue(add5.IsClose(new Point(6.6, 8.8)), add5.ToString());

        var add6 = v2 + p4;
        Assert.IsTrue(add6.IsClose(new Point(6.6, 8.8)), add6.ToString());
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
        Assert.IsTrue(sub2.IsClose(new Point(-2, -2)), sub2.ToString());

        var p3 = new Point(1.1, 2.2);
        var p4 = new Point(3.3, 4.4);
        var sub3 = p4 - p3;
        Assert.IsTrue(sub3.IsClose(new Vec(2.2, 2.2)), sub3.ToString());

        var v2 = new Vec(5.5, 6.6);
        var sub4 = p4 - v2;
        Assert.IsTrue(sub4.IsClose(new Point(-2.2, -2.2)), sub4.ToString());
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

        var p4 = new Point(1.1, 2.2);
        var p5 = new Point(1.1, 2.2);
        var p6 = new Point(3.3, 4.4);

        Assert.IsTrue(p4 == p5);
        Assert.IsFalse(p4 == p6);
        
        Assert.IsFalse(p4 != p5);
        Assert.IsTrue(p4 != p6);

        Assert.IsTrue(p4.Equals(p5));
        Assert.IsFalse(p4.Equals(p6));
        Assert.IsFalse(p4.Equals("p4"));
    }

    [TestMethod]
    public void String()
    {
        var p = new Point(1, 2);
        var str = p.ToString();
        Assert.AreEqual("(1, 2)", str);

        var p2 = new Point(1.1, 2.2);
        var str2 = p2.ToString();
        Assert.AreEqual("(1.1, 2.2)", str2);
    }

    [TestMethod]
    public void DistSq()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var dist = p1.DistSq(p2);
        Assert.AreEqual(8, dist);

        var p3 = new Point(1.1, 2.2);
        var p4 = new Point(3.3, 4.4);
        var dist2 = p3.DistSq(p4);
        Assert.IsTrue(9.679 < dist2 && dist2 < 9.681, dist2.ToString());
    }

    [TestMethod]
    public void Dist()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var dist = p1.Dist(p2);
        Assert.AreEqual(Math.Sqrt(8), dist);

        var p3 = new Point(1.1, 2.2);
        var p4 = new Point(3.3, 4.4);
        var dist2 = p3.Dist(p4);
        Assert.IsTrue(Math.Sqrt(9.679) < dist2 && dist2 < Math.Sqrt(9.681), dist2.ToString());
    }

    [TestMethod]
    public void Manhattan()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var dist = p1.Manhattan(p2);
        Assert.AreEqual(4, dist);

        var p3 = new Point(1.1, 2.2);
        var p4 = new Point(3.3, 4.4);
        var dist2 = p3.Manhattan(p4);
        Assert.AreEqual(4.4, dist2);
    }

    [TestMethod]
    public void Mid()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var mid = p1.Mid(p2);
        Assert.AreEqual(new Point(2, 3), mid);

        var p3 = new Point(1.1, 2.2);
        var p4 = new Point(3.3, 4.4);
        var mid2 = p3.Mid(p4);
        Assert.IsTrue(mid2.IsClose(new Point(2.2, 3.3)), mid2.ToString());
    }
}