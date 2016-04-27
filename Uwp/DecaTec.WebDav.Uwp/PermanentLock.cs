using System.Threading.Tasks;
using Windows.Web.Http;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a permanent lock for use in WebDavSession.
    /// </summary>
    internal partial class PermanentLock
    {
        /// <summary>
        /// Unlocks the currently locked resource.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        internal async Task<HttpResponseMessage> UnlockAsync()
        {
            return await this.WebDavClient.UnlockAsync(this.LockRoot, this.LockToken);
        }
    }
}
