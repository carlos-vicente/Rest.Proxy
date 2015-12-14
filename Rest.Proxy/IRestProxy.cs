namespace Rest.Proxy
{
    public interface IRestProxy
    {
        object Get(string baseUrl, string resourceUrl, object request);

        object Post(string baseUrl, string resourceUrl, object request);

        object Put(string baseUrl, string resourceUrl, object request);

        object Delete(string baseUrl, string resourceUrl, object request);
    }
}