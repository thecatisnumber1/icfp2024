using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spaceship
{
    public class SpaceshipProblem
    {
        public string Name { get; set; }
        public List<Point> TargetPoints { get; set; }

        public SpaceshipProblem(string name, List<Point> targetPoints)
        {
            Name = name;
            TargetPoints = targetPoints;
        }
    }
}
