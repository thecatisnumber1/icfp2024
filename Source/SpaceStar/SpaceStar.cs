using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
// using OxyPlot;
// using OxyPlot.Series;
// using OxyPlot.GtkSharp;

class Program
{
    static List<Point> ReadPointsFromFile(string filePath)
    {
        var points = new List<Point>();
        foreach (var line in File.ReadLines(filePath))
        {
            var parts = line.Split();
            points.Add(new Point(int.Parse(parts[0]), int.Parse(parts[1])));
        }
        return points;
    }

    static void PlotPointsAndPath(List<Point> points, List<Point> path = null)
    {
        // var model = new PlotModel { Title = "Points and Path on 2D Plane" };
        // var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Blue };
        // foreach (var point in points)
        // {
        //     scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
        // }
        // model.Series.Add(scatterSeries);

        // if (path != null)
        // {
        //     var lineSeries = new LineSeries { Color = OxyColors.Red };
        //     foreach (var point in path)
        //     {
        //         lineSeries.Points.Add(new DataPoint(point.X, point.Y));
        //     }
        //     model.Series.Add(lineSeries);
        // }

        // Application.Init();
        // var window = new Window("OxyPlot on macOS")
        // {
        //     DefaultWidth = 800,
        //     DefaultHeight = 800
        // };
        // var plotView = new PlotView
        // {
        //     Model = model
        // };
        // window.Add(plotView);
        // window.ShowAll();
        // Application.Run();
    }

