namespace Winium.Mobile.Connectivity
{
    #region

    using System;

    using Winium.StoreApps.Logging;

    #endregion

    internal static class Retry
    {
        #region Public Methods and Operators

        public static void WithRetry(Action action, uint maxRetries)
        {
            var triesLeft = maxRetries;
            while (true)
            {
                --triesLeft;
                try
                {
                    action();
                    return;
                }
                catch (Exception exception)
                {
                    if (triesLeft > 0)
                    {
                        Logger.Warn(
                            "Exception {0} {1} caught, going to retry. Retries left {2}.", 
                            exception.GetType().ToString(), 
                            exception.Message, 
                            triesLeft);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        #endregion
    }
}
