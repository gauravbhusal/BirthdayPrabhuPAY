using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ApiCallMethods
{
    public class WebApiRequestMethods<T, T1> : IWebApiRequest<T, T1>
    {
        private string _uri;
        public WebApiRequestMethods(string uri)
        {
            _uri = uri;
        }

        public async Task<string> GetAsync(T1 id, string authorizationHeader = "")
        {
            var client = new RestClient();

            if (id != null && id.ToString().Trim() != "" && !id.ToString().StartsWith("/"))
            {
                client.BaseUrl = new Uri(_uri + "?" + id);
            }
            else if (id.ToString().Contains("/"))
            {
                client.BaseUrl = new Uri(_uri + id);
            }
            else
            {
                client.BaseUrl = new Uri(_uri);
            }
            var request = new RestRequest(Method.GET) { RequestFormat = DataFormat.Json };
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.AddHeader("Authorization", authorizationHeader);
            }
            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            if (response != null && (response.StatusCode == HttpStatusCode.Accepted) &&
                (response.ResponseStatus == ResponseStatus.Completed))
            {
                return response.Content;
            }
            return default(string);
        }
    }

    public interface IWebApiRequest<T, T1>
    {
        Task<string> GetAsync(T1 id, string authorizationHeader = "");
    }

}


