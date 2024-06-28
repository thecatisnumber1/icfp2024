using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Spaceship
{
    public class ProblemLoader
    {
        public static List<SpaceshipProblem> LoadProblems()
        {
            var dir = Finder.GIT.FindDir("Tasks");
            dir = dir.GetDirectories("spaceship").FirstOrDefault();
            var problems = new List<SpaceshipProblem>();

            foreach (var file in dir.GetFiles("*decoded*"))
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var targetPoints = new List<Point>();

                foreach (var line in File.ReadAllLines(file.FullName))
                {
                    var coordinates = line.Split(' ');
                    if (coordinates.Length == 2 &&
                        int.TryParse(coordinates[0], out int x) &&
                        int.TryParse(coordinates[1], out int y))
                    {
                        targetPoints.Add(new Point(x, y));
                    }
                }

                problems.Add(new SpaceshipProblem(name, targetPoints));
            }

            return problems;
        }
    }
}
