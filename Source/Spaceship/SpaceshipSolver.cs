using Spaceship;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Point = Lib.PointInt;

public class SpaceshipSolver
{
    private Random random = new Random();
    private readonly TimeSpan timeLimit;
    private readonly double initialTemperature;
    private readonly double finalTemperature;

    public SpaceshipSolver(TimeSpan timeLimit, double initialTemperature = 100000.0, double finalTemperature = .01)
    {
        this.timeLimit = timeLimit;
        this.initialTemperature = initialTemperature;
        this.finalTemperature = finalTemperature;
    }

    public List<int> Solve(SpaceshipProblem problem)
    {
        Console.WriteLine("Starting Simulated Annealing Solver for Spaceship Problem...");

        // Initial order of target points
        List<Point> currentOrder = new List<Point>(problem.TargetPoints);
        List<int> currentPath = CalculatePath(currentOrder);
        int currentPathLength = currentPath.Count;

        List<int> bestPath = new List<int>(currentPath);
        int bestPathLength = currentPathLength;

        Stopwatch stopwatch = Stopwatch.StartNew();
        double lastLogTime = 0.0;
        int totalAccepted = 0;
        int totalIterations = 0;

        while (stopwatch.Elapsed < timeLimit)
        {
            double temperature = GetTemperature(stopwatch.Elapsed);
            List<Point> newOrder = new List<Point>(currentOrder);
            SwapRandomPoints(newOrder);
            List<int> newPath = CalculatePath(newOrder);
            int newPathLength = newPath.Count;

            if (newPathLength < currentPathLength || AcceptanceProbability(currentPathLength, newPathLength, temperature) > random.NextDouble())
            {
                currentOrder = newOrder;
                currentPathLength = newPathLength;

                if (newPathLength < bestPathLength)
                {
                    bestPathLength = newPathLength;
                    bestPath = new List<int>(newPath);
                    Console.WriteLine($"New best path length found: {bestPathLength}");
                }

                totalAccepted++;
            }

            totalIterations++;

            // Log every 0.5 seconds
            if (stopwatch.Elapsed.TotalSeconds - lastLogTime >= 0.5)
            {
                double acceptanceRate = totalIterations > 0 ? (double)totalAccepted / totalIterations * 100 : 0;
                Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed.TotalSeconds:F2}s, Total Acceptance Rate: {acceptanceRate:F2}%, t = {temperature}");
                lastLogTime = stopwatch.Elapsed.TotalSeconds;
                totalAccepted = 0;
                totalIterations = 0;
            }
        }

        Console.WriteLine($"Final path length: {bestPathLength}");
        return bestPath;
    }

    private double GetTemperature(TimeSpan elapsed)
    {
        double progress = elapsed.TotalSeconds / timeLimit.TotalSeconds;
        return initialTemperature * Math.Pow(finalTemperature / initialTemperature, progress);
    }

    private double AcceptanceProbability(int currentLength, int newLength, double temperature)
    {
        if (newLength < currentLength)
        {
            return 1.0;
        }
        return Math.Exp((currentLength - newLength) / temperature);
    }

    private void SwapRandomPoints(List<Point> points)
    {
        int index1 = random.Next(points.Count);
        int index2 = random.Next(points.Count);
        Point temp = points[index1];
        points[index1] = points[index2];
        points[index2] = temp;
    }

    private List<int> CalculatePath(List<Point> points)
    {
        SimplifiedNavigator navigator = new SimplifiedNavigator();
        List<int> fullPath = new List<int>();
        Point currentPoint = new Point(0, 0); // Starting point

        foreach (Point targetPoint in points)
        {
            List<int> segmentPath = navigator.FindPath(currentPoint.X, currentPoint.Y, targetPoint.X, targetPoint.Y);
            fullPath.AddRange(segmentPath);
            currentPoint = targetPoint;
        }

        return fullPath;
    }
}