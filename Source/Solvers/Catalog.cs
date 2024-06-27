using Lib;
using System.Reflection;
namespace Solvers;

public static class Catalog
{
    public static string[] Names { get; } = [.. GetSubclassNames()];

    private static IEnumerable<string> GetSubclassNames()
    {
        var baseType = typeof(Solver);
        var assembly = Assembly.GetAssembly(baseType);

        if (assembly == null)
        {
            return [];
        }

        return assembly.GetTypes()
            .Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract)
            .Select(type => type.Name);
    }

    public static Solver? CreateSolver(string name, Dictionary<string, string> args, ILogger logger)
    {
        string fullName = $"Solvers.{name}";
        var type = Type.GetType(fullName);

        if (type != null && typeof(Solver).IsAssignableFrom(type))
        {
            return (Solver?)Activator.CreateInstance(type, [args, logger]);
        }

        logger.LogError($"Failed to create solver {name}");
        return null;
    }
}
