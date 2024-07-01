using Core;
using LambdaMan;
using Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Core.Shorthand;

namespace Lambdaman;

public class RandomLambdaManSolver
{
    private const long SEED_A = 48271; //16807;
    private const long SEED_M = 2147483647;

    public static Expression? Solve(LambdaManGrid origGrid)
    {
        ConcurrentBag<(long[], int, int)> dataPills = [];

        Console.WriteLine(origGrid.Name);
        Console.WriteLine("Total Pills: " + origGrid.Pills.Count);

        // Try all 2-digit seeds (base 94)
        Parallel.For(0, 94 * 94, (i, state) =>
        {
            var solverOutput = TrySolve([i]);
            var (pills, steps) = Simulate(solverOutput, origGrid);

            dataPills.Add((solverOutput.Item2, pills, steps));

            if (dataPills.Count % 1000 == 0)
            {
                Console.WriteLine(dataPills.Count);
            }

            if (pills == 0)
            {
                state.Stop();
            }
        });

        var (data, pills, steps) = dataPills.MinBy(sp => (sp.Item2, sp.Item3));

        Console.WriteLine("Min pills: " + pills);
        Console.WriteLine("Best data: " + string.Join(' ', data));
        Console.WriteLine("Best steps: " + steps);

        var solverOutput = TrySolve(data);
        long bestSeed = data[0];
        //Simulate(solverOutput, origGrid, true);

        if (pills > 0)
        {
            // Pills left over, so no solution
            return null;
        }

        string solution = solverOutput.Item1[..steps];
        File.WriteAllText(Finder.GIT.GetRelativeFile("OUTPUT_MINE.txt").FullName, solution);

        // Create an ICFP function that is equivalent to TrySolve
        var factorial = V("f");
        var i = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);

        // Two steps, left/right turns only
        //var offset = V("o");
        //var startOffset = I(0);
        //var end = I(solution.Length / 2);
        //var func = RecursiveFunc(factorial, i, offset, seed)(
        //    If(i < I(0),
        //        S(""),
        //        Concat(
        //            Take(I(2), Drop(((seed % I(2)) * I(2)) + offset, S("UUDDLLRR"))),
        //            RecursiveCall(factorial, i - I(1), (offset + I(4)) % I(8), (seed * seedA) % seedM)
        //        )
        //    )
        //);

        // One step, no backwards
        var prev = V("p");
        var startPrev = S("R");
        var end = I(solution.Length - 1);
        var func = RecursiveFunc(factorial, i, prev, seed)(
            If(i < I(0),
                S(""),
                Concat(
                    Take(I(1), Drop(seed % I(3), Concat(prev, If((prev == S("D") | prev == S("U")), S("LR"), S("UD"))))),
                    RecursiveCall(factorial, i - I(1),
                        Take(I(1), Drop(seed % I(3), Concat(prev, If((prev == S("D") | prev == S("U")), S("LR"), S("UD"))))),
                        (seed * seedA) % seedM)
                )
            )
        );

        return Apply(Apply(Apply(func, end), startPrev), startSeed);
    }

    private static (string, long[]) TrySolve(long[] data)
    {
        StringBuilder solution = new();
        long seed = data[0];
        char lastDir = 'R';

        while (solution.Length <= 1000000 - "solve lambdaman00 ".Length)
        {
            //char dir = "UDLR"[(int)(seed % 4)];
            char dir;

            if (lastDir == 'U' || lastDir == 'D')
            {
                dir = (lastDir + "LR")[(int)(seed % 3)];
            }
            else
            {
                dir = (lastDir + "UD")[(int)(seed % 3)];
            }

            seed = seed * SEED_A % SEED_M;
            solution.Append(dir);
            lastDir = dir;
        }

        return (solution.ToString(), data);
    }

    private static (int, int) Simulate((string, long[]) solverOutput, LambdaManGrid origGrid, bool display = false)
    {
        var (solution, _) = solverOutput;
        char[,] grid = (char[,])origGrid.Grid.Clone();

        int w = origGrid.Width;
        int h = origGrid.Height;
        int x = origGrid.StartPosition.X;
        int y = origGrid.StartPosition.Y;
        int pills = origGrid.Pills.Count;
        int i;

        for (i = 0; i < solution.Length && pills > 0; i++)
        {
            int nx = x;
            int ny = y;

            switch (solution[i])
            {
                case 'D':
                    ny++;
                    break;
                case 'R':
                    nx++;
                    break;
                case 'U':
                    ny--;
                    break;
                case 'L':
                    nx--;
                    break;
            }

            // Ignore invalid moves
            if (nx >= 0 && ny >= 0 && nx < w && ny < h && grid[ny, nx] != '#')
            {
                x = nx;
                y = ny;

                if (grid[y, x] == '.')
                {
                    grid[y, x] = ' ';
                    pills--;
                }
            }

            if (display && (i == solution.Length - 1))
            {
                for (int gy = 0; gy < h; gy++)
                {
                    for (int gx = 0; gx < w; gx++)
                    {
                        Console.Write(grid[gy, gx]);
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("Pills: " + pills);
                Console.WriteLine();
            }
        }

        return (pills, i);
    }
}
