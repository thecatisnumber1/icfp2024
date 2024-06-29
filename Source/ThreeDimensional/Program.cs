using Core;

namespace ThreeDimensional
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            ProgramGrid pg = ProgramGrid.Parse(@". . . . 0 . . . .
. B > . = . . . .
. v 1 . . > . . .
. . - . . . + S .
. . . . . ^ . . .
. . v . . 0 > . .
. . . . . . A + .
. 1 @ 6 . . < . .
. . 3 . 0 @ 3 . .
. . . . . 3 . . .", 10, 11);
            */

            // p1 solution
            /*
            ProgramGrid pg = ProgramGrid.Parse(
@". . A > . > . > .
. 1 . 0 . . v . v
A - . # . . . . .
. . . . > . * . v
0 # . . . . . . .
. . v . . 4 @ 5 v
. . . > . . 4 . S
. . . 4 @ 5 . . .
. . . . 4 . . . .", 4);*/
            /*
            ProgramGrid pg = ProgramGrid.Parse(
@". 1 . 0 . < > . 1 . 0 . < > . .
A - . # v . . A + . # v . . . .
. . . 1 . . . . . . 1 . . . . .
0 = . + 5 @ 2 0 = . - 5 @ 2 . .
. . . S . 3 . . . . S . 3 . . .", 4);
            */

            string s = @". 1 . 0 . . 1 . 0 . . A .
A - . = . A + . = . 0 = .
. . . . v . . . . v . S .
0 # . . . 0 # . . . . . .
. . v 0 @ -2 . v 0 @ -2 . .
. . . . 3 . . . . 3 . . .
. . v . . . . v . . . A .
. . . 0 = . . . 0 = . - .
. 2 @ 7 . . 2 @ 7 . . S .
. . 4 A + S . 4 . . - . .
. . . . . . . . . . . . .";
            string problemName = "3d2";

            s = s.Replace("\r", "");
            ProgramGrid pg = ProgramGrid.Parse(s, -4);

            ProgramRunner runner = new ProgramRunner(pg);
            Console.WriteLine(runner);
            while (runner.Step())
            {
                Console.WriteLine(runner);
            }

            Console.WriteLine(runner.GetResult());
            string submissionString = $"solve {problemName}\n" + s;
            var result = SolutionSubmitter.submitSoluton("spaceship", problemName, -1, "S" + Encodings.EncodeMachineString(submissionString), new Dictionary<string, string>());
        }
    }
}
