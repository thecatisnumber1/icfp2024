using Core;

namespace CoreTests;

[TestClass]
public class DecodeTest
{
    private const int BASE = 94;
    private const string ALL_ENCODED_CHARS = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
    private const string ALL_DECODED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`|~ \n";

    [TestMethod]
    public void TestAssumptions()
    {
        Assert.AreEqual(BASE, ALL_ENCODED_CHARS.Length);
        Assert.AreEqual(BASE, ALL_DECODED_CHARS.Length);
    }

    private static T Parse<T>(string icfp) where T : Expression
    {
        var expr = Expression.Parse(icfp);
        Assert.IsInstanceOfType(expr, typeof(T));
        return (T)expr;
    }

    [TestMethod]
    [DataRow("F", false)]
    [DataRow("T", true)]
    public void TestBool(string icfp, bool expected)
    {
        bool decoded = Parse<Bool>(icfp).AsBool();
        Assert.AreEqual(expected, decoded);
    }

    [TestMethod]
    public void TestIntDigits()
    {
        // Test each individual digit. e.g. "I!" = 0, "I~" = 93
        for (int i = 0; i < ALL_ENCODED_CHARS.Length; i++)
        {
            long decoded = Parse<Integer>("I" + ALL_ENCODED_CHARS[i]).AsInt();
            Assert.AreEqual(i, decoded);
        }
    }

    [TestMethod]
    public void TestIntShifts()
    {
        // Test each individual digit and shift it by appending zeros
        // Don't start at 0 or infinite loop lulz
        for (int i = 1; i < ALL_ENCODED_CHARS.Length; i++)
        {
            string icfp = "I" + ALL_ENCODED_CHARS[i];
            long expected = i;

            while (expected >= 0 && expected <= long.MaxValue)
            {
                long decoded = Parse<Integer>(icfp).AsInt();
                Assert.AreEqual(expected, decoded);

                // Do a base-94 left shift
                expected *= BASE;
                // Append a 0, which is a base-94 shift
                icfp += "!";
            }
        }
    }

    [TestMethod]
    [DataRow("I\"!", 94L)]
    [DataRow("I!\"", 1L)] // Leading zero
    [DataRow("I/6", 1337L)]
    [DataRow("I<PP}d", (long)int.MaxValue)]
    [DataRow("I1**0#VEx9D", long.MaxValue)]
    public void TestInt(string icfp, long expected)
    {
        long decoded = Parse<Integer>(icfp).AsInt();
        Assert.AreEqual(expected, decoded);
    }

    [TestMethod]
    [DataRow("S", "")]
    [DataRow("SS", "Y")]
    [DataRow("S'%4}).$%8", "get index")]
    [DataRow("SB%,,/}Q/2,$_", "Hello World!")]
    [DataRow("S" + ALL_ENCODED_CHARS, ALL_DECODED_CHARS)]
    public void TestString(string icfp, string expected)
    {
        string decoded = Parse<Str>(icfp).AsString();
        Assert.AreEqual(expected, decoded);
    }

    [TestMethod]
    public void TestEvalUnaryOp()
    {
        Assert.AreEqual(new Integer(-3), Parse<Unary>("U- I$").Eval());
        Assert.AreEqual(Bool.False, Parse<Unary>("U! T").Eval());
        Assert.AreEqual(new Integer(15818151), Parse<Unary>("U# S4%34").Eval());
        Assert.AreEqual(Str.Make("test"), Parse<Unary>("U$ I4%34").Eval());
    }

    [TestMethod]
    public void TestEvalBinaryOp()
    {
        Assert.AreEqual(new Integer(5), Parse<Binary>("B+ I# I$").Eval());
        Assert.AreEqual(new Integer(1), Parse<Binary>("B- I$ I#").Eval());
        Assert.AreEqual(new Integer(6), Parse<Binary>("B* I$ I#").Eval());
        Assert.AreEqual(new Integer(-3), Parse<Binary>("B/ U- I( I#").Eval());
        Assert.AreEqual(new Integer(-1), Parse<Binary>("B% U- I( I#").Eval());
        Assert.AreEqual(Bool.False, Parse<Binary>("B< I$ I#").Eval());
        Assert.AreEqual(Bool.True, Parse<Binary>("B> I$ I#").Eval());
        Assert.AreEqual(Bool.False, Parse<Binary>("B= I$ I#").Eval());
        Assert.AreEqual(Bool.True, Parse<Binary>("B| T F").Eval());
        Assert.AreEqual(Bool.False, Parse<Binary>("B& T F").Eval());
        Assert.AreEqual(Str.Make("test"), Parse<Binary>("B. S4% S34").Eval());
        Assert.AreEqual(Str.Make("tes"), Parse<Binary>("BT I$ S4%34").Eval());
        Assert.AreEqual(Str.Make("t"), Parse<Binary>("BD I$ S4%34").Eval());
        Assert.AreEqual(Bool.True, Parse<Binary>("B| T F").Eval());
    }

    [TestMethod]
    public void TestEvalIf()
    {
        Assert.AreEqual(Str.Make("no"), Parse<If>("? B> I# I$ S9%3 S./").Eval());
        Assert.AreEqual(Str.Make("yes"), Parse<If>("? B> I$ I# S9%3 S./").Eval());
    }

    [TestMethod]
    public void TestEvalLambda()
    {
        Assert.AreEqual(Str.Make("Hello World!"), Parse<Binary>("B$ B$ L# L$ v# B. SB%,,/ S}Q/2,$_ IK").Eval());
    }

    [TestMethod]
    public void TestLanguage()
    {
        string languageTest = "? B= B$ B$ B$ B$ L$ L$ L$ L# v$ I\" I# I$ I% I$ ? B= B$ L$ v$ I+ I+ ? B= BD I$ S4%34 S4 ? B= BT I$ S4%34 S4%3 ? B= B. S4% S34 S4%34 ? U! B& T F ? B& T T ? U! B| F F ? B| F T ? B< U- I$ U- I# ? B> I$ I# ? B= U- I\" B% U- I$ I# ? B= I\" B% I( I$ ? B= U- I\" B/ U- I$ I# ? B= I# B/ I( I$ ? B= I' B* I# I$ ? B= I$ B+ I\" I# ? B= U$ I4%34 S4%34 ? B= U# S4%34 I4%34 ? U! F ? B= U- I$ B- I# I& ? B= I$ B- I& I# ? B= S4%34 S4%34 ? B= F F ? B= I$ I$ ? T B. B. SM%,&k#(%#+}IEj}3%.$}z3/,6%},!.'5!'%y4%34} U$ B+ I# B* I$> I1~s:U@ Sz}4/}#,!)-}0/).43}&/2})4 S)&})3}./4}#/22%#4 S\").!29}q})3}./4}#/22%#4 S\").!29}q})3}./4}#/22%#4 S\").!29}q})3}./4}#/22%#4 S\").!29}k})3}./4}#/22%#4 S5.!29}k})3}./4}#/22%#4 S5.!29}_})3}./4}#/22%#4 S5.!29}a})3}./4}#/22%#4 S5.!29}b})3}./4}#/22%#4 S\").!29}i})3}./4}#/22%#4 S\").!29}h})3}./4}#/22%#4 S\").!29}m})3}./4}#/22%#4 S\").!29}m})3}./4}#/22%#4 S\").!29}c})3}./4}#/22%#4 S\").!29}c})3}./4}#/22%#4 S\").!29}r})3}./4}#/22%#4 S\").!29}p})3}./4}#/22%#4 S\").!29}{})3}./4}#/22%#4 S\").!29}{})3}./4}#/22%#4 S\").!29}d})3}./4}#/22%#4 S\").!29}d})3}./4}#/22%#4 S\").!29}l})3}./4}#/22%#4 S\").!29}N})3}./4}#/22%#4 S\").!29}>})3}./4}#/22%#4 S!00,)#!4)/.})3}./4}#/22%#4 S!00,)#!4)/.})3}./4}#/22%#4";
        var expr = Expression.Parse(languageTest);
        var result = expr.Eval();
        Assert.AreEqual(Str.Make("Self-check OK, send `solve language_test 4w3s0m3` to claim points for it"), result);
    }
}