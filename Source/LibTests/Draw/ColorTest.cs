using System.Text.Json;

namespace LibTests;

[TestClass]
public class ColorTest
{
    [TestMethod]
    public void Construction()
    {
        var c1 = new Color();
        var c2 = new Color(0, 0, 0, 0);
        var c3 = JsonSerializer.Deserialize<Color>("{ \"R\": 0, \"G\": 0, \"B\": 0, \"A\": 0 }");

        Assert.AreEqual(Color.EMPTY, c1);
        Assert.AreEqual(Color.EMPTY, c2);
        Assert.AreEqual(Color.EMPTY, c3);

        var c4 = new Color(1, 2, 3, 4);
        var c5 = JsonSerializer.Deserialize<Color>("{ \"R\": 1, \"G\": 2, \"B\": 3, \"A\": 4 }");

        Assert.AreNotEqual(Color.EMPTY, c4);
        Assert.AreNotEqual(Color.EMPTY, c5);
        Assert.AreEqual(c4, c5);
    }

    [TestMethod]
    public void FromHSV()
    {
        var c = Color.FromHSV(0, 0, 0);
        Assert.AreEqual(new Color(0, 0, 0, 255), c);

        var c2 = Color.FromHSV(0, 0, 1);
        Assert.AreEqual(new Color(255, 255, 255, 255), c2);

        var c3 = Color.FromHSV(0, 1, 0);
        Assert.AreEqual(new Color(0, 0, 0, 255), c3);

        var c4 = Color.FromHSV(0, 1, 1);
        Assert.AreEqual(new Color(255, 0, 0, 255), c4);

        var c5 = Color.FromHSV(1.0 / 6, 1, 1);
        Assert.AreEqual(new Color(255, 255, 0, 255), c5);

        var c6 = Color.FromHSV(2.0 / 6, 1, 1);
        Assert.AreEqual(new Color(0, 255, 0, 255), c6);

        var c7 = Color.FromHSV(3.0 / 6, 1, 1);
        Assert.AreEqual(new Color(0, 255, 255, 255), c7);

        var c8 = Color.FromHSV(4.0 / 6, 1, 1);
        Assert.AreEqual(new Color(0, 0, 255, 255), c8);

        var c9 = Color.FromHSV(5.0 / 6, 1, 1);
        Assert.AreEqual(new Color(255, 0, 255, 255), c9);

        var c10 = Color.FromHSV(0.5, 0.5, 0.5);
        Assert.AreEqual(new Color(63, 127, 127, 255), c10);
    }

    [TestMethod]
    public void Deconstruct()
    {
        var (r, g, b, a) = new Color(1, 2, 3, 4);
        Assert.AreEqual(1, r);
        Assert.AreEqual(2, g);
        Assert.AreEqual(3, b);
        Assert.AreEqual(4, a);
    }

    [TestMethod]
    public void Clone()
    {
        var c = new Color(1, 2, 3, 4);
        var clone = c with { };
        Assert.AreEqual(c, clone);
        Assert.AreNotSame(c, clone);
    }

    [TestMethod]
    public void Equality()
    {
        var c1 = new Color(1, 2, 3, 4);
        var c2 = new Color(1, 2, 3, 4);
        var c3 = new Color(5, 6, 7, 8);

        Assert.IsTrue(c1 == c2);
        Assert.IsFalse(c1 == c3);

        Assert.IsFalse(c1 != c2);
        Assert.IsTrue(c1 != c3);

        Assert.IsTrue(c1.Equals(c2));
        Assert.IsFalse(c1.Equals(c3));
        Assert.IsFalse(c1.Equals("c1"));
    }

    [TestMethod]
    public void String()
    {
        var c = new Color(1, 2, 3, 4);
        var str = c.ToString();
        Assert.AreEqual("[1, 2, 3, 4]", str);
    }

    [TestMethod]
    public void Diff()
    {
        var c1 = new Color(1, 2, 3, 4);
        var c2 = new Color(5, 6, 7, 8);
        var diff = c1.Diff(c2);
        Assert.AreEqual(8, diff);
    }
}
