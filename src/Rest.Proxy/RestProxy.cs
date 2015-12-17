using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly Func<string, Method, IRestRequest> _requestFactoryFunc;

        public RestProxy(
            IRestClient restClient,
            ISerializer serializer,
            Func<string, Method, IRestRequest> requestFactoryFunc)
        {
            _restClient = restClient;
            _serializer = serializer;
            _requestFactoryFunc = requestFactoryFunc;
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

            return ExecuteRequestWithoutBody(
                baseUrl,
                resourceUrl,
                request,
                responseType,
                Method.GET);
        }

        public object Post(string baseUrl, string resourceUrl, object request, Type responseType)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(resourceUrl))
                throw new ArgumentNullException(nameof(resourceUrl));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (responseType == null)
                throw new ArgumentNullException(nameof(responseType));

            return ExecuteRequestWithBody(
                baseUrl,
                resourceUrl,
                request,
                responseType,
                Method.POST);
        }

        public object Put(string baseUrl, string resourceUrl, object request, Type responseType)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(resourceUrl))
                throw new ArgumentNullException(nameof(resourceUrl));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (responseType == null)
                throw new ArgumentNullException(nameof(responseType));

            return ExecuteRequestWithBody(
                baseUrl,
                resourceUrl,
                request,
                responseType,
                Method.PUT);
        }

        public void Delete(string baseUrl, string resourceUrl, object request)
        {
            throw new NotImplementedException();
        }

        private object ExecuteRequestWithoutBody(
            string baseUrl,
            string resourceUrl,
            object request,
            Type responseType,
            Method method)
        {
            // replace everything in URL template
            _restClient.BaseUrl = new Uri(baseUrl);

            // create a request for the URL
            var restRequest = GetRestRequest(resourceUrl, request, method);

            // execute the request
            var response = ExecuteRequestAndValidateResponse(restRequest);

            // deserialize the response body to return
            return responseType == typeof(void)
                ? null
                : _serializer.Deserialize(responseType, response.Content);
        }

        private object ExecuteRequestWithBody(
            string baseUrl,
            string resourceUrl,
            object request,
            Type responseType,
            Method method)
        {
            // replace everything in URL template
            _restClient.BaseUrl = new Uri(baseUrl);

            // create a request for the URL
            var restRequest = GetRestRequestWithBody(resourceUrl, request, method);

            // execute the request
            var response = ExecuteRequestAndValidateResponse(restRequest);

            // deserialize the response body to return (if there is return)
            return responseType == typeof (void)
                ? null
                : _serializer.Deserialize(responseType, response.Content);
        }

        private IRestRequest GetRestRequest(string resourceUrl, object request, Method method)
        {
            var segments = ExtractSegments(resourceUrl, request);

            var finalResourceUrl = new StringBuilder(resourceUrl)
                .ToString();

            segments
                .ForEach(segment =>
                    finalResourceUrl = finalResourceUrl
                        .Replace($"{{{segment.Name}}}", segment.Value));

            return _requestFactoryFunc(finalResourceUrl, method);
        }

        private IRestRequest GetRestRequestWithBody(string resourceUrl, object request, Method method)
        {
            var restRequest = GetRestRequest(resourceUrl, request, method);

            restRequest.AddJsonBody(request);

            return restRequest;
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

        private IRestResponse ExecuteRequestAndValidateResponse(IRestRequest restRequest)
        {
            var restResponse = _restClient.Execute(restRequest);

            if (restResponse.ErrorException != null)
            {
                throw new HttpException(
                    $"Server returned error: {restResponse.ErrorMessage}",
                    restResponse.ErrorException);
            }

            // TODO: just OK or all the 2xx family????
            if (restResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Server returned error: {restResponse.StatusDescription}");
            }

            return restResponse;
        }
    }
}
