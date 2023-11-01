using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace netSentryIsolated
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Create a task that tries to access an invalid path
            Task faultyTask = Task.Run(() =>
            {
                try
                {
                    // This path is intentionally incorrect to simulate a network path not found exception
                    File.ReadAllText(@"C:\home\LogFiles\Application\Functions\Host\NonExistentFile.txt");
                }
                catch (IOException ex) { throw new Exception("Could Not Find The File!!!", ex); }
                
            });

            // Do not await the task, causing it to be unobserved if it faults
            // In real scenarios, always await or observe your tasks!

            // Rest of your function code...

            // Respond with some message
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            await response.WriteStringAsync("Function executed").ConfigureAwait(false);


            return response;
        }
    }
}
