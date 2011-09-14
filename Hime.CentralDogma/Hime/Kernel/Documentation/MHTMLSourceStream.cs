﻿namespace Hime.Kernel.Documentation
{
    public abstract class MHTMLSourceStream : MHTMLSource
    {
        private const int bufferSize = 900;
        protected string mime;
        protected string location;
        private System.IO.Stream stream;
        private byte[] buffer;

        public virtual string ContentType { get { return mime; } }
        public string ContentTransferEncoding { get { return "base64"; } }
        public string ContentLocation { get { return location; } }

        public MHTMLSourceStream(string mime, string location, System.IO.Stream stream)
        {
            this.mime = mime;
            this.location = location;
            this.stream = stream;
            this.buffer = new byte[bufferSize];
        }

        public string Read()
        {
            int read = stream.Read(buffer, 0, bufferSize);
            if (read == 0)
                return null;
            return System.Convert.ToBase64String(buffer, 0, read);
        }
        public void Close() { }
    }
}