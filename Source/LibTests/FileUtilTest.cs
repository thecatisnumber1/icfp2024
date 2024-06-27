namespace LibTests;

[TestClass]
public class FileUtilTest
{
    private const string TestDir = "Test Files";
    private const string UnknownDir = "ICFP Template Test Unknown";

#pragma warning disable CS8618 // Non-nullable field
    private static DirectoryInfo tempDir;
#pragma warning restore CS8618

    [ClassInitialize]
    public static void ClassInit(TestContext ctx)
    {
        tempDir = Directory.CreateTempSubdirectory();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        tempDir.Delete(true);
    }

    [TestMethod]
    public void ReadWriteTextTest()
    {
        string file = Path.Combine(tempDir.FullName, "Test.txt");
        string text = "Test Files Text";

        FileUtil.WriteText(file, text);
        Assert.AreEqual(text, FileUtil.ReadText(file));
    }

    [TestMethod]
    public void ReadWriteJsonTest()
    {
        string file = Path.Combine(tempDir.FullName, "Json.txt");
        BoxInt box = BoxInt.From(1, 2, 3, 4);

        FileUtil.WriteJson(file, box);
        Assert.AreEqual(box, FileUtil.ReadJson<BoxInt>(file));
    }

    [TestMethod]
    public void LoadPNGFileTest()
    {
        Assert.ThrowsException<FileNotFoundException>(() => FileUtil.LoadPNG($"{UnknownDir}/PNG.png"));
        Assert.ThrowsException<FileNotFoundException>(() => FileUtil.LoadPNG($"{TestDir}/Unknown PNG.png"));

        var content = FileUtil.LoadPNG($"{TestDir}/PNG.png");
        Assert.AreEqual(10, content.Width);
        Assert.AreEqual(10, content.Height);
        Assert.AreEqual(new Color(34, 177, 76, 255), content[0, 0]);
    }
}
