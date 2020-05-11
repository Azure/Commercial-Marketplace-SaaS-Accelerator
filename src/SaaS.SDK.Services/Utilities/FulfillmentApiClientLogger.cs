namespace Microsoft.Marketplace.SaaS.SDK.Services.Utilities
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaasKit.Services;

    /// <summary>
    /// Logger.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Contracts.ILogger" />
    public class FulfillmentApiClientLogger : SaasKit.Contracts.ILogger
    {
        private readonly ILogger<FulfillmentApiClient> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentApiClientLogger"/> class.
        /// </summary>
        public FulfillmentApiClientLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });

            this.logger = loggerFactory.CreateLogger<FulfillmentApiClient>();
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            this.logger.LogDebug(message);
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Debug(string message, Exception ex)
        {
            this.logger.LogDebug(ex, message);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            this.logger.LogError(message);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Error(string message, Exception ex)
        {
            this.logger.LogError(ex, message);
        }

        /// <summary>
        /// Information the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            this.logger.LogInformation(message);
        }

        /// <summary>
        /// Information the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Info(string message, Exception ex)
        {
            this.logger.LogInformation(ex, message);
        }

        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            this.logger.LogWarning(message);
        }

        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Warn(string message, Exception ex)
        {
            this.logger.LogWarning(ex, message);
        }
    }
}
