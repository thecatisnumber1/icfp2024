using Point = Lib.PointInt;
using Box = Lib.BoxInt;
using System.Text.Json;

namespace LibTests;

[TestClass]
public class BoxIntTest
{
    [TestMethod]
    public void Construction()
    {
        var b1 = Box.From(0, 0, 0, 0);
        var b2 = Box.From(Point.ORIGIN, 0, 0);
        var b3 = new Box(Point.ORIGIN, Point.ORIGIN);
        var b4 = JsonSerializer.Deserialize<Box>("{ \"BottomLeft\": { \"X\": 0, \"Y\": 0 }, \"TopRight\": { \"X\": 0, \"Y\": 0 } }");

        Assert.AreEqual(b1, b2);
        Assert.AreEqual(b1, b3);
        Assert.AreEqual(b1, b4);

        var b5 = Box.From(1, 2, 3, 4);
        var b6 = Box.From(new Point(1, 2), 2, 2);
        var b7 = new Box(new Point(1, 2), new Point(3, 4));
        var b8 = JsonSerializer.Deserialize<Box>("{ \"BottomLeft\": { \"X\": 1, \"Y\": 2 }, \"TopRight\": { \"X\": 3, \"Y\": 4 } }");
        
        Assert.AreNotEqual(b1, b5);
        Assert.AreEqual(b5, b6);
        Assert.AreEqual(b5, b7);
        Assert.AreEqual(b5, b8);
    }

    [TestMethod]
    public void Equality()
    {
        var b1 = Box.From(1, 2, 3, 4);
        var b2 = Box.From(1, 2, 3, 4);
        var b3 = Box.From(5, 6, 7, 8);

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
        var b = Box.From(1, 2, 3, 4);
        var str = b.ToString();
        Assert.AreEqual("((1, 2), (3, 4))", str);
    }

    [TestMethod]
    public void Shift()
    {
        var b = Box.From(1, 2, 3, 4);

        var shift1 = b.Shift(new Vec(1, 2));
        Assert.AreNotSame(b, shift1);
        Assert.AreEqual(Box.From(2, 4, 4, 6), shift1);

        var shift2 = b.Shift(1, 2);
        Assert.AreNotSame(b, shift2);
        Assert.AreEqual(Box.From(2, 4, 4, 6), shift2);
    }

    [TestMethod]
    public void Resize()
    {
        var b = Box.From(1, 2, 3, 4);

        var resize1 = b.Resize(new Vec(1, 2));
        Assert.AreNotSame(b, resize1);
        Assert.AreEqual(Box.From(1, 2, 4, 6), resize1);

        var resize2 = b.Resize(2);
        Assert.AreNotSame(b, resize2);
        Assert.AreEqual(Box.From(1, 2, 5, 6), resize2);
    }
}
