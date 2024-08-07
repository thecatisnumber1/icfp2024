﻿using ConsoleRunner;
using Core;
using LambdaMan;
using Lib;
using System.Collections.Concurrent;
using System.Text;
using static Core.Shorthand;
using ExprMaker = System.Func<LambdaMan.LambdaManGrid, long, string, Core.Expression>;
using Solver = System.Func<LambdaMan.LambdaManGrid, long, (int, string)>;

namespace Lambdaman;

public class RandomLambdaManSolver
{
    public record struct RandomOpts(int StartSeed, int EndSeed) { }

    private const long SEED_A = 48271; //16807;
    private const long SEED_M = 2147483647;

    private static readonly List<(Solver, ExprMaker)> RAND_SOLVERS = [
        (RandMovesSimple, ExprSimple),
        (RandMovesOneStepNoBackwards, ExprOneStepNoBackwards),
        (RandMovesTwoStepsTurnsOnly, ExprTwoStepsTurnsOnly),
    ];

    public static Expression? Solve(LambdaManGrid problem, RandomOpts opts)
    {
        ConcurrentBag<(long, string, Expression)> solved = [];

        // Try all solvers
        foreach (var (solver, exprMaker) in RAND_SOLVERS)
        {
            // Try the specified range of seeds
            Parallel.For(opts.StartSeed, opts.EndSeed + 1, (seed, state) =>
            {
                var (pills, solution) = solver(problem, seed);

                if (pills == 0)
                {
                    var expr = exprMaker(problem, seed, solution);

                    solved.Add((seed, solution, expr));
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

        // Validate that evaluating the expression matches what we expect
        string? evalSolution = Communicator.Eval(bestExpr.ToICFP());

        if (evalSolution != bestSolution)
        {
            Console.WriteLine("~~~EVAL MISMATCH!!! Check equivalency~~~");
            File.WriteAllText(Finder.GIT.GetRelativeFile("OUTPUT_MINE.txt").FullName, bestSolution);
            File.WriteAllText(Finder.GIT.GetRelativeFile("OUTPUT_THEIRS.txt").FullName, evalSolution);

            return null;
        }

        return bestExpr;
    }

    private static int MaxSolutionLength(LambdaManGrid problem)
    {
        return 1000000 - problem.SolvePrefix().AsString().Length;
    }

    private static (int, string) RandMovesOneStepNoBackwards(LambdaManGrid problem, long seed)
    {
        StringBuilder moves = new();
        char lastDir = 'R';
        int maxLength = MaxSolutionLength(problem);

        while (moves.Length <= maxLength)
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
            moves.Append(dir);
            lastDir = dir;
        }

        string movesStr = moves.ToString();
        var (pills, steps) = Simulate(movesStr, problem);
        string solution = movesStr[..steps];

        return (pills, solution);
    }

    private static Expression ExprOneStepNoBackwards(LambdaManGrid problem, long bestSeed, string solution)
    {
        var f = V("f");
        var vi = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);

        var prev = V("p");
        var startPrev = S("R");
        var end = I(solution.Length - 1);
        var func = RecursiveFunc(f, vi, prev, seed)(
            If(vi < I(0),
                S(""),
                Concat(
                    Take(I(1), Drop(seed % I(3), Concat(prev, If((prev == S("D") | prev == S("U")), S("LR"), S("UD"))))),
                    RecursiveCall(f, vi - I(1),
                        Take(I(1), Drop(seed % I(3), Concat(prev, If((prev == S("D") | prev == S("U")), S("LR"), S("UD"))))),
                        (seed * seedA) % seedM)
                )
            )
        );

        return Apply(Apply(Apply(func, end), startPrev), startSeed);
    }

    private static (int, string) RandMovesTwoStepsTurnsOnly(LambdaManGrid problem, long seed)
    {
        StringBuilder moves = new();
        int maxLength = MaxSolutionLength(problem);
        int offset = 0;

        while (moves.Length <= maxLength)
        {
            string dir = "UUDDLLRR"[(((int)(seed % 2) * 2) + offset)..][..2];

            seed = seed * SEED_A % SEED_M;
            moves.Append(dir);
            offset = (offset + 4) % 8;
        }

        string movesStr = moves.ToString();
        var (pills, steps) = Simulate(movesStr, problem);
        // Ensure even number of steps
        string solution = movesStr[..(steps + steps % 2)];

        return (pills, solution);
    }

    private static Expression ExprTwoStepsTurnsOnly(LambdaManGrid problem, long bestSeed, string solution)
    {
        // Two steps, left/ right turns only
        var f = V("f");
        var vi = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);

        var offset = V("o");
        var startOffset = I(0);
        var end = I(solution.Length / 2);
        var func = RecursiveFunc(f, vi, offset, seed)(
            If(vi == I(0),
                S(""),
                Concat(
                    Take(I(2), Drop(((seed % I(2)) * I(2)) + offset, S("UUDDLLRR"))),
                    RecursiveCall(f, vi - I(1), (offset + I(4)) % I(8), (seed * seedA) % seedM)
                )
            )
        );

        return Apply(Apply(Apply(func, end), startOffset), startSeed);
    }

    private static (int, string) RandMovesSimple(LambdaManGrid problem, long seed)
    {
        StringBuilder moves = new();
        int maxLength = MaxSolutionLength(problem);

        for (int i = 0; i < maxLength; i++)
        {
            char dir = "UDLR"[(int)(seed % 4)];
            seed = seed * SEED_A % SEED_M;
            moves.Append(dir);
        }

        string movesStr = moves.ToString();
        var (pills, steps) = Simulate(movesStr, problem);
        string solution = movesStr[..steps];

        return (pills, solution);
    }

    private static Expression ExprSimple(LambdaManGrid problem, long bestSeed, string solution)
    {
        // Fully random one-step moves
        var f = V("f");
        var vi = V("i");
        var seed = V("s");

        var seedA = I(SEED_A);
        var seedM = I(SEED_M);
        var startSeed = I(bestSeed);
        var end = I(solution.Length - 1);

        var func = RecursiveFunc(f, vi, seed)(
            If(vi < I(0),
                S(""),
                Concat(
                    Take(I(1), Drop(seed % I(4), S("UDLR"))),
                    RecursiveCall(f, vi - I(1), (seed * seedA) % seedM)
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
