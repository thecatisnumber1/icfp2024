using System.Text.Json;

namespace Lib;

public class Scoreboard
{
    private const string ENDPOINT = "https://patcdr.net/carl/icfp2024/scoreboard/json";

    private readonly Dictionary<string, long> scores;

    private Scoreboard()
    {
        scores = [];
    }

    public long this[string problem]
    {
        get => scores[problem];
    }

    public static Scoreboard Fetch()
    {
        using var client = new HttpClient();

        var get = client.GetStringAsync(ENDPOINT);
        get.Wait();
        string result = get.Result;

        var entries = JsonSerializer.Deserialize<Dictionary<string, Entry>>(result);

        if (entries == null)
        {
            throw new Exception("bad response from scoreboard");
        }

        Scoreboard sb = new();
        
        foreach (var (p, s) in entries)
        {
            sb.scores[p] = s.received_score;
        }

        return sb;
    }

    private record class Entry(long received_score) { }
}
