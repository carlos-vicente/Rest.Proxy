using Castle.Core.Interceptor;

namespace Rest.Proxy
{
    public interface IRestProxy : IInterceptor
    {
        object Get(object request);

        object Post(object request);

        object Put(object request);

        object Delete(object request);
    }
}