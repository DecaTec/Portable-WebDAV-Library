using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DecaTec.WebDav
{
    /// <summary>
    /// A <see cref="Stream>"/> capable of reporting progress.
    /// </summary>
    /// <remarks>Inspired by the <a href="https://github.com/paulcbetts/ModernHttpClient">ModernHttpClient project</a>.</remarks>
    internal class ProgressStream : Stream
    {
        private CancellationToken token;

        internal ProgressStream(Stream stream, CancellationToken token)
        {
            ParentStream = stream;
            this.token = token;

            ReadCallback = delegate { };
            WriteCallback = delegate { };
        }

        internal Action<long> ReadCallback { get; set; }

        internal Action<long> WriteCallback { get; set; }

        internal Stream ParentStream { get; private set; }

        public override bool CanRead { get { return ParentStream.CanRead; } }

        public override bool CanSeek { get { return ParentStream.CanSeek; } }

        public override bool CanWrite { get { return ParentStream.CanWrite; } }

        public override bool CanTimeout { get { return ParentStream.CanTimeout; } }

        public override long Length { get { return ParentStream.Length; } }

        public override void Flush()
        {
            ParentStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return ParentStream.FlushAsync(cancellationToken);
        }

        public override long Position
        {
            get { return ParentStream.Position; }
            set { ParentStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            token.ThrowIfCancellationRequested();
            var readCount = ParentStream.Read(buffer, offset, count);
            ReadCallback(readCount);
            return readCount;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            token.ThrowIfCancellationRequested();
            return ParentStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            token.ThrowIfCancellationRequested();
            ParentStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            token.ThrowIfCancellationRequested();
            ParentStream.Write(buffer, offset, count);
            WriteCallback(count);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            token.ThrowIfCancellationRequested();
            var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
            var readCount = await ParentStream.ReadAsync(buffer, offset, count, linked.Token);
            ReadCallback(readCount);
            return readCount;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            token.ThrowIfCancellationRequested();
            var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
            var task = ParentStream.WriteAsync(buffer, offset, count, linked.Token);
            WriteCallback(count);
            return task;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ParentStream.Dispose();
            }
        }
    }
}
