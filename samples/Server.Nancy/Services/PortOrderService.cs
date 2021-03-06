﻿using System.Collections.Generic;
using Server.Contracts;
using Server.Contracts.Requests;
using Server.Contracts.Responses;

namespace Server.Nancy.Services
{
    public class PortOrderService : IPortOrderService
    {
        public GetAllResponse GetAll(GetAllRequest request)
        {
            return new GetAllResponse
            {
                Msisdns = new List<string>
                {
                    "003517962145891",
                    "003517962145892",
                    "003517962145893",
                }
            };
        }

        public GetByIdResponse GetById(GetByIdRequest request)
        {
            return new GetByIdResponse
            {
                Msisdn = $"0035179621458{request.Id}"
            };
        }

        public CreateNewPortOrderResponse CreateNewPortOrder(CreateNewPortOrderRequest request)
        {
            return new CreateNewPortOrderResponse
            {
                Id = request
                    .Msisdn
                    .Substring(request.Msisdn.Length - 3, 2)
            };
        }

        public void SchedulePortOrder(SchedulePortOrderRequest request)
        {
            // do something
        }
    }
}