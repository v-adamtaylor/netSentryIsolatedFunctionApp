using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace netSentryIsolated
{
    public class myTimer
    {
        private readonly ILogger _logger;

        public myTimer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<myTimer>();
        }

        [Function("myTimer")]
        public void Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
