namespace LibTests;

[TestClass]
public class FinderTest
{
    private static readonly string[][] DIRS = [["A", "B", "C"], ["D", "E", "F"], ["G", "H", "I"]];
#pragma warning disable CS8618 // Non-nullable field
    private static DirectoryInfo tempDir;
    private static List<string> allFiles;
#pragma warning restore CS8618

    [ClassInitialize]
    public static void ClassInit(TestContext ctx)
    {
        tempDir = Directory.CreateTempSubdirectory();
        allFiles = [];

        foreach (string[] dirs in DIRS)
        {
            var currDir = tempDir;
            string fileName = "";

            foreach (string dir in dirs)
            {
                fileName += dir;
                currDir = currDir.CreateSubdirectory(dir);

                string path = Path.Combine(currDir.FullName, fileName + ".txt");
                using var f = File.Create(path);
                allFiles.Add(path);
            }
        }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        tempDir.Delete(true);
    }

    [TestMethod]
    public void CWD_IsNotNull()
    {
        Assert.IsNotNull(Finder.CWD.Dir);
    }

    [TestMethod]
    public void SOLUTION_IsNotNull()
    {
        Assert.IsNotNull(Finder.SOLUTION.Dir);
    }

    [TestMethod]
    public void SOLUTION_HasSln()
    {
        Assert.IsNotNull(Finder.SOLUTION.FindFile("*.sln"));
    }

    [TestMethod]
    public void GIT_IsNotNull()
    {
        Assert.IsNotNull(Finder.GIT.Dir);
    }

    [TestMethod]
    public void GIT_HasGit()
    {
        Assert.IsNotNull(Finder.GIT.FindDir(".git"));
    }

    [TestMethod]
    public void Constructor_WhenGivenDir_StartsInSameDir()
    {
        Finder finder = new(tempDir.FullName);

        Assert.AreEqual(tempDir.FullName, finder.Dir.FullName);
    }

    [TestMethod]
    public void Constructor_WhenGivenFile_StartsInFilesDir()
    {
        Finder finder = new(Path.Combine(tempDir.FullName, "A", "A.txt"));

        Assert.AreEqual(Path.Combine(tempDir.FullName, "A"), finder.Dir.FullName);
    }

    [TestMethod]
    public void Constructor_NormalizesPaths()
    {
        string expected = Path.Combine(tempDir.FullName, "A");
        Assert.AreEqual(expected, new Finder(tempDir.FullName + "/A").Dir.FullName);
        Assert.AreEqual(expected, new Finder(tempDir.FullName + @"\A").Dir.FullName);
    }

    [TestMethod]
    public void GetRelativeFile_WhenFileExists_ReturnsFile()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNotNull(finder.GetRelativeFile("A/A.txt"));
    }

    [TestMethod]
    public void GetRelativeFile_WhenFileDoesNotExist_ReturnsFile()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNotNull(finder.GetRelativeFile("A/Z.txt"));
    }

    [TestMethod]
    public void GetRelativeFile_NormalizesPaths()
    {
        Finder finder = new(tempDir.FullName);

        Assert.AreEqual(finder.GetRelativeFile(@"A\A.txt")?.FullName, finder.GetRelativeFile("A/A.txt")?.FullName);
    }

    [TestMethod]
    public void GetRelativeFile_WhenPathIsNotRelative_ReturnsFile()
    {
        Finder finder = new(tempDir.FullName);
        string fullPath = Path.Combine(finder.Dir.FullName, "A", "A.txt");

        Assert.AreEqual(fullPath, finder.GetRelativeFile(fullPath)?.FullName);
    }

    [TestMethod]
    public void FindRelativeFile_WhenFileDoesNotExist_ThrowsFileNotFoundException()
    {
        Finder finder = new(tempDir.FullName);

        Assert.ThrowsException<FileNotFoundException>(() => finder.FindRelativeFile("A/Z.txt"));
    }

    [TestMethod]
    public void GetRelativeDir_WhenDirExists_ReturnsFile()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNotNull(finder.GetRelativeDir("A/B"));
    }

    [TestMethod]
    public void GetRelativeDir_WhenDirDoesNotExist_ReturnsDir()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNotNull(finder.GetRelativeDir("A/Z"));
    }

    [TestMethod]
    public void GetRelativeDir_NormalizesPaths()
    {
        Finder finder = new(tempDir.FullName);

        Assert.AreEqual(finder.GetRelativeDir(@"A\B")?.FullName, finder.GetRelativeDir("A/B")?.FullName);
    }

    [TestMethod]
    public void GetRelativeDir_WhenPathIsNotRelative_ReturnsDir()
    {
        Finder finder = new(tempDir.FullName);
        string fullPath = Path.Combine(finder.Dir.FullName, "A", "B");

        Assert.AreEqual(fullPath, finder.GetRelativeDir(fullPath)?.FullName);
    }

    [TestMethod]
    public void FindRelativeDir_WhenDirDoesNotExist_ThrowsDirectoryNotFoundException()
    {
        Finder finder = new(tempDir.FullName);

        Assert.ThrowsException<DirectoryNotFoundException>(() => finder.FindRelativeDir("A/Z"));
    }

    [TestMethod]
    public void FindFile_WhenFileExists_ReturnsFile()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNotNull(finder.FindFile("A.txt"));
    }

    [TestMethod]
    public void FindFile_WhenFileDoesNotExist_ReturnsNull()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNull(finder.FindFile("Z.txt"));
    }

    [TestMethod]
    public void FindDir_WhenDirExists_ReturnsDir()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNotNull(finder.FindDir("A"));
    }

    [TestMethod]
    public void FindDir_WhenDirDoesNotExist_ReturnsNull()
    {
        Finder finder = new(tempDir.FullName);

        Assert.IsNull(finder.FindDir("Z"));
    }

    [TestMethod]
    public void FindFiles_ForBroadSearch_ReturnsAllFiles()
    {
        Finder finder = new(tempDir.FullName);

        CollectionAssert.AreEquivalent(allFiles, finder.FindFiles(@".*").Select(f => f.FullName).ToList());
    }

    [TestMethod]
    public void FindFiles_ForSpecificSearch_ReturnsSpecificFiles()
    {
        Finder finder = new(tempDir.FullName);

        string[] expected = ["A.txt", "AB.txt", "ABC.txt"];
        CollectionAssert.AreEquivalent(expected, finder.FindFiles(@"A.?.?\.txt$").Select(f => f.Name).ToList());
    }

    [TestMethod]
    public void FindFileUp_WhenFileExists_ReturnsFile()
    {
        Finder finder = new(Path.Combine(tempDir.FullName, "A", "B"));

        Assert.IsNotNull(finder.FindFileUp("A.txt"));
    }

    [TestMethod]
    public void FindFileUp_WhenFileDoesNotExist_ReturnsNull()
    {
        Finder finder = new(Path.Combine(tempDir.FullName, "A", "B"));

        Assert.IsNull(finder.FindFileUp("D.txt"));
    }

    [TestMethod]
    public void FindDirUp_WhenDirExists_ReturnsDir()
    {
        Finder finder = new(Path.Combine(tempDir.FullName, "A", "B"));

        Assert.IsNotNull(finder.FindDirUp(tempDir.Name));
    }

    [TestMethod]
    public void FindDirUp_WhenDirDoesNotExist_ReturnsNull()
    {
        Finder finder = new(Path.Combine(tempDir.FullName, "A", "B"));

        Assert.IsNull(finder.FindFileUp("abc0123defzz^z"));
    }
}
