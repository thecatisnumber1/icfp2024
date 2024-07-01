using Core;
using Lib;
using System.Net;

namespace ConsoleRunner;

public static class Communicator
{
    private const string ENDPOINT = "https://boundvariable.space/communicate";
    private const string KEY = "ff993058-d102-4ee8-913f-e1c39614b957";

    public static string? Send(string icfp)
    {
        RestConnector.SetKey(KEY);

        int sleepMs = 0;
        string icfpReply;
        HttpStatusCode statusCode;

        do
        {
            if (sleepMs > 0)
            {
                Console.WriteLine($"Too many requests, sleeping for {sleepMs} ms");
                Thread.Sleep(sleepMs);
            }
            
            (icfpReply, statusCode) = RestConnector.PostFile(ENDPOINT, icfp);
            sleepMs += 1000;
        }
        while (statusCode == HttpStatusCode.TooManyRequests && sleepMs <= 10000);

        if (statusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"Got HTTP ${statusCode} for: {Expression.Parse(icfp).Eval()}");
            return null;
        }

        return icfpReply;
    }

    public static string? Eval(string icfp, bool isNumExpr = false)
    {
        // For normal echo, prefix command with U$ to convert number to string
        // because B. only works for string args
        string intToStr = isNumExpr ? "U$ " : "";
        // Send to the "echo" command
        string? icfpReply = Communicator.Send("B. S%#(/} " + intToStr + icfp);

        if (icfpReply == null)
        {
            return null;
        }

        string reply = Expression.Parse(icfpReply).Eval().ToString() ?? "";
        string value = reply.Replace("You scored some points for using the echo service!", "").Trim();

        if (isNumExpr)
        {
            // Undo the U$ we used by applying a U#
            return new Unary('#', Str.Make(value)).Eval().ToString();
        }
        else
        {
            return value;
        }
    }
}
