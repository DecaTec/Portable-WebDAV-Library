using System;
using System.Threading;
using System.Threading.Tasks;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a permanent lock for use in WebDavSession.
    /// </summary>
    internal class PermanentLock : IDisposable
    {
        private CancellationTokenSource cts;

        /// <summary>
        /// Initializes a new instance of PermanentLock.
        /// </summary>
        /// <param name="webDavClient">The WebDavClient to use.</param>
        /// <param name="lockToken">The LockToken to use.</param>
        /// <param name="lockRoot">The root folder of the lock.</param>
        /// <param name="timeoutString">The timeout string od the lock.</param>
        internal PermanentLock(WebDavClient webDavClient, LockToken lockToken, Uri lockRoot, string timeoutString)
        {
            this.WebDavClient = webDavClient;
            this.LockToken = lockToken;
            this.LockRoot = lockRoot;
            this.Timeout = ParseTimeoutString(timeoutString);

            if (this.Timeout.HasValue)
            {
                // No-infinite lock.
                // Timer is started immediately.
                // 5% timer offset, i.e. the time span the timer should raise before the lock expires.
                this.cts = new CancellationTokenSource();
                var offset = this.Timeout.Value.TotalSeconds * 0.05;
                var timerTimeSpan = TimeSpan.FromSeconds(this.Timeout.Value.TotalSeconds - offset);
                StartInfiniteLock(timerTimeSpan, this.cts);
            }
        }

        private void StartInfiniteLock(TimeSpan refrehTimeSpan, CancellationTokenSource cts)
        {
            var task = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(refrehTimeSpan, cts.Token);
                    RefreshLock();
                }
            }, cts.Token);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the WebDavClient.
        /// </summary>
        private WebDavClient WebDavClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the LockToken.
        /// </summary>
        internal LockToken LockToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the root folder of the lock.
        /// </summary>
        internal Uri LockRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the timeout of the lock.
        /// </summary>
        internal TimeSpan? Timeout
        {
            get;
            private set;
        }

        #endregion Properties

        #region Internal methods

        /// <summary>
        /// Unlocks the currently locked resource.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        internal async Task<WebDavResponseMessage> UnlockAsync()
        {
            return await this.WebDavClient.UnlockAsync(this.LockRoot, this.LockToken);
        }

        #endregion Internal methods

        #region Private methods

        private void RefreshLock()
        {
            var task = this.WebDavClient.RefreshLockAsync(this.LockRoot, WebDavTimeoutHeaderValue.CreateWebDavTimeout(this.Timeout.Value), this.LockToken);
            task.Wait();

            if (!task.Result.IsSuccessStatusCode)
                throw new WebDavException("The lock for " + this.LockRoot.ToString() + " cannot be refreshed");
        }

        private static TimeSpan? ParseTimeoutString(string timeoutString)
        {
            // The timeout string may have the values 'Infinite' (for infinite locks) or 'Second-4100000000' (for a timeout specified in seconds).
            if (timeoutString.Trim().ToLower().StartsWith("second", StringComparison.OrdinalIgnoreCase))
            {
                var split = timeoutString.Split('-');
                var seconds = Convert.ToInt32(split[1]);
                return TimeSpan.FromSeconds(seconds);
            }
            else
                return null;
        }

        #endregion Private methods

        #region Dispose

        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes an instance of PermanentLock.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.

                if (this.cts != null)
                {
                    this.cts.Cancel();
                    this.cts.Dispose();
                }

                // Unlock active lock.
                UnlockAsync().Wait();
            }

            // Free any unmanaged objects here.

            disposed = true;
        }

        #endregion Dispose
    }
}
