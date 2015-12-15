using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;
using CV.Common.Serialization;
using RestSharp;

namespace Rest.Proxy
{
    public class RestProxy : IRestProxy
    {
        private class UrlSegment
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        private readonly IRestClient _restClient;
        private readonly ISerializer _serializer;

        public RestProxy(
            IRestClient restClient,
            ISerializer serializer)
        {
            _restClient = restClient;
            _serializer = serializer;
        }

        public object Get(string baseUrl, string resourceUrl, object request, Type responseType)
        {
            if(string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(resourceUrl))
                throw new ArgumentNullException(nameof(resourceUrl));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (responseType == null)
                throw new ArgumentNullException(nameof(responseType));

            // replace everything in URL template
            _restClient.BaseUrl = new Uri(baseUrl);

            // create a request for the URL
            var segments = ExtractSegments(resourceUrl, request);

            var finalResourceUrl = new StringBuilder(resourceUrl)
                .ToString();

            segments
                .ForEach(segment =>
                    finalResourceUrl = finalResourceUrl
                        .Replace($"{{{segment.Name}}}", segment.Value));

            var restRequest = new RestRequest(finalResourceUrl, Method.GET);

            // execute the request
            var response = _restClient.Execute(restRequest);

            // deserialize the response body to return
            return _serializer.Deserialize(responseType, response.Content);
        }

        public void Post(string baseUrl, string resourceUrl, object request)
        {
            throw new NotImplementedException();
        }

        public void Put(string baseUrl, string resourceUrl, object request)
        {
            throw new NotImplementedException();
        }

        public void Delete(string baseUrl, string resourceUrl, object request)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<UrlSegment> ExtractSegments(string resourceUrl, object request)
        {
            var segments = new List<UrlSegment>();

            var lastIndex = 0;
            var openIndex = 0;
            var closeIndex = 0;

            do
            {
                openIndex = resourceUrl.IndexOf("{", lastIndex);

                if (openIndex != -1)
                {
                    closeIndex = resourceUrl.IndexOf("}", lastIndex);

                    var segmentName = resourceUrl
                        .Substring(openIndex + 1, (closeIndex - openIndex) - 1);

                    var segmentValue = GetPropertyValue(request, segmentName);

                    if(segmentValue != null)
                        segments.Add(new UrlSegment
                        {
                            Name = segmentName,
                            Value = segmentValue
                        });

                    lastIndex = openIndex + 1;
                }
            }
            while (openIndex != -1);

            return segments;
        }

        private static string GetPropertyValue(object request, string propertyName)
        {
            var properties = request
                .GetType()
                .GetProperties();

            var property = properties
                .SingleOrDefault(p => p.Name.Equals(propertyName));

            return property?.GetValue(request)?.ToString();
        }
    }
}
