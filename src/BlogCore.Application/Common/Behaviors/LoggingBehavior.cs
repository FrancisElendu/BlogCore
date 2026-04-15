using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BlogCore.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Processing {RequestName} started at {Timestamp}", requestName, DateTime.UtcNow);

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("Processing {RequestName} completed in {ElapsedMilliseconds}ms at {Timestamp}",
                    requestName, stopwatch.ElapsedMilliseconds, DateTime.UtcNow);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing {RequestName} after {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
