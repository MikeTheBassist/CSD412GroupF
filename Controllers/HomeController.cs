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
            // TODO:  Add a 'login with Steam' interface or just have a text box you put your Steam Username into that gets passed to the Recommendations action

            return View();
        }

        public async Task<IActionResult> Recommendations()
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
            String apiKey = "INSERT KEY HERE";

            // create new HttpClient for sending and receiving information via Http protocol
            var httpClient = new HttpClient();

            String userName = "INSERT STEAM USERNAME";

            // Using my Steam ID as a placeholder, this will be replaced by the "getUserNameFromId" method once it's written...
            long steamId = await getSteamIdFromUserName(apiKey, userName, httpClient);
            List<GameInfo> gameList = await parseGetOwnedGamesAsync(apiKey, steamId, httpClient);

            ViewData["gameList"] = gameList;

            return View(gameList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<List<GameInfo>> parseGetOwnedGamesAsync(String apiKey, long steamId, HttpClient client)
        {

            String queryString = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + apiKey + "&include_appinfo=true&steamid=" + steamId + "&format=json";

            using (var response = await client.GetAsync(queryString))
            {
                // read response as a String and parse to Json
                String apiResponse = await response.Content.ReadAsStringAsync();

                if(response.IsSuccessStatusCode)
                {
                    //// test JsonConvert Deserializer
                    dynamic parsedResponse = JsonConvert.DeserializeObject<dynamic>(apiResponse);

                    List<GameInfo> allGames = new List<GameInfo>();

                    if (parsedResponse.response.game_count >= 1)
                    {
                        Console.WriteLine("Games were found.");

                        foreach (var game in parsedResponse.response.games)
                        {
                            GameInfo tempGame = JsonConvert.DeserializeObject<GameInfo>(game.ToString());

                            allGames.Add(tempGame);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No games found or profile is marked private.");
                    }

                    return allGames;
                }
                else
                {
                    Console.WriteLine("Response was null, check UserName and API key");
                    return null;
                }
            }
        }

        public async Task<long> getSteamIdFromUserName(String apiKey, String userName, HttpClient client)
        {

            String queryString = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + apiKey + "&vanityUrl=" + userName;

            using (var response = await client.GetAsync(queryString))
            {
                // read response as a String and parse to Json
                String apiResponse = await response.Content.ReadAsStringAsync();
                
                dynamic parsedResponse = JsonConvert.DeserializeObject<dynamic>(apiResponse);

                if(parsedResponse.response.success == 1)
                {
                    long userId = parsedResponse.response.steamid;
                    return userId;
                }
                else
                {
                    Console.WriteLine("No Steam user with username: " + userName + " found.");
                    return 0;
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
