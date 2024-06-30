using Core;
using Lambdaman;
using Lib;
using static Core.Shorthand;

namespace LambdaMan
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SolveAllUsingRandom();
        }

        static void SolveAllUsingRandom()
        {
            SolveAll(problem => RandomLambdaManSolver.Solve(problem));
        }

        static void SolveAllUsingRawPaths()
        {
            SolveAll(problem => S(LambdaManSolver.Solve(problem)));
        }

        static void SolveAll(Func<LambdaManGrid, Expression?> solver)
        {
            var problems = ProblemLoader.LoadProblems();

            foreach (var problem in problems)
            {
                // Solve
                var expr = solver(problem.Duplicate());

                if (expr is null)
                {
                    continue;
                }

                // Submit solution
                Dictionary<string, string> meta = [];
                Expression solveExpr;

                if (expr is Str s)
                {
                    solveExpr = S($"solve {problem.Name} {s.AsString()}");
                }
                else
                {
                    solveExpr = Concat(S($"solve {problem.Name} "), expr);
                    meta["programmed"] = "true";
                }

                string icfpSolution = solveExpr.ToICFP();
                Console.WriteLine($"{problem.Name} solution = {icfpSolution}");

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

        static void AnimateSolution(LambdaManGrid grid, string solution)
        {
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
                    //Thread.Sleep(10);

                    // Clear the previous Lambda-Man position
                    grid.SetCell(currentPos.X, currentPos.Y, ' ');
                }
            }
        }
    }
}
