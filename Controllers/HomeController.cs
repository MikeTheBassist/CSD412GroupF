using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GroupF.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;

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
            // Create a test GameInfo object using Half Life data for testing view creation
            GameInfo gameTest = new GameInfo();

            // Json representing Half Life data
            String testString = "{\"appid\": 70,\"name\": \"Half-Life\",\"playtime_forever\": 100,\"img_icon_url\":" +
                " \"95be6d131fc61f145797317ca437c9765f24b41c\",\"img_logo_url\": \"6bd76ff700a8c7a5460fbae3cf60cb930279897d\",\"has_community_visible_stats\": " +
                "true,\"playtime_windows_forever\": 0,\"playtime_mac_forever\": 0,\"playtime_linux_forever\": 0}";

            gameTest = JsonConvert.DeserializeObject<GameInfo>(testString);

            // until I figure out how to securely keep an api key in the repo, this variable is a placeholder of sorts.  
            // get a Steam API Key using your login and 127.0.0.1 as your Domain Name: steamcommunity.com/dev/apikey
            String apiKey = "INSERT API KEY HERE";

            // create new HttpClient for sending and receiving information via Http protocol
            var httpClient = new HttpClient();

            // Using my Steam ID as a placeholder, this will be replaced by the "getUserNameFromId" method once it's written...
            long steamId = 76561197993425790;

            List<GameInfo> gameList = await parseGetOwnedGamesAsync(apiKey, steamId, httpClient);

            //TODO: Write method for getting SteamID from username.
            //var userNameInfo = await httpClient.GetAsync("http://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key=" + apiKey + "&vanityurl=https://steamcommunity.com/id/AnotherHumanGod");

            //var userName = await userNameInfo.Content.ReadAsStringAsync();


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public async Task<String> getUserNameFromId(long id)
        {
            String fakeName = "SuperRadical";

            return fakeName;
        }
        public async Task<List<GameInfo>> parseGetOwnedGamesAsync(String apiKey, long steamId, HttpClient client)
        {

            String queryString = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + apiKey + "&include_appinfo=true&steamid="+steamId+"&format=json";

            using (var response = await client.GetAsync(queryString))
            {
                // read response as a String and parse to Json
                String apiResponse = await response.Content.ReadAsStringAsync();

                //// test JsonConvert Deserializer
                String gamesList = apiResponse.Split("[")[1];

                String[] gamesListParsed = Regex.Split(gamesList, @"(?=[{])");

                List<GameInfo> allGames = new List<GameInfo>();

                for (int i = 1; i < gamesListParsed.Length; i++)
                {
                    if (i != gamesListParsed.Length - 1)
                    {
                        string tempString = new string(gamesListParsed[i].TrimEnd(','));
                        allGames.Add(JsonConvert.DeserializeObject<GameInfo>(tempString));
                    }
                    else
                    {
                        string tempString = new string(gamesListParsed[i].Substring(0, gamesListParsed[i].Length - 3));
                        allGames.Add(JsonConvert.DeserializeObject<GameInfo>(tempString));
                    }

                }

                return allGames;

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
