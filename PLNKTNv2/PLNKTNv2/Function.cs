using Amazon.Lambda.Core;

namespace PLNKTNv2
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(ILambdaContext context)
        {
            return "Test Complete FH.";
        }
    }
}