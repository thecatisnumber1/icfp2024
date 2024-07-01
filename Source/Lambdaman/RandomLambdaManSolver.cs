using Core;
using LambdaMan;
using Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Core.Shorthand;
using static System.Runtime.InteropServices.JavaScript.JSType;

using MovesMaker = System.Func<LambdaMan.LambdaManGrid, long, string>;
using ExprMaker = System.Func<LambdaMan.LambdaManGrid, long, string, Core.Expression>;

namespace Lambdaman;

public class RandomLambdaManSolver
{
    private const long SEED_A = 48271; //16807;
    private const long SEED_M = 2147483647;

    private static readonly List<(MovesMaker, ExprMaker)> RAND_SOLVERS = [
        (RandMovesSimple, ExprSimple),
        (RandMovesOneStepNoBackwards, ExprOneStepNoBackwards),
        //(RandMovesTwoStepsTurnsOnly, ExprTwoStepsTurnsOnly),
    ];

    public static Expression? Solve(LambdaManGrid problem)
    {
        ConcurrentBag<(long, string, Expression)> solved = [];

        Console.WriteLine(problem.Name);
        Console.WriteLine("Total Pills: " + problem.Pills.Count);

        // Try all solvers
        foreach (var (movesMaker, exprMaker) in RAND_SOLVERS)
        {
            // Try all 2-digit seeds (base 94), except 0 which is degenerate
            Parallel.For(1, 94 * 94, (seed, state) =>
            {
                string randMoves = movesMaker(problem, seed);
                var (pills, steps) = Simulate(randMoves, problem);

                if (pills == 0)
                {
                    string solution = randMoves[..steps];
                    var expr = exprMaker(problem, seed, solution);

                    solved.Add((seed, randMoves, expr));
                    state.Stop();
                }
            });
        }

        if (solved.IsEmpty)
        {
            // No solutions
            return null;
        }

        // Find best solution by expression length
        var (bestSeed, bestSolution, bestExpr) = solved.MinBy(sp => (sp.Item3.ToICFP(), sp.Item2.Length));

        Console.WriteLine("Best seed: " + bestSeed);
        Console.WriteLine("Best steps: " + bestSolution.Length);
        Console.WriteLine("Best expr length: " + bestExpr.ToICFP().Length);

        //Simulate(solverOutput, origGrid, true);
        //File.WriteAllText(Finder.GIT.GetRelativeFile("OUTPUT_MINE.txt").FullName, bestSolution);

        return bestExpr;
    }

    private static int MaxSolutionLength(LambdaManGrid problem)
    {
        return 1000000 - problem.SolvePrefix().AsString().Length;
    }

    private static string RandMovesOneStepNoBackwards(LambdaManGrid problem, long seed)
    {
        StringBuilder solution = new();
        char lastDir = 'R';
        int maxLength = MaxSolutionLength(problem);

        while (solution.Length <= maxLength)
        {
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

        return solution.ToString();
    }

    private static Expression ExprOneStepNoBackwards(LambdaManGrid problem, long bestSeed, string solution)
    {
        var factorial = V("f");
        var vi = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);

        var prev = V("p");
        var startPrev = S("R");
        var end = I(solution.Length - 1);
        var func = RecursiveFunc(factorial, vi, prev, seed)(
            If(vi < I(0),
                S(""),
                Concat(
                    Take(I(1), Drop(seed % I(3), Concat(prev, If((prev == S("D") | prev == S("U")), S("LR"), S("UD"))))),
                    RecursiveCall(factorial, vi - I(1),
                        Take(I(1), Drop(seed % I(3), Concat(prev, If((prev == S("D") | prev == S("U")), S("LR"), S("UD"))))),
                        (seed * seedA) % seedM)
                )
            )
        );

        return Apply(Apply(Apply(func, end), startPrev), startSeed);
    }

    private static string RandMovesTwoStepsTurnsOnly(LambdaManGrid problem, long seed)
    {
        // TODO
        return null;
    }

    private static Expression ExprTwoStepsTurnsOnly(LambdaManGrid problem, long bestSeed, string solution)
    {
        // Two steps, left/ right turns only
        var factorial = V("f");
        var vi = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);

        var offset = V("o");
        var startOffset = I(0);
        var end = I(solution.Length / 2 + 1);
        var func = RecursiveFunc(factorial, vi, offset, seed)(
            If(vi < I(0),
                S(""),
                Concat(
                    Take(I(2), Drop(((seed % I(2)) * I(2)) + offset, S("UUDDLLRR"))),
                    RecursiveCall(factorial, vi - I(1), (offset + I(4)) % I(8), (seed * seedA) % seedM)
                )
            )
        );

        return Apply(Apply(Apply(func, end), offset), startSeed);
    }

    private static string RandMovesSimple(LambdaManGrid problem, long seed)
    {
        StringBuilder solution = new();
        int maxLength = MaxSolutionLength(problem);

        for (int i = 0; i < maxLength; i++)
        {
            char dir = "UDLR"[(int)(seed % 4)];
            seed = seed * SEED_A % SEED_M;
            solution.Append(dir);
        }

        return solution.ToString();
    }

    private static Expression ExprSimple(LambdaManGrid problem, long bestSeed, string solution)
    {
        // Fully random one-step moves
        var factorial = V("f");
        var vi = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);
        var end = I(solution.Length - 1);

        var func = RecursiveFunc(factorial, vi, seed)(
            If(vi < I(0),
                S(""),
                Concat(
                    Take(I(1), Drop(seed % I(4), S("UDLR"))),
                    RecursiveCall(factorial, vi - I(1), (seed * seedA) % seedM)
                )
            )
        );

        return Apply(Apply(func, end), startSeed);
    }

    private static (int, int) Simulate(string solution, LambdaManGrid problem, bool display = false)
    {
        char[,] grid = (char[,])problem.Grid.Clone();

        int w = problem.Width;
        int h = problem.Height;
        int x = problem.StartPosition.X;
        int y = problem.StartPosition.Y;
        int pills = problem.Pills.Count;
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
