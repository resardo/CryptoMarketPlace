using CryptoBinance;
using CryptoMarketPlace.Helper;
using CryptoMarketPlace.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CryptoMarketPlace.Controllers
{
    public class HomeController : Controller
    {
        HangfireService _hangfireService;
        ApiHelper apiHelper = new ApiHelper();
        static List<FinancialDataDTO> users = new List<FinancialDataDTO>();
        private readonly ILogger<HomeController> _logger;
        string url = "https://api2.binance.com/api/v3/ticker/24hr";

        public HomeController(ILogger<HomeController> logger, HangfireService hangfireService)
        {
            
            _hangfireService = hangfireService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var result = FetchData().Result;
            return View(result);
        
        }

        public async Task<List<FinancialDataDTO>> FetchData()
        {
            // Call the ConfigureHangfire method to set up Hangfire and wait for it to complete
            await _hangfireService.ConfigureHangfire();

            // Now you can retrieve the data from the completed job
            var result = _hangfireService.GetLastResult();

            // Return the result
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Fetch()
        {
           try
            {
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
                        users = (List<FinancialDataDTO>)JsonConvert.DeserializeObject<List<FinancialDataDTO>>(JsonResult);
                    }
                    else
                    {
                        // if there is a error in response the status code error will be displayed
                        throw new Exception((int)response.StatusCode + "-" + response.StatusCode.ToString());
                    }
                    
                }
            }
            catch (Exception ex)
            {

            }
            //returns data 

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}