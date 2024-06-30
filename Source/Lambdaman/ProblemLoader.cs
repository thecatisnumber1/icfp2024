using Lib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LambdaMan;

public class ProblemLoader
{
    public static List<LambdaManGrid> LoadProblems()
    {
        var dir = Finder.GIT.FindDir("Tasks");
        dir = dir.GetDirectories("lambdaman").FirstOrDefault();
        var problems = new List<LambdaManGrid>();

        foreach (var file in dir.GetFiles("*decoded*"))
        {
            problems.Add(LoadProblem(file));
        }

        return problems;
    }

    public static LambdaManGrid LoadProblem(FileInfo file)
    {
        string name = file.Name.Replace("-decoded.txt", null);
        List<string> lines = new(File.ReadAllLines(file.FullName));

        return new (name, lines);
    }
}
