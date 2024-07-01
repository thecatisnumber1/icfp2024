using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Lib.PointInt;
using Lib;

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


        // TODO: For performance, we want a better list copy.
        public List<int> GetPathToFirstPoint(Ship ship, List<Point> toVisit, int maxSearchLength)
        {
            Queue<(Point pos, Vec v, int vCount, List<int> commands)> toExplore = new();
            HashSet<(Point pos, Vec v, int vCount)> visited = new();
            toExplore.Enqueue((ship.Pos, ship.Velocity, 0, new List<int>()));
            while (toExplore.Count > 0)
            {
                var (pos, v, vCount, commands) = toExplore.Dequeue();
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
                    List<int> nextCommands = new List<int>(commands);
                    nextCommands.Add(i);
                    if (toVisit[vCount] == nextPos)
                    {
                        nextVCount++;
                    }

                    if (nextVCount == toVisit.Count)
                    {
                        return nextCommands;
                    }

                    if (nextCommands.Count < maxSearchLength)
                    {
                        toExplore.Enqueue((nextPos, nextV, nextVCount, nextCommands));
                    }
                }
            }

            return null;
        }
    }
}
