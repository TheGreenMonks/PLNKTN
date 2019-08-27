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
            Console.WriteLine("Hello World!\n" +
                "This is the console app to execute the following methods from the PLNKTN Web App:\n" +
                "Calculate User Challange completion\n" +
                "Calculate User Reward completion");

            Thread.Sleep(1000);

            ExecuteChallengeCompletionAPICall().Wait();
            ExecuteRewardCompletionAPICall().Wait();

            // TODO - Remove from release version
            Console.ReadLine();
        }

        static async Task ExecuteChallengeCompletionAPICall()
        {
            HttpClient cons = new HttpClient();
            cons.BaseAddress = new Uri("https://localhost:44312/");
            cons.DefaultRequestHeaders.Accept.Clear();
            cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (cons)
            {
                HttpResponseMessage res = await cons.GetAsync("api/Challenges");
                res.EnsureSuccessStatusCode();
                if (res.IsSuccessStatusCode)
                {
                    var s = await res.Content.ReadAsStringAsync();
                    Console.WriteLine("\n\n" + "METHOD -- ExecuteChallengeCompletionAPICall() -- Complete\n" + "API Call Output: " + s);
                }
            }
        }

        static async Task ExecuteRewardCompletionAPICall()
        {
            HttpClient cons = new HttpClient();
            cons.BaseAddress = new Uri("https://localhost:44312/");
            cons.DefaultRequestHeaders.Accept.Clear();
            cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (cons)
            {
                HttpResponseMessage res = await cons.GetAsync("api/Rewards");
                res.EnsureSuccessStatusCode();
                if (res.IsSuccessStatusCode)
                {
                    var s = await res.Content.ReadAsStringAsync();
                    Console.WriteLine("\n\n" + "METHOD -- ExecuteRewardCompletionAPICall() -- Complete\n" + "API Call Output: " + s);
                }
            }
        }
    }
}
