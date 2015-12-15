using Rest.Proxy.Attributes;
using Rest.Proxy.UnitTests.Support.Requests;
using Rest.Proxy.UnitTests.Support.Response;
using RestSharp;

namespace Rest.Proxy.UnitTests.Support
{
    [ServiceRoute(BaseUrl = "http://www.testserver.com/testservice")]
    public interface ITest
    {
        [MethodRoute(Method = Method.GET, Template = "/resource/{Id}")]
        GetSingleResponse TestGet(GetSingleRequest request);

        [MethodRoute(Method = Method.GET, Template = "/resources")]
        GetAllResponse TestGetAll(GetAllRequest request);

        [MethodRoute(Method = Method.POST, Template = "/resources")]
        void TestPost(PostRequest request);

        [MethodRoute(Method = Method.PUT, Template = "/resource/{Id}")]
        void TestPut(PutRequest request);

        [MethodRoute(Method = Method.DELETE, Template = "/resource/{Id}")]
        void TestDelete(DeleteRequest request);
    }
}