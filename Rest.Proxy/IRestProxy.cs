using Castle.Core.Interceptor;

namespace Rest.Proxy
{
    public interface IRestProxy : IInterceptor
    {
        void Get();

        void Post();

        void Put();

        void Delete();
    }
}