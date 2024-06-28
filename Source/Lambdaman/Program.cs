using Core;

namespace LambdaMan
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var problems = ProblemLoader.LoadProblems();
            foreach (var problem in problems)
            {
                string result = LambdaManSolver.Solve(problem.Duplicate());
                string submissionString = $"solve {problem.Name} " + result;
                Console.WriteLine($"{problem.Name} solution = {result}");
                //var res = 
                //    SolutionSubmitter.submitSoluton(
                //        "lambdaman", 
                //        problem.Name, 
                //        result.Length, 
                //        "S" + Encodings.EncodeMachineString(submissionString), 
                //        new Dictionary<string, string>());

                //Console.WriteLine($"{problem.Name} score = {res.score}");
            }
        }

        static void AnimateSolution(LambdaManGrid grid, string solution)
        {
            var currentPos = grid.StartPosition;

            foreach (var move in solution)
            {
                switch (move)
                {
                    case 'D':
                        currentPos = (currentPos.X, currentPos.Y + 1);
                        break;
                    case 'R':
                        currentPos = (currentPos.X + 1, currentPos.Y);
                        break;
                    case 'U':
                        currentPos = (currentPos.X, currentPos.Y - 1);
                        break;
                    case 'L':
                        currentPos = (currentPos.X - 1, currentPos.Y);
                        break;
                }

                grid.SetCell(currentPos.X, currentPos.Y, 'L');
                grid.Display();
                Thread.Sleep(200);

                // Clear the previous Lambda-Man position
                grid.SetCell(currentPos.X, currentPos.Y, ' ');
            }
        }
    }
}
