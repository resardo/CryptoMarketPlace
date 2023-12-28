using System.Net.Http.Headers;

namespace CryptoBinance
{
    public class ApiHelper
    {
        public void loadHttpClientSettings(HttpClient client)
        {
           

             client.DefaultRequestHeaders.Accept.Clear();

            //sets request header return type as JSON
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
