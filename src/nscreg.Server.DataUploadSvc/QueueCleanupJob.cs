using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using nscreg.Data;
using nscreg.Server.Common.Services.DataSources;
using nscreg.ServicesUtils.Interfaces;

namespace nscreg.Server.DataUploadSvc
{
    /// <summary>
    /// Класс по работе очистки очереди
    /// </summary>
    internal class QueueCleanupJob : IJob
    {
        public int Interval { get; }

        private readonly int _timeout;
        private readonly ILogger _logger;

        public QueueCleanupJob(int dequeueInterval, int timeout, ILogger logger)
        {
            Interval = dequeueInterval;
            _timeout = timeout;
            _logger = logger;
        }

        /// <summary>
        /// Метод выполнения очистки очереди
        /// </summary>
        public async Task Execute(CancellationToken cancellationToken)
        {
            var dbContextHelper = new DbContextHelper();
            var ctx = dbContextHelper.CreateDbContext(new string[] { });
            _logger.LogInformation("cleaning up queue...");
            await new QueueService(ctx).ResetDequeuedByTimeout(_timeout);
        }

        /// <summary>
        /// Метод обработчик исключений
        /// </summary>
        public void OnException(Exception e)
        {
            _logger.LogError("cleaning up queue exception {0}", e);
            throw new NotImplementedException();
        }
    }
}
