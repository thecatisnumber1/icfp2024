using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spaceship
{
    public class HardcodedFlier
    {
        private static Dictionary<int, (int dx, int dy)> velocities = new Dictionary<int, (int dx, int dy)>
        {
            { 1, (-1, -1) },
            { 2, (0, -1) },
            { 3, (1, -1) },
            { 4, (-1, 0) },
            { 5, (0, 0) },
            { 6, (1, 0) },
            { 7, (-1, 1) },
            { 8, (0, 1) },
            { 9, (1, 1) }
        };
        public static List<int> HardCodedFlier(SpaceshipProblem problem)
        {
            Ship ship = new Ship();
            List<int> solution = new List<int>();
            ship.Move(9);
            ship.Move(5);
            while (problem.TargetPoints.Count > 0)
            {
                bool moved = false;
                moved = TryOneMoveAhead(problem, ship, solution) ||
                        TryTwoMovesAhead(problem, ship, solution) ||
                        TryThreeMovesAhead(problem, ship, solution) ||
                        TryFourMovesAhead(problem, ship, solution);

                if (!moved)
                {
                    return null;
                }
            }
            return solution;
        }

        static bool TryOneMoveAhead(SpaceshipProblem problem, Ship ship, List<int> solution)
        {
            foreach (var target in problem.TargetPoints.ToList())
            {
                foreach (var vel in velocities)
                {
                    int newX = ship.X + ship.VX + vel.Value.dx;
                    int newY = ship.Y + ship.VY + vel.Value.dy;
                    if (newX == target.X && newY == target.Y)
                    {
                        ship.Move(vel.Key);
                        solution.Add(vel.Key);
                        problem.TargetPoints.Remove(target);
                        return true;
                    }
                }
            }
            return false;
        }

        static bool TryTwoMovesAhead(SpaceshipProblem problem, Ship ship, List<int> solution)
        {
            foreach (var vel1 in velocities)
            {
                int midX = ship.X + ship.VX + vel1.Value.dx;
                int midY = ship.Y + ship.VY + vel1.Value.dy;
                int midVX = ship.VX + vel1.Value.dx;
                int midVY = ship.VY + vel1.Value.dy;

                foreach (var vel2 in velocities)
                {
                    int newX = midX + midVX + vel2.Value.dx;
                    int newY = midY + midVY + vel2.Value.dy;

                    foreach (var target in problem.TargetPoints.ToList())
                    {
                        if (newX == target.X && newY == target.Y)
                        {
                            ship.Move(vel1.Key);
                            ship.Move(vel2.Key);
                            solution.Add(vel1.Key);
                            solution.Add(vel2.Key);
                            problem.TargetPoints.Remove(target);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        static bool TryThreeMovesAhead(SpaceshipProblem problem, Ship ship, List<int> solution)
        {
            foreach (var vel1 in velocities)
            {
                int midX1 = ship.X + ship.VX + vel1.Value.dx;
                int midY1 = ship.Y + ship.VY + vel1.Value.dy;
                int midVX1 = ship.VX + vel1.Value.dx;
                int midVY1 = ship.VY + vel1.Value.dy;

                foreach (var vel2 in velocities)
                {
                    int midX2 = midX1 + midVX1 + vel2.Value.dx;
                    int midY2 = midY1 + midVY1 + vel2.Value.dy;
                    int midVX2 = midVX1 + vel2.Value.dx;
                    int midVY2 = midVY1 + vel2.Value.dy;

                    foreach (var vel3 in velocities)
                    {
                        int newX = midX2 + midVX2 + vel3.Value.dx;
                        int newY = midY2 + midVY2 + vel3.Value.dy;

                        foreach (var target in problem.TargetPoints.ToList())
                        {
                            if (newX == target.X && newY == target.Y)
                            {
                                ship.Move(vel1.Key);
                                ship.Move(vel2.Key);
                                ship.Move(vel3.Key);
                                solution.Add(vel1.Key);
                                solution.Add(vel2.Key);
                                solution.Add(vel3.Key);
                                problem.TargetPoints.Remove(target);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        static bool TryFourMovesAhead(SpaceshipProblem problem, Ship ship, List<int> solution)
        {
            foreach (var vel1 in velocities)
            {
                int midX1 = ship.X + ship.VX + vel1.Value.dx;
                int midY1 = ship.Y + ship.VY + vel1.Value.dy;
                int midVX1 = ship.VX + vel1.Value.dx;
                int midVY1 = ship.VY + vel1.Value.dy;

                foreach (var vel2 in velocities)
                {
                    int midX2 = midX1 + midVX1 + vel2.Value.dx;
                    int midY2 = midY1 + midVY1 + vel2.Value.dy;
                    int midVX2 = midVX1 + vel2.Value.dx;
                    int midVY2 = midVY1 + vel2.Value.dy;

                    foreach (var vel3 in velocities)
                    {
                        int midX3 = midX2 + midVX2 + vel3.Value.dx;
                        int midY3 = midY2 + midVY2 + vel3.Value.dy;
                        int midVX3 = midVX2 + vel3.Value.dx;
                        int midVY3 = midVY2 + vel3.Value.dy;

                        foreach (var vel4 in velocities)
                        {
                            int newX = midX3 + midVX3 + vel4.Value.dx;
                            int newY = midY3 + midVY3 + vel4.Value.dy;

                            foreach (var target in problem.TargetPoints.ToList())
                            {
                                if (newX == target.X && newY == target.Y)
                                {
                                    ship.Move(vel1.Key);
                                    ship.Move(vel2.Key);
                                    ship.Move(vel3.Key);
                                    ship.Move(vel4.Key);
                                    solution.Add(vel1.Key);
                                    solution.Add(vel2.Key);
                                    solution.Add(vel3.Key);
                                    solution.Add(vel4.Key);
                                    problem.TargetPoints.Remove(target);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
