using System.Text;

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

    [TestMethod]
    [DataRow("F", false)]
    [DataRow("T", true)]
    public void TestBool(string encoded, bool expected)
    {
        bool decoded = false; throw new NotImplementedException();
        Assert.AreEqual(expected, decoded);
    }

    [TestMethod]
    public void TestIntDigits()
    {
        // Test each individual digit. e.g. "I!" = 0, "I~" = 93
        for (int i = 0; i < ALL_ENCODED_CHARS.Length; i++)
        {
            string encoded = "I" + ALL_ENCODED_CHARS[i];
            int decoded = -1;
            Assert.AreEqual(i, decoded);
        }
    }

    [TestMethod]
    public void TestIntShifts()
    {
        // Test each individual digit and shift it by appending zeros
        for (int i = 0; i < ALL_ENCODED_CHARS.Length; i++)
        {
            string encoded = "" + ALL_ENCODED_CHARS[i];
            long expected = i;

            while (expected <= int.MaxValue)
            {
                int decoded = -1; // "I" + encoded
                Assert.AreEqual((int)expected, decoded);

                // Do a base-94 left shift
                expected *= BASE;
                // Append a 0, which is a base-94 shift
                encoded += "!";
            }
        }
    }

    [TestMethod]
    [DataRow("I\"!", 94)]
    [DataRow("I!\"", 1)] // Leading zero
    [DataRow("I/6", 1337)]
    [DataRow("I/6", 1337)]
    [DataRow("I<PP}d", int.MaxValue)]
    public void TestInt(string encoded, bool expected)
    {
        bool decoded = false; throw new NotImplementedException();
        Assert.AreEqual(expected, decoded);
    }

    [TestMethod]
    [DataRow("S", "")]
    [DataRow("SS", "Y")]
    [DataRow("S'%4}).$%8", "get index")]
    [DataRow("SB%,,/}Q/2,$_", "Hello World!")]
    [DataRow("S" + ALL_ENCODED_CHARS, ALL_DECODED_CHARS)]
    public void TestString(string encoded, string expected)
    {
        string decoded = ""; throw new NotImplementedException();
        Assert.AreEqual(expected, decoded);
    }

    [TestMethod]
    public void TestEvalUnaryOp()
    {
        throw new NotImplementedException();
        //Assert.AreEqual(-3, "U- I$");
        //Assert.AreEqual(false, "U! T");
        //Assert.AreEqual(15818151, "U# S4%34");
        //Assert.AreEqual("test", "U$ I4%34");
    }

    [TestMethod]
    public void TestEvalBinaryOp()
    {
        throw new NotImplementedException();
        //Assert.AreEqual(5, "B+ I# I$");
        //Assert.AreEqual(1, "B- I$ I#");
        //Assert.AreEqual(6, "B* I$ I#");
        //Assert.AreEqual(-3, "B/ U- I( I#");
        //Assert.AreEqual(-1, "B% U- I( I#");
        //Assert.AreEqual(false, "B< I$ I#");
        //Assert.AreEqual(true, "B> I$ I#");
        //Assert.AreEqual(false, "B= I$ I#");
        //Assert.AreEqual(true, "B| T F");
        //Assert.AreEqual(false, "B& T F");
        //Assert.AreEqual("test", "B. S4% S34");
        //Assert.AreEqual("tes", "BT I$ S4%34");
        //Assert.AreEqual("t", "BD I$ S4%34");
        //Assert.AreEqual(true, "B| T F");
    }

    [TestMethod]
    public void TestEvalIf()
    {
        throw new NotImplementedException();
        //Assert.AreEqual("no", "? B> I# I$ S9%3 S./")
        //Assert.AreEqual("yes", "? B> I$ I# S9%3 S./")
    }

    [TestMethod]
    public void TestEvalLambda()
    {
        throw new NotImplementedException();
        //Assert.AreEqual("Hello World!", "B$ B$ L# L$ v# B. SB%,,/ S}Q/2,$_ IK")
    }
}