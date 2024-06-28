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
        public string? problem_type { get; set; }
        public string? problem_name { get; set; }
        public int expected_score { get; set; }
        public string? solution { get; set; }
        public string meta {get; set;}
    }

    //   return {'status': 'OK', 'score': received_score, 'parsed': result_decode}

    public class SolveResult
    {
        public string? status { get; set; }
        public int? score { get; set; }

        public string? result_decode { get; set; }

    }

    public class SolutionSubmitter
    {
        private static HttpClient client = new();
        public static SolveResult submitSoluton(string problemType, string problemName, int expectedScore, string solution, Dictionary<string, string> meta)
        {
            SolveResult srez = null;
            var solutionJson = JsonSerializer.Serialize(new Solution {
                problem_type = problemType,
                problem_name = problemName,
                expected_score = expectedScore,
                solution = solution,
                meta = JsonSerializer.Serialize(meta)
            });

            for (var i = 0; i < 3; i++)
            {
                var task = client.PostAsync("https://patcdr.net/carl/icfp2024/solve", new StringContent(solutionJson, Encoding.UTF8, "application/json"));

                task.Wait();

                var result = task.Result;
                var rezTask = result.Content.ReadAsStringAsync();
                rezTask.Wait();
                var content = rezTask.Result;

                srez = JsonSerializer.Deserialize<SolveResult>(content);
                if (srez.status == "BUSY SERVER")
                {
                    Console.WriteLine("SERVER BUSY");
                    Thread.Sleep(20);
                }
                else
                {
                    break;
                }

            }

            return srez;
        }
    }
}
