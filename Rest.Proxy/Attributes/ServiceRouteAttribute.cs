using System;

namespace Rest.Proxy.Attributes
{
    public class ServiceRouteAttribute : Attribute
    {
        public string BaseUrl { get; set; }
    }
}