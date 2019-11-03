using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GroupF.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace GroupF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<String> reservationList = new List<String>();
            String apiResponse = new String("");
            using (var httpClient = new HttpClient())
            {
                
                using (var response = await httpClient.GetAsync("http://api.oceandrivers.com/v1.0/getEasyWind/EW013"))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                    
                }
            }
            return View(apiResponse);
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
