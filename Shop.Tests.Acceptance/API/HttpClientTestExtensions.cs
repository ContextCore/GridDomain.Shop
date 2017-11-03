using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Should;

namespace Shop.Tests.Acceptance.API {
    public static class HttpClientTestExtensions
    {
        public static async Task<TResponse> Post<TResponse>(this HttpClient client,string route, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(route, content);

            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);

            return (TResponse)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(TResponse));
        }

        public static async Task<TResponse> Get<TResponse>(this HttpClient client, string route)
        {
            var response = await client.GetAsync(route);
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            return (TResponse)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(TResponse));
        }
    }
}