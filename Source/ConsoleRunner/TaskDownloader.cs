using Core;
using Lib;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleRunner;

public static class TaskDownloader
{
    private const string TASKS_DIR = "Tasks";

    public static void Download(string taskName)
    {
        var tasksDir = Finder.GIT.FindRelativeDir(TASKS_DIR);
        string taskFile = Path.Combine(tasksDir.FullName, taskName + ".txt");
        
        if (!DownloadSingle(taskName, taskFile))
        {
            return;
        }

        var taskProblemsDir = Finder.GIT.GetRelativeDir(Path.Combine(TASKS_DIR, taskName));

        if (!taskProblemsDir.Exists)
        {
            taskProblemsDir.Create();
        }

        Regex regex = new(@"^\* \[(" + taskName + @"\d+)\]");
        string task = File.ReadAllText(taskFile);

        foreach (string line in task.Replace("\r", "").Split("\n"))
        {
            var match = regex.Match(line);

            if (match.Success)
            {
                string problem = match.Groups[1].Value;
                string problemFile = Path.Combine(taskProblemsDir.FullName, problem + ".txt");

                if (!DownloadSingle(problem, problemFile))
                {
                    return;
                }
            }
        }
    }

    private static bool DownloadSingle(string filename, string filepath)
    {
        string icfpReply = Communicator.Send(Str.Make($"get {filename}").ToICFP());
        string reply;

        try
        {
            reply = Expression.Parse(icfpReply).Eval([]).ToString() ?? "";
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception while parsing response for: get {filename}");
            Console.WriteLine(e);
            return false;
        }

        File.WriteAllText(filepath, reply);
        Console.WriteLine($"Downloaded {filename}.txt");
        return true;
    }
}
