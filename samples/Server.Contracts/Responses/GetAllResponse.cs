using System.Collections.Generic;

namespace Server.Contracts.Responses
{
    public class GetAllResponse
    {
        public IEnumerable<string> Msisdns { get; set; }
    }
}
