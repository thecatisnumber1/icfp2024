using Core;
using Lib;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Net;
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
        string? taskText;

        if ((taskText = DownloadSingle(taskName, tasksDir.FullName)) == null)
        {
            return;
        }

        var taskProblemsDir = Finder.GIT.GetRelativeDir(Path.Combine(TASKS_DIR, taskName));

        Regex regex = new(@"^\* \[(" + taskName + @"\d+)\]");

        foreach (string line in taskText.Split("\r\n"))
        {
            var match = regex.Match(line);

            if (match.Success)
            {
                if (!taskProblemsDir.Exists)
                {
                    taskProblemsDir.Create();
                }

                string problem = match.Groups[1].Value;

                if (DownloadSingle(problem, taskProblemsDir.FullName) == null)
                {
                    return;
                }
            }
        }
    }

    private static string? DownloadSingle(string filename, string dirPath)
    {
        string? icfpReply = Communicator.Send(Str.Make($"get {filename}").ToICFP());
        
        if (icfpReply == null)
        {
            return null;
        }

        string rawFilename = filename + "-raw.txt";
        File.WriteAllText(Path.Combine(dirPath, rawFilename), icfpReply);
        Console.WriteLine($"Downloaded to {rawFilename}");

        string reply;

        try
        {
            reply = Expression.Parse(icfpReply).Eval().ToString() ?? "";
        }
        catch (Exception e)
        {
            Console.WriteLine(icfpReply);
            Console.WriteLine($"Exception while parsing response for: get {filename}");
            Console.WriteLine(e);
            return null;
        }

        reply = reply.Replace("\r", "").Replace("\n", "\r\n");

        string decodedFilename = filename + "-decoded.txt";
        File.WriteAllText(Path.Combine(dirPath, decodedFilename), reply);
        Console.WriteLine($"Decoded to {decodedFilename}");
        return reply;
    }
}
