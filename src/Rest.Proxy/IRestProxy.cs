using System;

namespace Rest.Proxy
{
    public interface IRestProxy
    {
        object Get(string baseUrl, string resourceUrl, object request, Type responseType);

        void Post(string baseUrl, string resourceUrl, object request);

        void Put(string baseUrl, string resourceUrl, object request);

        void Delete(string baseUrl, string resourceUrl, object request);
    }
}