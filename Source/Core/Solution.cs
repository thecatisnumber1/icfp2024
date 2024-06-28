using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core
{

    internal class Solution
    {
        public string? problemType { get; set; }
        public string? problemName { get; set; }
        public int expectedScore { get; set; }
        public string? solution { get; set; }
    }

    //   return {'status': 'OK', 'score': received_score, 'parsed': result_decode}

    public class SolveResult
    {
        public string? status { get; set; }
        public int? score { get; set; }

        public string? result_decode { get; set; }

    }

    internal class SolutionSubmitter
    {
        private static HttpClient client = new();
        public static SolveResult submitSoluton(string problemType, string problemName, int expectedScore, string solution)
        {
            SolveResult srez = null;
            for (var i = 0; i < 3; i++)
            {
                var task = client.PostAsJsonAsync<Solution>("https://patcdr.net/carl/icfp2024/solve", new Solution
                {
                    problemType = problemType,
                    problemName = problemName,
                    expectedScore = expectedScore,
                    solution = solution
                });

                task.RunSynchronously();

                var result = task.Result;

                srez = JsonSerializer.Deserialize<SolveResult>(result.Content.ToString());
                if (srez.status == "BUSY SERVER")
                {
                    Console.WriteLine("SERVER BUSY");
                    Thread.Sleep(20);
                }

            }

            return srez;
        }
    }
}
