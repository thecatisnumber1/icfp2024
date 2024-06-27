using Core;
using Lib;
using Solvers;
namespace Runner;

public class Run
{
    public static string[] SolverNames { get; } = Catalog.Names;

    public Solver Solver { get; }
    public Assignment Assignment { get; }

    public Run(string solverName, Dictionary<string, string> solverArgs, ILogger logger)
    {
        Solver = Catalog.CreateSolver(solverName, solverArgs, logger) ?? throw new Exception("Failed to create solver");
        Assignment = new Assignment(100, 100, logger);
        Solver.Start(Assignment);
    }

    public void Step()
    {
        Solver.Step();
    }

    public void RunUntilDone()
    {
        while (!Assignment.IsSolved)
        {
            Step();
        }
    }
}