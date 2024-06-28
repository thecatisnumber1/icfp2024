using Core;
using Lib;

namespace ConsoleRunner;

public static class Communicator
{
    private const string ENDPOINT = "https://boundvariable.space/communicate";
    private const string KEY = "ff993058-d102-4ee8-913f-e1c39614b957";

    public static string Send(string icfp)
    {
        RestConnector.SetKey(KEY);
        return RestConnector.PostFile(ENDPOINT, icfp);
    }
}
