using System;

namespace Rest.Proxy
{
    /// <summary>
    /// A proxy object factory, used to create a proxy object to a REST service
    /// </summary>
    /// <typeparam name="TProxyType">The interface to use when creating the proxy</typeparam>
    public class ProxyFactory<TProxyType> 
        : IProxyFactory<TProxyType> where TProxyType : class
    {
        private readonly IRestProxy _restProxy;
        private readonly Func<IRestProxy, TProxyType> _proxyBuilderFunc;

        /// <summary>
        /// Creates a factory for proxy objects
        /// </summary>
        /// <param name="proxyBuilderFunc"></param>
        /// <param name="restProxy"></param>
        public ProxyFactory(
            IRestProxy restProxy,
            Func<IRestProxy, TProxyType> proxyBuilderFunc)
        {
            if (!typeof(TProxyType).IsInterface)
                throw new InvalidOperationException("An interface must be provided to create a proxy");

            _restProxy = restProxy;
            _proxyBuilderFunc = proxyBuilderFunc;
        }
        
        /// <summary>
        /// The method builds a proxy object that implements the defined interface and uses an interceptor to
        /// perform the REST request
        /// </summary>
        /// <returns>A proxy object that implements the defined interface</returns>
        public TProxyType CreateProxy()
        {
            return _proxyBuilderFunc(_restProxy);
        }
    }
}
