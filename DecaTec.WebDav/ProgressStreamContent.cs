using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class for reporting progress for a <see cref="StreamContent"/>.
    /// </summary>
    /// <remarks>Inspired by the <a href="https://github.com/paulcbetts/ModernHttpClient">ModernHttpClient project</a>, this class is used to report progress while uploading a file using WebDAV.</remarks>
    internal class WebDavProgressStreamContent : StreamContent
    {
        private long bytes;
        private long totalBytes = -1;
        private IProgress<WebDavProgress> progress;

        internal WebDavProgressStreamContent(Stream stream, long totalBytes, CancellationToken token, IProgress<WebDavProgress> progress)
            : this(new ProgressStream(stream, token), totalBytes, progress)
        {
        }

        internal WebDavProgressStreamContent(Stream stream, long totalBytes, int bufferSize, IProgress<WebDavProgress> progress)
            : this(new ProgressStream(stream, CancellationToken.None), totalBytes, bufferSize, progress)
        {
        }

        internal WebDavProgressStreamContent(ProgressStream stream, long totalBytes, IProgress<WebDavProgress> progress)
            : base(stream)
        {
            Init(stream, totalBytes, progress);
        }

        internal WebDavProgressStreamContent(ProgressStream stream, long totalBytes, int bufferSize, IProgress<WebDavProgress> progress)
            : base(stream, bufferSize)
        {
            Init(stream, totalBytes, progress);
        }

        internal void Init(ProgressStream stream, long totalBytes, IProgress<WebDavProgress> progress)
        {
            this.totalBytes = totalBytes;
            stream.ReadCallback = ReadBytes;
            this.progress = progress;
        }

        internal protected void Reset()
        {
            bytes = 0L;
        }
        
        protected void ReadBytes(long bytes)
        {
            if (totalBytes == -1)
                totalBytes = Headers.ContentLength ?? -1;

            if (totalBytes == -1 && TryComputeLength(out long computedLength))
                totalBytes = computedLength == 0 ? -1 : computedLength;

            // If less than zero still then change to -1
            totalBytes = Math.Max(-1, totalBytes);
            this.bytes += bytes;

            this.progress.Report(new WebDavProgress() { Bytes = this.bytes, TotalBytes = totalBytes });
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Reset();
            return base.SerializeToStreamAsync(stream, context);
        }

        protected override bool TryComputeLength(out long length)
        {
            var result = base.TryComputeLength(out length);
            totalBytes = length;
            return result;
        }      
    }
}