    static int Heuristic(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    static List<(Point, Point)> GetNeighbors(Point point, Point velocity)
    {
        var neighbors = new List<(Point, Point)>();
        for (int dvx = -1; dvx <= 1; dvx++)
        {
            for (int dvy = -1; dvy <= 1; dvy++)
            {
                var newVelocity = new Point(velocity.X + dvx, velocity.Y + dvy);
                var newPoint = new Point(point.X + newVelocity.X, point.Y + newVelocity.Y);
                neighbors.Add((newPoint, newVelocity));
            }
        }
        return neighbors;
    }

    static List<(List<Point> path, Point velocity)> AStar(Point start, List<Point> goals, Point currentVelocity, int maxPositionTests = 100000, int maxPaths = 3, int maxVelocity = 10)
    {
        var openSet = new PriorityQueue<(Point point, Point velocity, List<Point> path), int>();
        openSet.Enqueue((start, currentVelocity, new List<Point> { start }), 0);
        
        var gScore = new Dictionary<Point, List<int>> { [start] = new List<int> { 0 } };
        var fScore = new Dictionary<Point, List<int>> { [start] = new List<int> { goals.Min(goal => Heuristic(start, goal)) } };
        int positionTests = 0;

        var paths = new List<(List<Point> path, Point velocity)>();

        while (openSet.Count > 0 && positionTests < maxPositionTests && paths.Count < maxPaths)
        {
            var currentItem = openSet.Dequeue();
            var (current, velocity, currentPath) = currentItem;
            positionTests++;

            if (goals.Contains(current))
            {
                paths.Add((currentPath, velocity));
                continue;
            }

            foreach (var (neighbor, newVelocity) in GetNeighbors(current, velocity))
            {
                if (Math.Abs(newVelocity.X) > maxVelocity || Math.Abs(newVelocity.Y) > maxVelocity)
                    continue;

                int tentativeGScore = gScore[current][0] + 1;

                if (!gScore.ContainsKey(neighbor) || gScore[neighbor].Count < maxPaths || tentativeGScore < gScore[neighbor].Max())
                {
                    if (!gScore.ContainsKey(neighbor))
                    {
                        gScore[neighbor] = new List<int>();
                        fScore[neighbor] = new List<int>();
                    }
                    gScore[neighbor].Add(tentativeGScore);
                    fScore[neighbor].Add(tentativeGScore + goals.Min(goal => Heuristic(neighbor, goal)));
                    openSet.Enqueue((neighbor, newVelocity, new List<Point>(currentPath) { neighbor }), fScore[neighbor].Last());
                }
            }
        }
        return paths.OrderBy(p => p.path.Count).ToList();
    }

    static List<string> ReconstructVelocityChanges(List<Point> path, Point startVelocity)
    {
        int dx = startVelocity.X, dy = startVelocity.Y;
        var totalChanges = new List<string>();

        for (int i = 1; i < path.Count; i++)
        {
            int cx = path[i].X - path[i - 1].X, cy = path[i].Y - path[i - 1].Y;
            var change = (cx - dx, cy - dy);
            switch (change)
            {
                case (1, 0): totalChanges.Add("6"); break;
                case (-1, 0): totalChanges.Add("4"); break;
                case (0, 1): totalChanges.Add("8"); break;
                case (0, -1): totalChanges.Add("2"); break;
                case (1, 1): totalChanges.Add("9"); break;
                case (1, -1): totalChanges.Add("3"); break;
                case (-1, 1): totalChanges.Add("7"); break;
                case (-1, -1): totalChanges.Add("1"); break;
                default: totalChanges.Add("5"); break;
            }
            dx = cx;
            dy = cy;
        }
        return totalChanges;
    }

    static (List<Point> path, List<string> moves) FindSimplePath(List<Point> points)
    {
        var start = new Point(0, 0);
        var fullPath = new List<Point> { start };
        var velocity = new Point(0, 0);

        while (points.Any())
        {
            List<(List<Point> path, Point velocity)> paths = null;
            for (int maxVelocity = 5; maxVelocity > 1; maxVelocity -= 1)
            {
                paths = AStar(start, points, velocity, maxVelocity: maxVelocity);
                if (paths.Any())
                    break;
                Console.WriteLine($"No paths found with max velocity: {maxVelocity}");
            }

            if (paths == null || !paths.Any())
                return (null, null);

            var (path, newVelocity) = paths.First();
            fullPath.AddRange(path.Skip(1));
            start = path.Last();
            velocity = newVelocity;

            points = points.Except(new List<Point> { start }).OrderBy(point => Heuristic(start, point)).ToList();
            Console.WriteLine($"Found path with {fullPath.Count} points and velocity: {newVelocity}. Remaining points: {points.Count}");
        }
        return (fullPath, ReconstructVelocityChanges(fullPath, new Point(0, 0)));
    }

    static void VerifyResults(List<string> moves, List<Point> points)
    {
        var visitedPoints = new HashSet<Point>();
        var currentPosition = new Point(0, 0);
        var currentVelocity = new Point(0, 0);
        visitedPoints.Add(currentPosition);

        foreach (var move in moves)
        {
            switch (move)
            {
                case "1": currentVelocity = new Point(currentVelocity.X - 1, currentVelocity.Y - 1); break;
                case "2": currentVelocity = new Point(currentVelocity.X, currentVelocity.Y - 1); break;
                case "3": currentVelocity = new Point(currentVelocity.X + 1, currentVelocity.Y - 1); break;
                case "4": currentVelocity = new Point(currentVelocity.X - 1, currentVelocity.Y); break;
                case "5": break;
                case "6": currentVelocity = new Point(currentVelocity.X + 1, currentVelocity.Y); break;
                case "7": currentVelocity = new Point(currentVelocity.X - 1, currentVelocity.Y + 1); break;
                case "8": currentVelocity = new Point(currentVelocity.X, currentVelocity.Y + 1); break;
                case "9": currentVelocity = new Point(currentVelocity.X + 1, currentVelocity.Y + 1); break;
            }

            currentPosition = new Point(currentPosition.X + currentVelocity.X, currentPosition.Y + currentVelocity.Y);
            visitedPoints.Add(currentPosition);
        }

        bool allPointsVisited = points.All(point => visitedPoints.Contains(point));
        if (allPointsVisited)
            Console.WriteLine("All points visited at least once.");
        else
        {
            Console.WriteLine("Not all points were visited.");
            var missingPoints = points.Where(point => !visitedPoints.Contains(point)).ToList();
            Console.WriteLine($"Missing points: {string.Join(", ", missingPoints)}");
        }
    }

    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please provide the problem id as a command line argument.");
            return;
        }

        if (!int.TryParse(args[0], out int problem))
        {
            Console.WriteLine("Invalid problem id. Please provide a valid integer.");
            return;
        }

        string filePath = $"../../Tasks/spaceship/spaceship{problem}-decoded.txt";
        var pointsList = ReadPointsFromFile(filePath);
        Console.WriteLine(string.Join(", ", pointsList));

        var (shortestPath, shortestMoves) = FindSimplePath(pointsList);
        Console.WriteLine(string.Join(", ", shortestPath));
        Console.WriteLine(string.Join("", shortestMoves));
        Console.WriteLine(shortestMoves.Count);

        VerifyResults(shortestMoves, pointsList);
        PlotPointsAndPath(pointsList, shortestPath);

        Console.WriteLine("Submit? (y/n)");
        if (Console.ReadLine() == "y")
        {
            // Assuming `solve` is a namespace with a method `Submit`
            // Solve.Submit("spaceship", $"spaceship{problem}", shortestMoves.Count, "S" + Solve.EncodeString($"solve spaceship{problem} " + string.Join("", shortestMoves)), new Dictionary<string, string>());
        }
    }
}
