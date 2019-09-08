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

            // For debug - waits for server to start up properly before calling
            Thread.Sleep(1000);

            ExecuteChallengeCompletionAPICall().Wait();

            // Waits 20s for DynamoDb to synchronise any writes from the previous call.  Shouldn't be needed as consistent read is set
            // however this doesn't seem to be working.  20s is the AWS stipulated period of time.
            Thread.Sleep(20000);

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
                // Create a custom HTTP request message and use it to call the API method
                var customMethod = new HttpMethod("CalcChallenges");
                var requestMessage = new HttpRequestMessage(customMethod, "api/Rewards");
                HttpResponseMessage res = await cons.SendAsync(requestMessage);
                res.EnsureSuccessStatusCode();
                if (res.IsSuccessStatusCode)
                {
                    var s = await res.Content.ReadAsStringAsync();
                    Console.WriteLine("\n\n" + "METHOD -- ExecuteChallengeCompletionAPICall() -- Complete\n" + "API Content Output: " + s + "\n" +
                        "API HTTP Response: " + res.StatusCode.ToString());
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
                // Create a custom HTTP request message and use it to call the API method
                var customMethod = new HttpMethod("CalcRewards");
                var requestMessage = new HttpRequestMessage(customMethod, "api/Rewards");
                HttpResponseMessage res = await cons.SendAsync(requestMessage);
                res.EnsureSuccessStatusCode();
                if (res.IsSuccessStatusCode)
                {
                    var s = await res.Content.ReadAsStringAsync();
                    Console.WriteLine("\n\n" + "METHOD -- ExecuteRewardCompletionAPICall() -- Complete\n" + "API Content Output: " + s + "\n" +
                        "API HTTP Response: " + res.StatusCode.ToString());
                }
            }
        }
    }
}
