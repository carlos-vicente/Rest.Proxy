using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using Server.Contracts;
using Server.Contracts.Requests;

namespace Server.Modules
{
    public class PortOrderModule : NancyModule
    {
        private readonly IPortOrderService _service;

        public PortOrderModule(IPortOrderService service)
        {
            _service = service;

            Get["/portOrders"] = GetAllPortOrders;
            Get["/portOrder/{Id}"] = GetPortOrderById;
            Post["/portOrders"] = PostPortOrder;
        }

        private dynamic GetAllPortOrders(dynamic parameters)
        {
            var request = this.Bind<GetAllRequest>();

            var response = _service.GetAll(request);

            return Negotiate
                .WithStatusCode(HttpStatusCode.OK)
                .WithModel(response)
                .WithAllowedMediaRange(new MediaRange("application/json"));
        }

        private dynamic GetPortOrderById(dynamic parameters)
        {
            var request = this.Bind<GetByIdRequest>();

            var response = _service.GetById(request);

            return Negotiate
                .WithStatusCode(HttpStatusCode.OK)
                .WithModel(response)
                .WithAllowedMediaRange(new MediaRange("application/json"));
        }

        private dynamic PostPortOrder(dynamic parameters)
        {
            var request = this.Bind<CreateNewPortOrderRequest>();

            _service.CreateNewPortOrder(request);

            return Negotiate
                .WithStatusCode(HttpStatusCode.OK);
        }
    }
}