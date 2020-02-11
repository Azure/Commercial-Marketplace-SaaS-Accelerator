namespace Microsoft.Marketplace.Saas.Web.Utlities
{
    using log4net;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using System;

    /// <summary>
    /// Logger
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Contracts.ILogger" />
    public class Logger : ILogger
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILog logger = LogManager.GetLogger(typeof(Logger));

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Debug(string message, Exception ex)
        {
            logger.Debug(message, ex);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            logger.Error(message);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Error(string message, Exception ex)
        {
            logger.Error(message, ex);
        }

        /// <summary>
        /// Information the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        /// Information the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Info(string message, Exception ex)
        {
            logger.Info(message, ex);
        }

        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            logger.Warn(message);
        }

        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Warn(string message, Exception ex)
        {
            logger.Warn(message, ex);
        }
    }
}
