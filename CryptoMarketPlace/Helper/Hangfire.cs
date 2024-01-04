

using CryptoBinance;
using CryptoMarketPlace.Models;
using Hangfire;
using Hangfire.MemoryStorage;
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
           
            GlobalConfiguration.Configuration.UseMemoryStorage();

            BackgroundJob.Enqueue(() => YourRecurringJobMethod());

            await Task.CompletedTask;
        }

        public void YourRecurringJobMethod()
        {
            using (var client = new HttpClient())
            {
                apiHelper.loadHttpClientSettings(client);

                //get response in HttpResponseMessage form
                var response = client.GetAsync(url).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var JsonResult = response.Content.ReadAsStringAsync().Result;
                    var users = (List<FinancialDataDTO>)JsonConvert.DeserializeObject<List<FinancialDataDTO>>(JsonResult);
                    _users = users;
                }
                else
                {
                    throw new Exception((int)response.StatusCode + "-" + response.StatusCode.ToString());
                }
                NotifyClientsAboutDataChange();
            }
        }

        public List<FinancialDataDTO> GetLastResult()
        {
           return _users;
        }

        private void NotifyClientsAboutDataChange()
        {
            _hubContext.Clients.All.SendAsync("DataChanged");
        }
    }
}
