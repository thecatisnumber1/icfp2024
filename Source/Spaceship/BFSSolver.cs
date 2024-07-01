using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Lib.PointInt;
using Lib;
using System.Collections.Immutable;


namespace Spaceship
{
    public class BFSSolver
    {
        public List<int> Solve(List<Point> targets, int lookahead, int maxSearchLength, bool debug = false)
        {
            List<int> solution = new List<int>();
            Ship ship = new Ship();

            for (int i = 0; i < targets.Count; i++)
            {
                List<Point> toVisit = targets.GetRange(i, Math.Min(lookahead, targets.Count - i));
                List<int> path = GetPathToFirstPoint(ship, toVisit, maxSearchLength);

                if (debug)
                {
                    Console.WriteLine($"Found path to {i} out of {targets.Count}");
                }

                if (path == null)
                {
                    if (debug)
                    {
                        BFSSolver.PrintPath(solution);
                    }
                    return null;
                }

                int commandIndex = 0;
                while (ship.Pos != toVisit[0])
                {
                    ship.Move(path[commandIndex]);
                    solution.Add(path[commandIndex]);
                    commandIndex++;
                } 
            }

            return solution;
        }

        public static void PrintPath(List<int> commands)
        {
            Ship s = new Ship();
            Console.WriteLine($"Pos = ({s.X}, {s.Y}), Vel = ({s.VX}, {s.VY})");
            foreach (int command in commands)
            {
                s.Move(command);
                Console.WriteLine($"Pos = ({s.X}, {s.Y}), Vel = ({s.VX}, {s.VY})");
            }
        }

        public List<int> GetPathToFirstPoint(Ship ship, List<Point> toVisit, int maxSearchLength)
        {
            Queue<(Point pos, Vec v, int vCount, ImmutableStack<int> commands, int pathLength)> toExplore = new();
            HashSet<(Point pos, Vec v, int vCount)> visited = new();
            toExplore.Enqueue((ship.Pos, ship.Velocity, 0, ImmutableStack<int>.Empty, 0));
            while (toExplore.Count > 0)
            {
                var (pos, v, vCount, commands, pathLength) = toExplore.Dequeue();
                if (visited.Contains((pos, v, vCount)))
                {
                    continue;
                }

                visited.Add((pos, v, vCount));
                for (int i = 1; i <= 9; i++) 
                {
                    Vec nextV = v + Command.GetVec(i);
                    Point nextPos = pos + nextV;
                    int nextVCount = vCount;
                    var nextCommands = commands.Push(i);
                    int nextPathLength = pathLength + 1;
                    if (toVisit[vCount] == nextPos)
                    {
                        nextVCount++;
                    }

                    if (nextVCount == toVisit.Count)
                    {
                        return nextCommands.Reverse().ToList();
                    }

                    if (pathLength < maxSearchLength)
                    {
                        toExplore.Enqueue((nextPos, nextV, nextVCount, nextCommands, nextPathLength));
                    }
                }
            }

            return null;
        }
    }
}
