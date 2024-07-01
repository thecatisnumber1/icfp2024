using Core;
using Lambdaman;
using Lib;
using static Core.Shorthand;
using static Lambdaman.RandomLambdaManSolver;

namespace LambdaMan
{
    internal class Program
    {
        const bool SUBMIT = true;

        static void Main(string[] args)
        {
            RandomOpts randopts;

            if (args.Length == 2)
            {
                randopts = new(int.Parse(args[0]), int.Parse(args[1]));
            }
            else
            {
                randopts = new(1, (94 * 94) - 1);
            }

            SolveAll(problem => problem.Number switch
            {
                1 => HandwrittenSolvers.Lambdaman1(problem), // Tiny
                2 => HandwrittenSolvers.Lambdaman2(problem), // Tiny
                3 => HandwrittenSolvers.Lambdaman3(problem), // Tiny
                4 => RandomLambdaManSolver.Solve(problem, randopts), // Small maze
                5 => RandomLambdaManSolver.Solve(problem, randopts), // Small circular spiral
                6 => HandwrittenSolvers.Lambdaman6(problem), // Straight right
                7 => RandomLambdaManSolver.Solve(problem, randopts), // Pacman level 1
                8 => HandwrittenSolvers.Lambdaman8(problem), // Big rectangular spiral
                9 => null, // Medium open space
                10 => RandomLambdaManSolver.Solve(problem, randopts), // Open space with regular blocks
                <= 15 => RandomLambdaManSolver.Solve(problem, randopts), // Large mazes
                16 => RandomLambdaManSolver.Solve(problem, randopts), // Hilbert curve
                17 => RandomLambdaManSolver.Solve(problem, randopts), // Medium space with long shafts
                18 => RandomLambdaManSolver.Solve(problem, randopts), // Huge space with long shafts
                19 => RandomLambdaManSolver.Solve(problem, randopts), // Large diamond fractal
                20 => RandomLambdaManSolver.Solve(problem, randopts), // Large  diamond fractal
                21 => RandomLambdaManSolver.Solve(problem, randopts), // Open space with "3D"
                _ => throw new ArgumentException("Unknown problem: " + problem.Number)
            });
        }

        static void SolveAll(Func<LambdaManGrid, Expression?> solver)
        {
            Scoreboard scoreboard = Scoreboard.Fetch();
            var problems = ProblemLoader.LoadProblems();

            foreach (var problem in problems.OrderBy(p => p.Number))
            {
                Console.WriteLine();
                Console.WriteLine(problem.Name);
                Console.WriteLine("Total Pills: " + problem.Pills.Count);

                // Solve
                var expr = solver(problem.Duplicate());

                if (expr is null)
                {
                    continue;
                }

                // Submit solution
                Dictionary<string, string> meta = [];

                if (expr is not Str)
                {
                    meta["programmed"] = "true";
                }

                Expression solveExpr;
                Str solvePrefix = problem.SolvePrefix();
                string solvePrefixIcfp = solvePrefix.ToICFP();

                // If the solution does not contain the solve prefix, then prepend it
                if (!expr.ToICFP().Contains(solvePrefixIcfp))
                {
                    if (expr is Str s)
                    {
                        // If the solution is just a string, just add it to the string
                        solveExpr = S($"{solvePrefix.AsString()}{s.AsString()}");
                    }
                    else
                    {
                        // Otherwise we need to use an ICFP concat
                        solveExpr = Concat(solvePrefix, expr);
                        
                    }
                }
                else
                {
                    // Solution contains the solve prefix, so just use it directly
                    solveExpr = expr;
                }

                string icfpSolution = solveExpr.ToICFP();
                int expectedScore = icfpSolution.Length;
                Console.WriteLine($"{problem.Name} solution = {icfpSolution}");
                Console.WriteLine($"Expected score: {expectedScore}");
                Console.WriteLine($"Our best score: {scoreboard[problem.Name]}");

                if (!SUBMIT)
                {
                    continue;
                }

                if (expectedScore >= scoreboard[problem.Name])
                {
                    if (expectedScore != scoreboard[problem.Name])
                    {
                        Console.WriteLine($"Not submitting solution worse than best. Try harder.");
                    }
                    continue;
                }

                var res = SolutionSubmitter.submitSoluton(
                        "lambdaman",
                        problem.Name,
                        icfpSolution.Length,
                        icfpSolution,
                        meta);

                Console.WriteLine(res.status);
                Console.WriteLine($"{problem.Name} score = {res.score}");
                Console.WriteLine();
            }
        }

        public static void AnimateSolution(LambdaManGrid grid, string solution)
        {
            Console.WindowWidth = grid.Width;
            Console.WindowHeight = Console.LargestWindowHeight;
            grid.Display();
            Console.CursorVisible = false;

            var currentPos = grid.StartPosition;

            foreach (var move in solution)
            {
                var nextPos = currentPos;

                switch (move)
                {
                    case 'D':
                        nextPos = (currentPos.X, currentPos.Y + 1);
                        break;
                    case 'R':
                        nextPos = (currentPos.X + 1, currentPos.Y);
                        break;
                    case 'U':
                        nextPos = (currentPos.X, currentPos.Y - 1);
                        break;
                    case 'L':
                        nextPos = (currentPos.X - 1, currentPos.Y);
                        break;
                }

                // Ignore invalid moves
                if (!grid.IsWall(nextPos.X, nextPos.Y))
                {
                    Console.Write(" ");

                    currentPos = nextPos;

                    Console.SetCursorPosition(currentPos.X, currentPos.Y);
                    Console.Write("L");
                    Console.SetCursorPosition(currentPos.X, currentPos.Y);

                    grid.SetCell(currentPos.X, currentPos.Y, 'L');
                    Thread.Sleep(500);

                    // Clear the previous Lambda-Man position
                    grid.SetCell(currentPos.X, currentPos.Y, ' ');
                }
            }
        }
    }
}
