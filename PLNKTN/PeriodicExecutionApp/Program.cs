using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeriodicExecutionApp
{
    public class Program
    {
        private static string _serverUri { get; } = "http://ec2-54-241-148-226.us-west-1.compute.amazonaws.com/";
        private static string _localUri { get; } = "https://localhost:44312/";
        private static string _rewardsControllerUri { get; } = "api/Rewards";
        private static string _newLine { get; } = Environment.NewLine;
        private static HttpClient _httpClient { get; set; } = new HttpClient();

        public static void Main(string[] args)
        {
            Console.WriteLine(CreateIntroMsg());

            // Wait for server to start up properly before calling (in case it has been put in standby)
            Thread.Sleep(1000);

            _httpClient = SetupHttpClient(_httpClient);

            Task calcChallenges = ExecuteAPICall("CalcChallenges", _rewardsControllerUri, "CalculateUserChallengeCompletion()");
            Console.WriteLine(_newLine + "Calculating challenge completion...");
            calcChallenges.Wait();
            Console.WriteLine(_newLine + "Challenge calculations complete!");

            // Waits 20s for DynamoDb to synchronise any writes from the previous call.  Shouldn't be needed as consistent read is set
            // however this doesn't seem to be working.  20s is the AWS stipulated period of time.
            Console.WriteLine(_newLine + "Waiting 20 seconds...");
            Thread.Sleep(20000);

            

            ExecuteAPICall("CalcRewards", _rewardsControllerUri, "CalculateUserRewardCompletion()").Wait();

            // TODO - Implement logging to record web API return values
            Thread.Sleep(10000);
        }

        private static HttpClient SetupHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(_serverUri);
            // For LOCAL ONLY -> 
            // httpClient.BaseAddress = new Uri(_localUri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        /// <summary>
        /// Uses a specified URL, method name and HTTP verb to call a Web API method where it will then
        /// check the status code.  If the code is success 
        /// </summary>
        /// <param name="httpRequestVerb">The HTTP verb to be called.  Can be custom</param>
        /// <param name="apiURL">The URL or the API to be called</param>
        /// <param name="apiCallName">The method name in the specified API to be called</param>
        /// <returns>Task operation</returns>
        private static async Task ExecuteAPICall(string httpRequestVerb, string apiURL, string apiCallName)
        {
            // Create a custom HTTP request message and use it to call the API method
            HttpMethod customMethod = new HttpMethod(httpRequestVerb);
            HttpRequestMessage requestMessage = new HttpRequestMessage(customMethod, apiURL);

            try
            {
                HttpResponseMessage webApiResponse = await _httpClient.SendAsync(requestMessage);
                webApiResponse.EnsureSuccessStatusCode();

                if (webApiResponse.IsSuccessStatusCode)
                {
                    string apiResponse = await webApiResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(CreateAPIOutputMsg(apiURL, apiCallName, apiResponse, webApiResponse.StatusCode.ToString()));
                }
            }
            catch (HttpRequestException ex)
            {
                // TODO - Implement logging
                throw ex;
            }
            catch (Exception ex)
            {
                // TODO - Implement logging
                throw new InvalidOperationException("An error has occurred during execution of " + apiCallName + " method.", ex);
            }
        }

        /// <summary>
        /// Builds the informational message to be displayed on the console / logged.
        /// </summary>
        /// <returns>String object with informational message</returns>
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

        /// <summary>
        /// Builds Web API return information message to be displayed to the user / logged.
        /// </summary>
        /// <param name="apiURL">URL of the called API</param>
        /// <param name="apiCallName">The method name in the specified API to be called</param>
        /// <param name="apiResponse">Response message received from the API</param>
        /// <param name="statusCode">Status code received from teh API</param>
        /// <returns>String object with informational message</returns>
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
