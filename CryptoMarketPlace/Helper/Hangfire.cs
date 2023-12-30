

using CryptoBinance;
using CryptoMarketPlace.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace CryptoMarketPlace.Helper
{
    public class HangfireService
    {
        ApiHelper apiHelper = new ApiHelper();
        static List<FinancialDataDTO> _users = new List<FinancialDataDTO>();
        string url = "https://api2.binance.com/api/v3/ticker/24hr";

        private readonly IHubContext<YourNotificationHub> _hubContext;

        public HangfireService( IHubContext<YourNotificationHub> hubContext)
        {
            
            _hubContext = hubContext;
        }

        public async Task ConfigureHangfire()
        {
            // Configure Hangfire
            GlobalConfiguration.Configuration.UseMemoryStorage();

            // Enqueue a Hangfire job
            BackgroundJob.Enqueue(() => YourRecurringJobMethod());

            // Await any asynchronous operations if needed
            await Task.CompletedTask;
        }

        public void YourRecurringJobMethod()
        {
            Console.WriteLine("job str");
            using (var client = new HttpClient())
            {
                //method that is needed for api connection
                apiHelper.loadHttpClientSettings(client);

                //get response in HttpResponseMessage form
                var response = client.GetAsync(url).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    //in case response is successful this code is excecuted
                    var JsonResult = response.Content.ReadAsStringAsync().Result;
                    //coverts json result to FinancialDTO model
                   var users = (List<FinancialDataDTO>)JsonConvert.DeserializeObject<List<FinancialDataDTO>>(JsonResult);
                    _users = users;
                }
                else
                {
                    // if there is a error in response the status code error will be displayed
                    throw new Exception((int)response.StatusCode + "-" + response.StatusCode.ToString());
                }
                NotifyClientsAboutDataChange();
            }
            Console.WriteLine("job end");
        }

        public List<FinancialDataDTO> GetLastResult()
        {
            // Retrieve the last stored result
            
            return _users;
        }

        private void NotifyClientsAboutDataChange()
        {
            _hubContext.Clients.All.SendAsync("DataChanged");
        }
    }
}
