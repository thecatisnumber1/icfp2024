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

            ProgramGrid pg = ProgramGrid.Parse(@". . A > . > . > .
. 1 . 0 . . v . v
A - . # . . . . .
. . . . > . * . v
0 # . . . . . . S
. . v . . 4 @ 5 .
. . . > . . 4 . .
. . . 4 @ 5 . . .
. . . . 4 . . . .", 4);

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
