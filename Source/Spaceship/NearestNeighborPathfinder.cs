using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Lib.PointInt;

namespace Spaceship
{
    public class NearestNeighborPathfinder
    {
        public static List<Point> OrderPoints(List<Point> points)
        {
            List<Point> orderedPoints = new List<Point>();
            Point currentPoint = Point.ORIGIN;
            HashSet<Point> toVisit = new HashSet<Point>(points);
            while (toVisit.Count > 0)
            {
                Point nearestPoint = Point.ORIGIN;
                long minDist = long.MaxValue;
                foreach (var point in toVisit)
                {
                    if (point.DistSq(currentPoint) < minDist)
                    {
                        minDist = point.DistSq(currentPoint);
                        nearestPoint = point;
                    }
                }

                orderedPoints.Add(nearestPoint);
                toVisit.Remove(nearestPoint);
                currentPoint = nearestPoint;
            }

            return orderedPoints;
        }

        public static void DumpPointsToFile(List<Point> points, string filename)
        {
            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach (var point in points)
                {
                    file.WriteLine($"{point.X} {point.Y}");
                }
            }
        }
    }
}
