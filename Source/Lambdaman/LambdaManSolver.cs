using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaMan
{
    public class LambdaManSolver
    {
        public static string Solve(LambdaManGrid grid)
        {
            var path = new List<char>();
            var currentPos = grid.StartPosition;

            while (grid.Pills.Count > 0)
            {
                var nearestPill = FindNearestPill(grid, currentPos);
                var subPath = FindPath(grid, currentPos, nearestPill);

                path.AddRange(subPath);
                grid.SetCell(currentPos.X, currentPos.Y, ' ');  // Leave the current position
                currentPos = nearestPill;
                grid.SetCell(currentPos.X, currentPos.Y, 'L');  // Move to new location
                grid.Pills.Remove(currentPos);
            }

            return new string(path.ToArray());
        }

        private static (int X, int Y) FindNearestPill(LambdaManGrid grid, (int X, int Y) start)
        {
            var queue = new Queue<((int X, int Y) Pos, List<char> Path)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((start, new List<char>()));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (currentPos, currentPath) = queue.Dequeue();

                foreach (var (dir, (dx, dy)) in GetDirections())
                {
                    var newPos = (X: currentPos.X + dx, Y: currentPos.Y + dy);
                    if (!visited.Contains(newPos) && !grid.IsWall(newPos.X, newPos.Y))
                    {
                        var newPath = new List<char>(currentPath) { dir };
                        if (grid.GetCell(newPos.X, newPos.Y) == '.')
                        {
                            return newPos;
                        }
                        queue.Enqueue((newPos, newPath));
                        visited.Add(newPos);
                    }
                }
            }

            return start;  // Should never reach here if there are pills left
        }

        private static List<char> FindPath(LambdaManGrid grid, (int X, int Y) start, (int X, int Y) end)
        {
            var queue = new Queue<((int X, int Y) Pos, List<char> Path)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((start, new List<char>()));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (currentPos, currentPath) = queue.Dequeue();

                if (currentPos == end)
                {
                    return currentPath;
                }

                foreach (var (dir, (dx, dy)) in GetDirections())
                {
                    var newPos = (X: currentPos.X + dx, Y: currentPos.Y + dy);
                    if (!visited.Contains(newPos) && !grid.IsWall(newPos.X, newPos.Y))
                    {
                        var newPath = new List<char>(currentPath) { dir };
                        queue.Enqueue((newPos, newPath));
                        visited.Add(newPos);
                    }
                }
            }

            return new List<char>();  // Should never reach here if end is reachable
        }

        private static IEnumerable<(char, (int, int))> GetDirections()
        {
            return new List<(char, (int, int))>
            {
                ('L', (-1, 0)),
                ('D', (0, 1)),
                ('R', (1, 0)),
                ('U', (0, -1))
            };
        }
    }
}
