using Core;
using LambdaMan;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Core.Shorthand;

namespace Lambdaman;

public class RandomLambdaManSolver
{
    private const long SEED_A = 16807;
    private const long SEED_M = 2147483647;

    public static Expression? Solve(LambdaManGrid origGrid)
    {
        ConcurrentBag<(long[], int, int)> dataPills = [];

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

        if (pills > 0)
        {
            // Pills left over, so no solution
            return null;
        }

        var solverOutput = TrySolve(data, true);
        long bestSeed = data[0];
        string solution = solverOutput.Item1[..steps];
        //Console.WriteLine(solution);

        //Simulate(solverOutput, origGrid, true);

        // Create an ICFP function that is equivalent to TrySolve
        var factorial = V("f");
        var i = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);
        var end = I(solution.Length - 1);

        var func = RecursiveFunc(factorial, i, seed)(
            If(i < I(0),
                S(""),
                Concat(
                    Take(I(1), Drop(seed % I(4), S("UDLR"))),
                    RecursiveCall(factorial, i - I(1), (seed * seedA) % seedM)
                )
            )
        );

        return Apply(Apply(func, end), startSeed);
    }

    private static (string, long[]) TrySolve(long[] data, bool vis = false)
    {
        StringBuilder solution = new();
        long seed = data[0];

        for (int i = 0; i < 1000000 - 17; i++)
        {
            char dir = "UDLR"[(int)(seed % 4)];
            seed = seed * SEED_A % SEED_M;
            solution.Append(dir);
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
