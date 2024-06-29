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

            ProgramGrid pg = ProgramGrid.Parse(
@". 1 . 0 . . 1 . 0 . .
A - . = . A + . = . .
. . . -1 @ -4 . . 1 @ -5
0 # . . 2 0 # . . 2 .
. . v . . . . v . . .
. . . . . . . . . . .
. 2 @ 5 . . 2 @ 5 . .
. . 3 . 0 = . 3 . . A
. . . . . . . 0 = . -
. . . . A + S . . . S", 4);

            ProgramRunner runner = new ProgramRunner(pg);
            Console.WriteLine(runner);
            while (runner.Step())
            {
                Console.WriteLine(runner);
            }

            Console.WriteLine(runner.GetResult());
        }
    }
}
