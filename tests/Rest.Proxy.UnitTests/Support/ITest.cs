using Rest.Proxy.Attributes;
using Rest.Proxy.UnitTests.Support.Requests;
using Rest.Proxy.UnitTests.Support.Response;

namespace Rest.Proxy.UnitTests.Support
{
    [ServiceRoute(SettingBaseUrlName = "SomeUrlSetting")]
    public interface ITest
    {
        [MethodRoute(Method = HttpMethod.Get, Template = "/resource/{Id}")]
        GetSingleResponse TestGet(GetSingleRequest request);

        [MethodRoute(Method = HttpMethod.Get, Template = "/resources")]
        GetAllResponse TestGetAll(GetAllRequest request);

        [MethodRoute(Method = HttpMethod.Post, Template = "/resources")]
        void TestPost(PostRequest request);

        [MethodRoute(Method = HttpMethod.Put, Template = "/resource/{Id}")]
        void TestPut(PutRequest request);

        [MethodRoute(Method = HttpMethod.Delete, Template = "/resource/{Id}")]
        void TestDelete(DeleteRequest request);
    }
}