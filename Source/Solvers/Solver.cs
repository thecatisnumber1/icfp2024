using Core;
using Lib;
namespace Solvers;

public abstract class Solver(Dictionary<string, string> args, ILogger logger)
{
    protected Dictionary<string, string> Args { get; } = args;
    protected ILogger Logger { get; } = logger;
    protected Assignment? Assignment { get; private set; }

    public virtual void Start(Assignment assignment)
    {
        Assignment = assignment;
    }

    public abstract void Step();
}
