using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Rewards_Challenges_Calculation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HttpClient cons = new HttpClient();
            cons.BaseAddress = new Uri("https://localhost:44312/");
            Thread.Sleep(1000);
            cons.DefaultRequestHeaders.Accept.Clear();
            cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ExecuteChallengeRewardAPICall(cons).Wait();
        }

        static async Task ExecuteChallengeRewardAPICall(HttpClient cons)
        {
            using (cons)
            {
                HttpResponseMessage res = await cons.GetAsync("api/Challenges");
                res.EnsureSuccessStatusCode();
                if (res.IsSuccessStatusCode)
                {
                    var s = await res.Content.ReadAsStringAsync();
                    Console.WriteLine(s);
                    Console.ReadLine();
                }
            }
        }
    }
}
