using System;
using RestSharp;

namespace Rest.Proxy.Attributes
{
    public class MethodRouteAttribute : Attribute
    {
        public Method Method { get; set; }

        public string Template { get; set; }
    }
}
