using Lib;
using Core;
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
                if (!problem.Name.Contains("1"))
                {
                    //continue;
                }

                //NearestNeighborSolver solver = new NearestNeighborSolver();
                List<int> solution = HardCodedFlier(problem);
                if (solution == null)
                {
                    continue;
                }

                WriteSolutionToFile(problem, solution, $"{problem.Name}.json");

                if (solution.Count > 1000000)
                {
                    continue;
                }

                string submissionString = $"solve {problem.Name} " + string.Join("", solution); //string.Join(" ", solution.Select(x => Encodings.EncodeMachineInt(x)));
                var result = SolutionSubmitter.submitSoluton("spaceship", problem.Name, solution.Count, "S" + Encodings.EncodeMachineString(submissionString), new Dictionary<string, string>());
            }
        }

        private static Dictionary<int, (int dx, int dy)> velocities = new Dictionary<int, (int dx, int dy)>
        {
            { 1, (-1, -1) },
            { 2, (0, -1) },
            { 3, (1, -1) },
            { 4, (-1, 0) },
            { 5, (0, 0) },  // Typically, command 5 represents no movement
            { 6, (1, 0) },
            { 7, (-1, 1) },
            { 8, (0, 1) },
            { 9, (1, 1) }
        };
        static List<int> HardCodedFlier(SpaceshipProblem problem)
        {
            Ship ship = new Ship();
            List<int> solution = new List<int>();

            while (problem.TargetPoints.Count > 0)
            {
                bool moved = false;

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
                            moved = true;
                            break;
                        }
                    }
                    if (moved)
                    {
                        break;
                    }
                }

                if (!moved)
                {
                    return null;
                }
            }

            return solution;
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
