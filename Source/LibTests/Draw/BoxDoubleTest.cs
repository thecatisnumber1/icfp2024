using Point = Lib.PointDouble;
using Box = Lib.BoxDouble;
using System.Text.Json;

namespace LibTests;

[TestClass]
public class BoxDoubleTest
{
    [TestMethod]
    public void Construction()
    {
        var b1 = Box.From(0.0, 0.0, 0.0, 0.0);
        var b2 = Box.From(Point.ORIGIN, 0.0, 0.0);
        var b3 = new Box(Point.ORIGIN, Point.ORIGIN);
        var b4 = JsonSerializer.Deserialize<Box>("{ \"BottomLeft\": { \"X\": 0.0, \"Y\": 0.0 }, \"TopRight\": { \"X\": 0.0, \"Y\": 0.0 } }");

        Assert.AreEqual(b1, b2);
        Assert.AreEqual(b1, b3);
        Assert.AreEqual(b1, b4);

        var b5 = Box.From(1.5, 2.5, 3.5, 4.5);
        var b6 = Box.From(new Point(1.5, 2.5), 2, 2);
        var b7 = new Box(new Point(1.5, 2.5), new Point(3.5, 4.5));
        var b8 = JsonSerializer.Deserialize<Box>("{ \"BottomLeft\": { \"X\": 1.5, \"Y\": 2.5 }, \"TopRight\": { \"X\": 3.5, \"Y\": 4.5 } }");
        
        Assert.AreNotEqual(b1, b5);
        Assert.AreEqual(b5, b6);
        Assert.AreEqual(b5, b7);
        Assert.AreEqual(b5, b8);
    }

    [TestMethod]
    public void Equality()
    {
        var b1 = Box.From(1.5, 2.5, 3.5, 4.5);
        var b2 = Box.From(1.5, 2.5, 3.5, 4.5);
        var b3 = Box.From(5.5, 6.5, 7.5, 8.5);

        Assert.IsTrue(b1 == b2);
        Assert.IsFalse(b1 == b3);

        Assert.IsFalse(b1 != b2);
        Assert.IsTrue(b1 != b3);

        Assert.IsTrue(b1.Equals(b2));
        Assert.IsFalse(b1.Equals(b3));
        Assert.IsFalse(b1.Equals("b1"));
    }

    [TestMethod]
    public void String()
    {
        var b = Box.From(1.5, 2.5, 3.5, 4.5);
        var str = b.ToString();
        Assert.AreEqual("((1.5, 2.5), (3.5, 4.5))", str);
    }

    [TestMethod]
    public void Shift()
    {
        var b = Box.From(1.5, 2.5, 3.5, 4.5);

        var shift1 = b.Shift(new Vec(1.5, 2.5));
        Assert.AreNotSame(b, shift1);
        Assert.AreEqual(Box.From(3, 5, 5, 7), shift1);

        var shift2 = b.Shift(1.5, 2.5);
        Assert.AreNotSame(b, shift2);
        Assert.AreEqual(Box.From(3, 5, 5, 7), shift2);
    }

    [TestMethod]
    public void Resize()
    {
        var b = Box.From(1.5, 2.5, 3.5, 4.5);

        var resize1 = b.Resize(new Vec(1.5, 2.5));
        Assert.AreNotSame(b, resize1);
        Assert.AreEqual(Box.From(1.5, 2.5, 5, 7), resize1);

        var resize2 = b.Resize(2.5);
        Assert.AreNotSame(b, resize2);
        Assert.AreEqual(Box.From(1.5, 2.5, 6, 7), resize2);
    }
}
