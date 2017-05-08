namespace DecaTec.WebDav
{
    /// <summary>
    /// Struct representing a progress of an upload or download.
    /// </summary>
    public struct WebDavProgress
    {
        /// <summary>
        /// Gets or sets a value of the bytes received/sent.
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Gets or sets a value of the total bytes to receive/send.
        /// </summary>
        public long TotalBytes { get; set; }
    }
}
