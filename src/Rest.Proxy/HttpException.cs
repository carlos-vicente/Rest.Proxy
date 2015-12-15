using System;

namespace Rest.Proxy
{
    public class HttpException : Exception
    {
        public HttpException()
            : base("Exception occurred when performing an Http request")
        {
        }

        public HttpException(string message)
            : base(message)
        {
        }

        public HttpException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
