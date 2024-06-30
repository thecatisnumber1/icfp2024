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
            return fullPath;
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