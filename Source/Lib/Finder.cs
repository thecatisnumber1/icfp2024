using System.Text.RegularExpressions;

namespace Lib;

/// <summary>
/// Finds files and directories.
/// </summary>
public class Finder
{
    /// <summary>
    /// Finder rooted at the current working directory.
    /// </summary>
    public static readonly Finder CWD = new(Directory.GetCurrentDirectory());
    
    /// <summary>
    /// Finder rooted at the Visual Studio solution's directory.
    /// This is determined by looking in parent directories until a .sln file is found.
    /// </summary>
    public static readonly Finder SOLUTION = new(CWD.FindFileUp("*.sln"));

    /// <summary>
    /// Finder rooted at the base directory of the current Git repo.
    /// This is determined by looking in parent directories until a .git directory is found.
    /// </summary>
    public static readonly Finder GIT = new(CWD.FindDirUp(".git")?.Parent);

    public DirectoryInfo Dir { get; private init; }

    /// <summary>
    /// Creates a Finder rooted at the given start path. All operations
    /// are relative to this path. If startPath is a file, the root of
    /// operations will be that file's directory.
    /// </summary>
    /// <param name="startPath">The root path for operations</param>
    /// <exception cref="ArgumentException">If startPath does not exist</exception>
    public Finder(string startPath)
    {
        // Normalize the path
        startPath = Normalize(startPath);

        if (Directory.Exists(startPath))
        {
            Dir = new(startPath);
        }
        else if (File.Exists(startPath))
        {
            Dir = Directory.GetParent(startPath)!;
        }
        else
        {
            throw new FileNotFoundException($"No such file or directory: {startPath}");
        }
    }

    private Finder(FileSystemInfo? info)
    {
        if (info == null)
        {
            throw new FileNotFoundException("No such file or directory");
        }
        else if (info is DirectoryInfo dir)
        {
            Dir = dir;
        }
        else if (info is FileInfo file)
        {
            Dir = file.Directory ?? throw new ArgumentException("File has no parent dir");
        }
        else
        {
            throw new ArgumentException("Unknown FileSystemInfo type");
        }
    }

