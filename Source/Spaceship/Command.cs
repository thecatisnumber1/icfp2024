using Lib;

namespace Spaceship;

public static class Command
{
    private static readonly Vec[] Velocities =
    {
        new(-1, -1),
        new(0, -1),
        new(1, -1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(-1, 1),
        new(0, 1),
        new(1, 1)
    };

    public static Vec GetVec(int command)
    {
        return Velocities[command - 1];
    }

    /// <summary>
    /// In order, 0-indexed
    /// </summary>
    public static IEnumerable<Vec> GetCandidates(Vec current)
    {
        return Velocities.Select(v => new Vec(current.X + v.X, current.Y + v.Y));
    }
}
