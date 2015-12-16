using System;

namespace Rest.Proxy.Attributes
{
    public class MethodRouteAttribute : Attribute
    {
        public HttpMethod Method { get; set; }

        public string Template { get; set; }
    }
}
