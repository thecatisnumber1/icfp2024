using Core;
using static Core.Shorthand;

namespace CoreTests;

[TestClass]
public class ShorthandTest
{
    [TestMethod]
    public void BasicTest()
    {
        string expected = "B$ B$ L# L$ v# B. SB%,,/ S}Q/2,$_ IK";

        var x = V("#");
        var y = V("$");

        Expression actual = Apply(Apply(Lambda(x, Lambda(y, x)), Concat(S("Hello"), S(" World!"))), I(42));

        Assert.AreEqual(expected, actual.ToICFP());
    }
}
