using Lib;
using System.IO;
using System.Text.Json;

namespace Squigglizer;

/// <summary>
/// Handles loading and saving settings related to the Squigglizer so that
/// when it is restarted the last used settings can be used.
/// </summary>
public static class SettingsSaver
{
    // This is relative to the git repository root
    public const string USER_SETTINGS_DIR = "user_settings";
    public const string LAST_SETTINGS_FILE = "last_squigglizer_settings.json";
 
    public static T? Load<T>(string filename) where T : class
    {
        var file = Finder.GIT.GetRelativeFile(Path.Combine(USER_SETTINGS_DIR, filename));

        T? result = null;

        if (file.Exists)
        {
            try
            {
                return FileUtil.ReadJson<T>(file.FullName);
            }
            catch
            {
                // Do nothing, we'll just overwrite with the defaults
            }
        }

        return result;
    }

    public static void Save<T>(string filename, T obj)
    {
        var dir = Finder.GIT.GetRelativeDir(USER_SETTINGS_DIR);

        if (!dir.Exists)
        {
            dir.Create();
        }

        string file = Path.Combine(dir.FullName, filename);
        File.WriteAllText(file, JsonSerializer.Serialize(obj));
    }
}
