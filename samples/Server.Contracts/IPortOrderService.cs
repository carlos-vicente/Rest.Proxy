using Rest.Proxy;
using Rest.Proxy.Attributes;
using Server.Contracts.Requests;
using Server.Contracts.Responses;

namespace Server.Contracts
{
    [ServiceRoute(SettingBaseUrlName = "ServerEndpoint")]
    public interface IPortOrderService
    {
        [MethodRoute(Method = HttpMethod.Get, Template = "/portOrders")]
        GetAllResponse GetAll(GetAllRequest request);

        [MethodRoute(Method = HttpMethod.Get, Template = "/portOrder/{Id}")]
        GetByIdResponse GetById(GetByIdRequest request);

        [MethodRoute(Method = HttpMethod.Post, Template = "/portOrders")]
        void CreateNewPortOrder(CreateNewPortOrderRequest request);
    }
}
