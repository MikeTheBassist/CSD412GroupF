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
using System.IO;

namespace GroupF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            //// until I figure out how to securely keep an api key in the repo, this variable is a placeholder of sorts.  
            //String apiKey = "Empty";
            //List<String> reservationList = new List<String>();
            //String apiResponse = new String("");
            //var httpClient = new HttpClient();
            //var userNameInfo = await httpClient.GetAsync("http://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key=" + apiKey + "&vanityurl=https://steamcommunity.com/id/AnotherHumanGod");
            //var userName = await userNameInfo.Content.ReadAsStringAsync();
            //using (var response = await httpClient.GetAsync("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key="+apiKey+"&include_appinfo=true&steamid=76561198919733346&format=json"))
            //{
            //    // read response as a String and parse to Json
            //    apiResponse = await response.Content.ReadAsStringAsync();
            //    var jsonFormatResponse = Json(apiResponse);

            //    // test JsonConvert Deserializer
            //    var json = JsonConvert.DeserializeObject(apiResponse);

            //    var jsonFormatResponse2 = Json(response);

            //    return Ok(jsonFormatResponse);
            //    //Dictionary<string, string> keyValuePairs = apiResponse.Split(',')
            //    //.Select(value => value.Split(':'))
            //    //.ToDictionary(pair => pair[0], pair => pair[1]);

            //    //foreach (var entry in keyValuePairs)
            //    //{
            //    //    Console.WriteLine(entry.Key.ToString() + " " + entry.Value.ToString());
            //    //}
            //}

            return View();
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
