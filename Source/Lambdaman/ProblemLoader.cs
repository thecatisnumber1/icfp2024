using Lib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace LambdaMan
{
    public class ProblemLoader
    {
        public static List<LambdaManGrid> LoadProblems()
        {
            var dir = Finder.GIT.FindDir("Tasks");
            dir = dir.GetDirectories("lambdaman").FirstOrDefault();
            var problems = new List<LambdaManGrid>();

            foreach (var file in dir.GetFiles("*decoded*"))
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                problems.Add(new LambdaManGrid(name.Replace("-decoded", null), new List<string>(File.ReadAllLines(file.FullName))));
            }

            return problems;
        }
    }
}
