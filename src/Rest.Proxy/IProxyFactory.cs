namespace Rest.Proxy
{
    public interface IProxyFactory<out TProxyType> where TProxyType : class
    {
        TProxyType CreateProxy();
    }
}