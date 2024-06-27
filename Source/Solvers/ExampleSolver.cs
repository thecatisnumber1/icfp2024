using Core;
using Lib;

namespace Solvers;

public class ExampleSolver : Solver
{
    private int startX, endX, startY, endY;

    public ExampleSolver(Dictionary<string, string> args, ILogger logger) : base(args, logger)
    {
        Logger.LogMessage("Hello World");
        Logger.LogMessage($"Passed [{string.Join(',', Args.Keys)}] example args");
    }

    public override void Start(Assignment assignment)
    {
        base.Start(assignment);
        Logger.LogMessage("Starting ExampleSolver");
        startX = 0;
        endX = assignment.Width - 1;
        startY = 0;
        endY = assignment.Height - 1;
    }

    public override void Step()
    {
        if (Assignment == null)
        {
            Logger.LogError("Assignment is null");
            return;
        };

        if (startX > endX || startY > endY)
        {
            Logger.LogError("Failed to find target");
            return;
        }

        int midX = (startX + endX) / 2;
        int midY = (startY + endY) / 2;

        Logger.LogMessage($"Guessing {midX}, {midY}");
        Vec result = Assignment.Guess(midX, midY);

        if (result == Vec.ZERO)
        {
            Logger.LogMessage($"Found target at {midX}, {midY}");
            return;
        }

        var (x, y) = result;

        if (x == 1) startX = midX + 1;
        else if (x == -1) endX = midX - 1;

        if (y == 1) startY = midY + 1;
        else if (y == -1) endY = midY - 1;
    }
}
