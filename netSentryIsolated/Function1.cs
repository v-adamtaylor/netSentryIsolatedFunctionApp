using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Sentry;

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
            Task faultyTask = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("I've started my Task!");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    // Simulate an exception
                    throw new IOException("Simulated IO Exception: The network path was not found.");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An exception occurred in the faulty task.", ex);
                    SentrySdk.CaptureException(ex); // Capture the exception manually
                                                    // Rethrow if you want to preserve the original stack trace
                                                    // and have it rethrown by the task scheduler
                    throw;
                }
            });

            // Do not await the task, causing it to be unobserved if it faults
            // In real scenarios, always await or observe your tasks!

            // Rest of your function code...

            // Respond with some message
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            await response.WriteStringAsync("Function executed").ConfigureAwait(false);

            _logger.LogInformation("The Function has completed and is returning!");

            // Flush Sentry queue before exiting
            await SentrySdk.FlushAsync(TimeSpan.FromSeconds(2));

            //await Task.Delay(TimeSpan.FromSeconds(5)); // Give some time for the faultyTask to throw its exception

            return response;
        }
    }
}
