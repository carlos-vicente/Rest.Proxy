using System;

namespace Rest.Proxy.Attributes
{
    public class ServiceRouteAttribute : Attribute
    {
        public string SettingBaseUrlName { get; set; }
    }
}