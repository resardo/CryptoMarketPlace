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
            var result = await FetchData();
            return View(result);
       
        }

        public async Task<IActionResult> FetchData()
        {
            // Call the ConfigureHangfire method to set up Hangfire and wait for it to complete
            await _hangfireService.ConfigureHangfire();

            var result = _hangfireService.GetLastResult();

            return Json(result);
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