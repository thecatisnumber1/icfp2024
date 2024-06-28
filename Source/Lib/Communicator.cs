namespace Lib;

public class Communicator
{
    private const string ENDPOINT = "https://boundvariable.space/communicate";
    private const string KEY = "ff993058-d102-4ee8-913f-e1c39614b957";

    public static String Send(string message)
    {
        RestConnector.SetKey(KEY);
        return RestConnector.PostFile(ENDPOINT, message);
    }
}
