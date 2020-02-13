using log4net;
using Microsoft.Marketplace.SaasKit.Contracts;
using System;

namespace Microsoft.Marketplace.SaasKit.Client.Utilities
{
    public class Logger : ILogger
    {
        protected readonly ILog logger = LogManager.GetLogger(typeof(Logger));

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Debug(string message, Exception ex)
        {
            logger.Debug(message, ex);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            logger.Error(message, ex);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }

        public void Info(string message, Exception ex)
        {
            logger.Info(message, ex);
        }

        public void Warn(string message)
        {
            logger.Warn(message);
        }

        public void Warn(string message, Exception ex)
        {
            logger.Warn(message, ex);
        }
    }
}
