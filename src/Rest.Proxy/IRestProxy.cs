using System;

namespace Rest.Proxy
{
    public interface IRestProxy
    {
        object Get(string baseUrl, string resourceUrl, object request, Type responseType);

        object Post(string baseUrl, string resourceUrl, object request, Type responseType);

        object Put(string baseUrl, string resourceUrl, object request, Type responseType);

        void Delete(string baseUrl, string resourceUrl, object request);
    }
}