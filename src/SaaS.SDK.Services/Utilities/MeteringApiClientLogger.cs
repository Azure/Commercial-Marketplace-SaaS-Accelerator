namespace Microsoft.Marketplace.SaaS.SDK.Services.Utilities
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaasKit.Services;
    using System;

    /// <summary>
    /// Logger
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Contracts.ILogger" />
    public class MeteringApiClientLogger : SaasKit.Contracts.ILogger
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<MeteredBillingApiClient> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringApiClientLogger"/> class.
        /// </summary>
        public MeteringApiClientLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });

            this.logger = loggerFactory.CreateLogger<MeteredBillingApiClient>();
        }

        /// <summary>
        /// Log the message at Debug severity
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            logger.LogDebug(message);
        }

        /// <summary>
        /// Log the message at Debug severity
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Debug(string message, Exception ex)
        {
            logger.LogDebug(ex, message);
        }

        /// <summary>
        /// Log the message at Error severity
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            logger.LogError(message);
        }

        /// <summary>
        /// Log the message at Error severity
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Error(string message, Exception ex)
        {
            logger.LogError(ex, message);
        }

        /// <summary>
        /// Log the message at Info severity
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            logger.LogInformation(message);
        }

        /// <summary>
        /// Log the message at Info severity
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Info(string message, Exception ex)
        {
            logger.LogInformation(ex, message);
        }

        /// <summary>
        /// Log the message at Warn severity
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            logger.LogWarning(message);
        }

        /// <summary>
        /// Log the message at Warn severity
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Warn(string message, Exception ex)
        {
            logger.LogWarning(ex, message);
        }
    }
}
