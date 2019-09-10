using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PeriodicExecutionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var rewardsControllerUri = "api/Rewards";

            Console.WriteLine("This is the console app to execute the following methods from the PLNKTN Web App:\n" +
                "Calculate User Challange completion\n" +
                "Calculate User Reward completion\n");

            // For debug - waits for server to start up properly before calling
            Thread.Sleep(1000);

            ExecuteAPICall("CalcChallenges", rewardsControllerUri, "CalculateUserChallengeCompletion()").Wait();

            // Waits 20s for DynamoDb to synchronise any writes from the previous call.  Shouldn't be needed as consistent read is set
            // however this doesn't seem to be working.  20s is the AWS stipulated period of time.
            Console.WriteLine("\nWaiting 20 seconds...");
            Thread.Sleep(20000);

            ExecuteAPICall("CalcRewards", rewardsControllerUri, "CalculateUserRewardCompletion()").Wait();

            // TODO - Remove from release version
            Console.ReadLine();
        }

        static async Task ExecuteAPICall(string httpRequestVerb, string apiURL, string apiCallName)
        {
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44312/")
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (httpClient)
            {
                // Create a custom HTTP request message and use it to call the API method
                var customMethod = new HttpMethod(httpRequestVerb);
                var requestMessage = new HttpRequestMessage(customMethod, apiURL);
                HttpResponseMessage res = await httpClient.SendAsync(requestMessage);
                res.EnsureSuccessStatusCode();
                if (res.IsSuccessStatusCode)
                {
                    var s = await res.Content.ReadAsStringAsync();
                    Console.WriteLine("\n" + "The API method '" + apiCallName + "' at location '" + apiURL + "' has been executed.\n" + "API Content Output: " + s + "\n" +
                        "API HTTP Response: " + res.StatusCode.ToString());
                }
            }
        }
    }
}
