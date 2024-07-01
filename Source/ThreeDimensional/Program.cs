using Core;
using Lib;
using ConsoleRunner;

namespace ThreeDimensional
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string problemName = "3d8";
            string s = ReadProblem(problemName);

            //RegularizeFile($"{problemName}.3d");
            //RunWithVisualization(s, 25);

            //Test3d8(s);
            CallTest();

            //ProgramGrid submission = ProgramGrid.Parse(s);
            //string submissionString = $"solve {problemName}\n" + submission.ToMinimalString();
            //var result = SolutionSubmitter.submitSoluton("3d", problemName, -1, "S" + Encodings.EncodeMachineString(submissionString), new Dictionary<string, string>());
            //Console.WriteLine($"score = {result.score}");
        }

        private static void Test3d8(string s)
        {
            for (int i = 2; i <= 10000; i++)
            {
                int smallestBase = FindSmallestPalindromeBase(i);
                ProgramGrid pg = ProgramGrid.Parse(s, i);
                ProgramRunner runner = new ProgramRunner(pg);
                runner.RunToCompletion();
                if (runner.GetResult() != smallestBase)
                {
                    Console.WriteLine($"Failed for {i} with {runner.GetResult()} instead of {smallestBase}");
                }
                else
                {
                    Console.WriteLine($"Passed for {i}");
                }
            }
        }

        static void CallTest()
        {
            string problemName = "3d8";
            string s = ReadProblem(problemName);
            ProgramGrid submission = ProgramGrid.Parse(s);
            string input = submission.ToMinimalString();
            input = input.Replace("\r", "");
            string submissionString = $"test 3d 1 2\n" + input;
            var str = Communicator.Send("S" + Encodings.EncodeMachineString(submissionString));
            var result = Communicator.Send("B. S%#(/} " + str);
            Console.WriteLine(Encodings.DecodeMachineString(result));
        }

        static void RunWithVisualization(string s, int? a = null, int? b = null)
        {
            ProgramGrid pg = ProgramGrid.Parse(s, a, b);
            ProgramRunner runner = new ProgramRunner(pg);
            Console.WriteLine(pg.MetadataToString());
            Console.WriteLine(runner);
            Console.ReadLine();
            while (runner.Step())
            {
                Console.WriteLine(runner);
                Console.ReadLine();
            }

            Console.WriteLine(runner.GetResult());
        }

        static string ReadProblem(string problemName)
        {
            string s = File.ReadAllText(
                Finder.SOLUTION
                .FindDir("ThreeDimensional")
                .GetDirectories("Programs")
                .FirstOrDefault()
                .GetFiles($"{problemName}.3d")
                .FirstOrDefault()
                .FullName);
            return s.Replace("\r", "");
        }

        static int FindSmallestPalindromeBase(int A)
        {
            for (int baseX = 2; baseX <= 10000; baseX++)
            {
                if (IsPalindromeInBase(A, baseX))
                {
                    return baseX;
                }
            }
            return -1; // Should never reach here for given constraints.
        }

        static bool IsPalindromeInBase(int number, int baseX)
        {
            string representation = ConvertToBase(number, baseX);
            return IsPalindrome(representation);
        }

        static string ConvertToBase(int number, int baseX)
        {
            string result = "";
            while (number > 0)
            {
                int remainder = number % baseX;
                result = GetCharForDigit(remainder) + result;
                number /= baseX;
            }
            return result;
        }

        static char GetCharForDigit(int digit)
        {
            if (digit >= 0 && digit <= 9)
            {
                return (char)('0' + digit);
            }
            else
            {
                return (char)('A' + (digit - 10));
            }
        }

        static bool IsPalindrome(string s)
        {
            int left = 0;
            int right = s.Length - 1;
            while (left < right)
            {
                if (s[left] != s[right])
                {
                    return false;
                }
                left++;
                right--;
            }
            return true;
        }

        static void RegularizeFile(string pattern)
        {
            foreach (var f in Finder.SOLUTION.FindDir("ThreeDimensional").GetDirectories("Programs").FirstOrDefault().GetFiles(pattern))
            {
                Console.WriteLine(f.FullName);
                GridRegularizer.RegularizeGridFile(f.FullName);
            }
        }
    }
}
