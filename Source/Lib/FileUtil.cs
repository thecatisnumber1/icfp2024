using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Lib;

/// <summary>
/// Utility class for reading and writing files.
/// All pathing is based on the "Repo Root", which is the top-level repository directory containing the ".git" directory.
/// </summary>
public static class FileUtil
{
    private static readonly Finder ROOT = Finder.GIT;

    /// <summary>
    /// Reads and returns the text from a file in the repository
    /// </summary>
    public static string ReadText(string pathFromRepoRoot)
    {
        return File.ReadAllText(ROOT.FindRelativeFile(pathFromRepoRoot).FullName);
    }

    /// <summary>
    /// Reads and returns the deserialized JSON object from a file in the repository
    /// </summary>
    public static T? ReadJson<T>(string pathFromRepoRoot)
    {
        string text = ReadText(pathFromRepoRoot);
        return JsonSerializer.Deserialize<T>(text);
    }

    /// <summary>
    /// Writes text to a file in the repository
    /// </summary>
    public static void WriteText(string pathFromRepoRoot, string contents)
    {
        string file = ROOT.GetRelativeFile(pathFromRepoRoot).FullName;
        File.WriteAllText(file, contents);
    }

    /// <summary>
    /// Serializes an object to JSON and writes it to a file in the repository
    /// </summary>
    public static void WriteJson<T>(string pathFromProjectRoot, T obj)
    {
        string file = ROOT.GetRelativeFile(pathFromProjectRoot).FullName;
        string text = JsonSerializer.Serialize<T>(obj);
        WriteText(file, text);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1416: Only supported on Windows")]
    public static Image LoadPNG(string pathFromRepoRoot)
    {
        Bitmap original = new(ROOT.FindRelativeFile(pathFromRepoRoot).FullName);
        int width = original.Width;
        int height = original.Height;

        PixelFormat format = PixelFormat.Format32bppArgb;
        int depth = System.Drawing.Image.GetPixelFormatSize(format) / 8; // Return size in bytes

        byte[] imageBytes = new byte[original.Width * original.Height * depth];

        BitmapData bmpData = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
        Marshal.Copy(bmpData.Scan0, imageBytes, 0, bmpData.Stride * height);

        original.UnlockBits(bmpData);

        Color[,] pixels = new Color[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int baseIndex = (x + y * height) * 4;
                // ARGB -> RGBA requires offsetting by 1, 2, 3, and then 0
                pixels[x, height - 1 - y] = new Color(imageBytes[baseIndex + 2], imageBytes[baseIndex + 1], imageBytes[baseIndex], imageBytes[baseIndex + 3]);
            }
        }

        return new Image(pixels);
    }
}
