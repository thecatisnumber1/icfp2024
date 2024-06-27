using Lib;
namespace Runner;

public record Args(string SolverName, Dictionary<string, string> Rest)
{
    public static Args ParseArgs(string[] args, ILogger logger)
    {
        string? solverName = null;
        Dictionary<string, string> rest = [];

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--solver":
                case "-s":
                    solverName = args[++i];
                    break;
                default:
                    rest.Add(args[i], args[++i]);
                    break;
            }
        }

        if (solverName == null)
        {
            logger.LogError("--solver/-s argument is required");
            Environment.Exit(1);
        }

        return new Args(solverName, rest);
    }
}
