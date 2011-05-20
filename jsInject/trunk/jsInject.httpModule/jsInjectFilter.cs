using System;
using System.IO;

namespace jsInject.httpModule
{
    class jsInjectFilter : Stream
    {
        private System.IO.Stream Base;
        private string _stringToInject = "<!-- stringToInject is empty! -->";
        private string _injectBefore = "</head>";

        public jsInjectFilter(System.IO.Stream ResponseStream)
        {
            if ((ResponseStream == null)) {
                throw new ArgumentNullException("ResponseStream");
            }
            this.Base = ResponseStream;
        }

        public jsInjectFilter(System.IO.Stream ResponseStream, string stringToInject)
        {
            if ((ResponseStream == null))
            {
                throw new ArgumentNullException("ResponseStream");
            }
            this.Base = ResponseStream;
            if (!string.IsNullOrEmpty(stringToInject))
            {
                this._stringToInject = stringToInject.ToLower();
            }
        }

        public jsInjectFilter(System.IO.Stream ResponseStream, string stringToInject, string injectBefore)
        {
            if ((ResponseStream == null))
            {
                throw new ArgumentNullException("ResponseStream");
            }
            this.Base = ResponseStream;
            if (!string.IsNullOrEmpty(stringToInject))
            {
                this._stringToInject = stringToInject.ToLower();
            }
            if (!string.IsNullOrEmpty(injectBefore))
            {
                this._injectBefore = injectBefore.ToLower();
            }
        }
    
        public override int Read(byte[] buffer, int offset, int count) {
            return this.Base.Read(buffer, offset, count);
        }
    
        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            return this.Base.Seek(offset, origin);
        }
    
        public override void SetLength(long value) {
            this.Base.SetLength(value);
        }
    
        public override void Write(byte[] buffer, int offset, int count) {
            //  Get HTML code
            string HTML = System.Text.Encoding.UTF8.GetString(buffer, offset, count);
            //  Replace the text with something else
            HTML = HTML.ToLower().Replace(_injectBefore, string.Format("{0}{1}", this._stringToInject, _injectBefore));
            //  Send output
            buffer = System.Text.Encoding.UTF8.GetBytes(HTML);
            this.Base.Write(buffer, 0, buffer.Length);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            this.Base.Flush();
        }

        public override long Length
        {
            get { return this.Base.Length; }
        }

        public override long Position
        {
            get
            {
                return this.Base.Position;
            }
            set
            {
                this.Base.Position = value;
            }
        }
    }
}
