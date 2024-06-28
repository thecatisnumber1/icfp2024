using Lib;
using System.Runtime.Serialization.Formatters;
using System.Text.Json;

namespace Spaceship
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var allProblems = ProblemLoader.LoadProblems();

            for (int i = 0; i < allProblems.Count; i++)
            {
                var problem = allProblems[i];
                SpaceshipSolver solver = new SpaceshipSolver(TimeSpan.FromSeconds(5));
                List<int> solution = solver.Solve(problem);
                PathSimulator.SimulateAndPrintPath(solution);
                WriteSolutionToFile(problem, solution, $"solution_{i}.json");
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
