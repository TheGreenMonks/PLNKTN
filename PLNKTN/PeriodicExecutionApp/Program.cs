using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeriodicExecutionApp
{
    class Program
    {
        private static string _serverUri { get; } = "http://ec2-54-241-148-226.us-west-1.compute.amazonaws.com/";
        private static string _localUri { get; } = "https://localhost:44312/";
        private static string _rewardsControllerUri { get; } = "api/Rewards";
        private static string _newLine { get; } = Environment.NewLine;
        private static HttpClient _httpClient { get; } = new HttpClient();

        static void Main(string[] args)
        {
            Console.WriteLine(CreateIntroMsg());

            // Waits for server to start up properly before calling (in case it has been put in standby)
            Thread.Sleep(1000);

            ExecuteAPICall("CalcChallenges", _rewardsControllerUri, "CalculateUserChallengeCompletion()").Wait();

            // Waits 20s for DynamoDb to synchronise any writes from the previous call.  Shouldn't be needed as consistent read is set
            // however this doesn't seem to be working.  20s is the AWS stipulated period of time.
            Console.WriteLine(_newLine + "Waiting 20 seconds...");
            Thread.Sleep(20000);

            ExecuteAPICall("CalcRewards", _rewardsControllerUri, "CalculateUserRewardCompletion()").Wait();

            // TODO - Implement logging to record web API return values
            Thread.Sleep(10000);
            return;
        }

        static async Task ExecuteAPICall(string httpRequestVerb, string apiURL, string apiCallName)
        {
            _httpClient.BaseAddress = new Uri(_serverUri);
            // For LOCAL ONLY -> 
            // _httpClientBaseAddress = new Uri(_localUri);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Create a custom HTTP request message and use it to call the API method
            var customMethod = new HttpMethod(httpRequestVerb);
            var requestMessage = new HttpRequestMessage(customMethod, apiURL);
            HttpResponseMessage res = await _httpClient.SendAsync(requestMessage);
            res.EnsureSuccessStatusCode();
            if (res.IsSuccessStatusCode)
            {
                var apiResponse = await res.Content.ReadAsStringAsync();
                Console.WriteLine(CreateAPIOutputMsg(apiURL, apiCallName, apiResponse, res.StatusCode.ToString()));
            }
        }

        private static string CreateIntroMsg()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("This is the console app to execute the following methods from the PLNKTN Web App:");
            sb.Append(_newLine);
            sb.Append("Calculate User Challange completion");
            sb.Append(_newLine);
            sb.Append("Calculate User Reward completion");
            sb.Append(_newLine);

            return sb.ToString();
        }

        private static string CreateAPIOutputMsg(string apiURL, string apiCallName, string apiResponse, string statusCode)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("The API method '");
            sb.Append(apiCallName);
            sb.Append("' at location '");
            sb.Append(apiURL);
            sb.Append("' has been executed.");
            sb.Append(_newLine);
            sb.Append("API Content Output: ");
            sb.Append(apiResponse);
            sb.Append(_newLine);
            sb.Append("API HTTP Response: ");
            sb.Append(statusCode);

            return sb.ToString();
        }
    }
}