    // Converts paths that have non-OS directory separators to the current OS's version.
    private static string Normalize(string path)
    {
        return path.Replace('\\', '/').Replace('/', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Returns a FileInfo for the given path relative to this Finder's root.
    /// If the path is not relative, then this just returns that file.
    /// If the file does not exist, the returned FileInfo's .Exists can be
    /// used to determine that.
    /// </summary>
    /// <param name="relativePath">The relative path to get</param>
    /// <returns>The FileInfo for the path relative to the root</returns>
    public FileInfo GetRelativeFile(string relativePath)
    {
        relativePath = Normalize(relativePath);
        string fullPath;

        if (Path.IsPathRooted(relativePath))
        {
            fullPath = relativePath;
        }
        else
        {
            fullPath = Path.GetFullPath(relativePath, Dir.FullName);
        }

        return new(fullPath);
    }

    /// <summary>
    /// This is the same as GetRelativeFile, except it throws a
    /// FileNotFoundException if the file does not exist.
    /// </summary>
    /// <param name="relativePath">The relative path to get</param>
    /// <returns>The FileInfo for the path relative to the root</returns>
    /// <exception cref="FileNotFoundException">If no such file exists</exception>
    public FileInfo FindRelativeFile(string relativePath)
    {
        var file = GetRelativeFile(relativePath);

        if (!file.Exists)
        {
            throw new FileNotFoundException(file.FullName);
        }

        return file;
    }

    /// <summary>
    /// Returns a DirectoryInfo for the given path relative to this Finder's root.
    /// If the path is not relative, then this just returns that directory.
    /// If the directory does not exist, the returned DirectoryInfo's .Exists can be
    /// used to determine that.
    /// </summary>
    /// <param name="relativePath">The relative path to get</param>
    /// <returns>The DirectoryInfo for the path relative to the root</returns>
    public DirectoryInfo GetRelativeDir(string relativePath)
    {
        relativePath = Normalize(relativePath);
        string fullPath;

        if (Path.IsPathRooted(relativePath))
        {
            fullPath = relativePath;
        }
        else
        {
            fullPath = Path.GetFullPath(relativePath, Dir.FullName);
        }

        return new(fullPath);
    }

    /// <summary>
    /// This is the same as GetRelativeDir, except it throws a
    /// DirectoryNotFoundException if the directory does not exist.
    /// </summary>
    /// <param name="relativePath">The relative path to get</param>
    /// <returns>The DirectoryInfo for the path relative to the root</returns>
    /// <exception cref="DirectoryNotFoundException">If no such directory exists</exception>
    public DirectoryInfo FindRelativeDir(string relativePath)
    {
        var dir = GetRelativeDir(relativePath);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(dir.FullName);
        }

        return dir;
    }

    /// <summary>
    /// Recursively looks for a file of the given name and returns the
    /// first one found. If no such file exists, returns null.
    /// The name may be a path pattern, for example "*.txt" will return
    /// a file with a ".txt" extension.
    /// </summary>
    /// <param name="name">The file name pattern to find</param>
    /// <returns>A matching file or null if none exists</returns>
    public FileInfo? FindFile(string name)
    {
        return Dir.EnumerateFiles(name, SearchOption.AllDirectories).FirstOrDefault();
    }

    /// <summary>
    /// Recursively looks for a directory of the given name and returns the
    /// first one found. If no such directory exists, returns null.
    /// The name may be a path pattern, for example "se*" will return
    /// a directory starting with "se".
    /// </summary>
    /// <param name="name">The directory name pattern to find</param>
    /// <returns>A matching directory or null if none exists</returns>
    public DirectoryInfo? FindDir(string name)
    {
        return Dir.EnumerateDirectories(name, SearchOption.AllDirectories).FirstOrDefault();
    }

    /// <summary>
    /// Recursively enumerates all files whose file name matches the given
    /// regular expression.
    /// </summary>
    /// <param name="regex">The regular expression to match against file names</param>
    /// <returns>An enumeration over matched files</returns>
    public IEnumerable<FileInfo> FindFiles(string regex)
    {
        Regex re = new(regex);

        foreach(var file in Dir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            if (re.IsMatch(file.Name))
            {
                yield return file;
            }
        }
    }

    /// <summary>
    /// Searches parent directories for a file of the given name and
    /// returns the first one found. If no such file exists, returns null.
    /// 
    /// This works by searching files contained in the root dir of this Finder,
    /// then files in the root dir's parent, then that dir's parent, etc.
    /// This does not perform a recursive search.
    /// 
    /// The name may be a path pattern, for example "*.txt" will return
    /// a file with a ".txt" extension.
    /// </summary>
    /// <param name="name">The file name pattern to find</param>
    /// <returns>A matching file or null if none exists</returns>
    public FileInfo? FindFileUp(string name)
    {
        var currDir = Dir;

        while (currDir != null)
        {
            var info = currDir.EnumerateFiles(name, SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (info != null)
            {
                return info;
            }

            currDir = currDir.Parent;
        }

        return null;
    }

    /// <summary>
    /// Searches parent directories for a directory of the given name and
    /// returns the first one found. If no such directory exists, returns null.
    /// 
    /// This works by searching directories contained in the root dir of this Finder,
    /// then directories in the root dir's parent, then that dir's parent, etc.
    /// This does not perform a recursive search.
    /// 
    /// The name may be a path pattern, for example "se*" will return
    /// a directory starting with "se".
    /// </summary>
    /// <param name="name">The directory name pattern to find</param>
    /// <returns>A matching directory or null if none exists</returns>
    public DirectoryInfo? FindDirUp(string name)
    {
        var currDir = Dir;

        while (currDir != null)
        {
            var info = currDir.EnumerateDirectories(name, SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (info != null)
            {
                return info;
            }

            currDir = currDir.Parent;
        }

        return null;
    }
}
