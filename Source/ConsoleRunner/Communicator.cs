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
}
