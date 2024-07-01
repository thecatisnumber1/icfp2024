using Lib;
using Core;
using System.Runtime.Serialization.Formatters;
using System.Text.Json;
using Point = Lib.PointInt;

namespace Spaceship
{
    internal class Program
    {
        HashSet<int> SolvedProblems = new HashSet<int> { 1, 2, 3, 5, 6, 8, 11, 12, };

        static void Main(string[] args)
        {
            var allProblems = ProblemLoader.LoadProblems();

            for (int i = 0; i < allProblems.Count; i++)
            {
                var problem = allProblems[i];
                if (!problem.Name.Equals("spaceship13"))
                {
                    continue;
                }

                List<Point> ordered = NearestNeighborPathfinder.OrderPoints(problem.TargetPoints);
                NearestNeighborPathfinder.DumpPointsToFile(ordered, $"{problem.Name}-ordered.txt");

                BFSSolver solver = new BFSSolver();
                var solution = solver.Solve(NearestNeighborPathfinder.OrderPoints(problem.TargetPoints), 4, 20, true);
                if (solution == null)
                {
                    Console.WriteLine($"No solution found for problem {problem.Name}");
                    continue;
                }

                WriteSolutionToFile(problem, solution, $"{problem.Name}.json");

                if (solution.Count > 1000000)
                {
                    continue;
                }

                string submissionString = $"solve {problem.Name} " + string.Join("", solution); //string.Join(" ", solution.Select(x => Encodings.EncodeMachineInt(x)));
                var result = SolutionSubmitter.submitSoluton("spaceship", problem.Name, solution.Count, "S" + Encodings.EncodeMachineString(submissionString), new Dictionary<string, string>());
                //Console.WriteLine(result);
            }
        }

        static void WriteSolutionToFile(SpaceshipProblem problem, List<int> solution, string filename)
        {
            var data = new
            {
                ProblemName = problem.Name,
                StartPoint = new { X = 0, Y = 0 },
                TargetPoints = problem.TargetPoints.Select(p => new { X = p.X, Y = p.Y }).ToList(),
                Solution = solution
            };

            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, jsonString);

            Console.WriteLine($"Solution written to {filename}");
        }
    }
}
