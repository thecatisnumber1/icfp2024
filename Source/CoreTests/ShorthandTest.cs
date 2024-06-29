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

    [TestMethod]
    public void RecurrsionTest()
    {
        var factorial = new Variable("1");
        var n = new Variable("2");

        Expression factorialFunc = RecursiveFunc(factorial, n)(
            If(n == I(0),
                I(1),
                n * RecursiveCall(factorial, n - I(1))
            )
        );

        Assert.AreEqual(120, Apply(factorialFunc, I(5)).Eval().AsInt());
    }
}
