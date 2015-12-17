using System;

namespace Server.Contracts.Requests
{
    public class SchedulePortOrderRequest
    {
        public string Id { get; set; }

        public DateTime ToDate { get; set; }

        public string RecipientNetworkOperator { get; set; }

        public string DonorNetworkOperator { get; set; }
    }
}
