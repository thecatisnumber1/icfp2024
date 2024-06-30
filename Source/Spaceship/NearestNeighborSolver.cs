using Lib;
using Point = Lib.PointInt;

namespace Spaceship
{
    public class NearestNeighborSolver
    {
        private readonly SimplifiedNavigator navigator;

        public NearestNeighborSolver()
        {
            navigator = new SimplifiedNavigator();
        }

        public List<int> Solve(SpaceshipProblem problem)
        {
            Console.WriteLine("Starting Nearest Neighbor Solver for Spaceship Problem...");

            List<Point> remainingPoints = new List<Point>(problem.TargetPoints);
            List<int> fullPath = new List<int>();
            Point currentPoint = new Point(0, 0); // Starting point

            while (remainingPoints.Count > 0)
            {
                Point nearestPoint = FindNearestPoint(currentPoint, remainingPoints);
                List<int> segmentPath = navigator.FindPath(currentPoint.X, currentPoint.Y, nearestPoint.X, nearestPoint.Y);
                fullPath.AddRange(segmentPath);
                currentPoint = nearestPoint;
                remainingPoints.Remove(nearestPoint);
            }

            Console.WriteLine($"Final path length: {fullPath.Count}");

            Console.WriteLine("But Wait! There's more! Trimming full stops...");

            List<int> trimmedPath = TrimFullStops(fullPath);

            Console.WriteLine($"Trimmed path length: {trimmedPath.Count}");

            return trimmedPath;
        }

        private static List<int> TrimFullStops(List<int> solution)
        {
            List<int> trimSolution = [];
            Ship ship = new();
            Vec prevPrevV = Vec.ZERO;
            Vec prevV = Vec.ZERO;

            for (int i = 0; i < solution.Count; i++)
            {
                prevPrevV = prevV;
                prevV = ship.Velocity;

                ship.Move(solution[i]);
                trimSolution.Add(solution[i]);

                // If we've done a full stop
                if (i != 0 && prevV == Vec.ZERO)
                {
                    // We might be able to trim the full stop by applying the diff as one step
                    // instead of performing the two stop/start steps
                    var diff = ship.Velocity - prevPrevV;
                    var diffCommand = Command.GetCommand(diff);

                    //Console.WriteLine($"Diff: {diff} -> {diffCommand}".PadLeft(50));

                    if (diffCommand > 0)
                    {
                        trimSolution.RemoveAt(trimSolution.Count - 1);
                        trimSolution.RemoveAt(trimSolution.Count - 1);
                        trimSolution.Add(diffCommand);
                    }
                }

                //Console.WriteLine($"Command: {solution[i]}, Pos: {ship.Pos}, Vel: {ship.Velocity}");
            }

            return trimSolution;
        }

        private Point FindNearestPoint(Point currentPoint, List<Point> points)
        {
            return points.OrderBy(p => CalculateDistance(currentPoint, p)).First();
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}