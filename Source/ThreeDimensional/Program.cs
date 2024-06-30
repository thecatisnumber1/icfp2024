using Core;
using Lib;

namespace ThreeDimensional
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string problemName = "3d9";
            string s = ReadProblem(problemName);

            RegularizeFile($"{problemName}.3d");
            //RunWithVisualization(s, 22);
            
            ProgramGrid submission = ProgramGrid.Parse(s);
            string submissionString = $"solve {problemName}\n" + submission.ToMinimalString();
            var result = SolutionSubmitter.submitSoluton("3d", problemName, -1, "S" + Encodings.EncodeMachineString(submissionString), new Dictionary<string, string>());
            Console.WriteLine($"score = {result.score}");
        }


        static void RunWithVisualization(string s, int? a = null, int? b = null)
        {
            ProgramGrid pg = ProgramGrid.Parse(s, a, b);
            ProgramRunner runner = new ProgramRunner(pg);
            Console.WriteLine(pg.MetadataToString());
            Console.WriteLine(runner);
            Console.ReadLine();
            while (runner.Step())
            {
                Console.WriteLine(runner);
                Console.ReadLine();
            }

            Console.WriteLine(runner.GetResult());
        }

        static string ReadProblem(string problemName)
        {
            string s = File.ReadAllText(
                Finder.SOLUTION
                .FindDir("ThreeDimensional")
                .GetDirectories("Programs")
                .FirstOrDefault()
                .GetFiles($"{problemName}.3d")
                .FirstOrDefault()
                .FullName);
            return s.Replace("\r", "");
        }

        static void RegularizeFile(string pattern)
        {
            foreach (var f in Finder.SOLUTION.FindDir("ThreeDimensional").GetDirectories("Programs").FirstOrDefault().GetFiles(pattern))
            {
                Console.WriteLine(f.FullName);
                GridRegularizer.RegularizeGridFile(f.FullName);
            }
        }
    }
}
