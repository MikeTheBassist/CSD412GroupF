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
using System.Text;
using GroupF.Data;
using Microsoft.EntityFrameworkCore;

namespace GroupF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // TODO:  Add a 'login with Steam' interface or just have a text box you put your Steam Username into that gets passed to the Recommendations action



            return View();
        }

        public async Task<IActionResult> Recommendations(String steamUserName, String steamAPIKey)
        {
            // Create a test GameInfo object using Half Life data for testing view creation
            //GameInfo gameTest = new GameInfo();

            // Json representing Half Life data
            //String testString = "{\"appid\": 70,\"name\": \"Half-Life\",\"playtime_forever\": 100,\"img_icon_url\":" +
           //     " \"95be6d131fc61f145797317ca437c9765f24b41c\",\"img_logo_url\": \"6bd76ff700a8c7a5460fbae3cf60cb930279897d\",\"has_community_visible_stats\": " +
             //   "true,\"playtime_windows_forever\": 0,\"playtime_mac_forever\": 0,\"playtime_linux_forever\": 0}";

            //gameTest = JsonConvert.DeserializeObject<GameInfo>(testString);

            // until I figure out how to securely keep an api key in the repo, this variable is a placeholder of sorts.  
            // get a Steam API Key using your login and 127.0.0.1 as your Domain Name: steamcommunity.com/dev/apikey
            String apiKey = steamAPIKey;



            // create new HttpClient for sending and receiving information via Http protocol
            var httpClient = new HttpClient();

            String userName = steamUserName;
            ViewData["steamUserName"] = steamUserName;

            // Using my Steam ID as a placeholder, this will be replaced by the "getUserNameFromId" method once it's written...

            long steamId = await GetSteamIdFromUserName(apiKey, userName, httpClient);

            List<GameInfo> gameList = await ParseGetOwnedGamesAsync(apiKey, steamId, httpClient);
            
            List<GameInfoPlus> gameInfoPlusList = new List<GameInfoPlus>();

            
            if (steamId == 0 || gameList.Count == 0 || gameList == null)
            {
                return View();
            }
            else
            {
                gameInfoPlusList = gameInfoPlusList.OrderByDescending(o => o.playtime_forever).ToList();

                AddGameInfoToDatabase(gameList);

                foreach (var game in gameList)
                {
                    Game dbGame = _context.Game.Find(game.appid);
                    if (dbGame != null)
                    {
                        gameInfoPlusList.Add(new GameInfoPlus(game, dbGame)); //creating GameInfoPlus objects out of Game objects from the database and GameInfo Objects from the api query
                    }
                }
                // gameList = await getAppInfoFromListAsync(gameList, httpClient);
            }

            ViewData["gameList"] = gameInfoPlusList;


            return View(gameInfoPlusList);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<List<GameInfo>> ParseGetOwnedGamesAsync(String apiKey, long steamId, HttpClient client)
        {

            String queryString = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + apiKey + "&include_appinfo=true&steamid=" + steamId + "&format=json";

            using (var response = await client.GetAsync(queryString))
            {
                // read response as a String and parse to Json
                String apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
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

        public async Task<long> GetSteamIdFromUserName(String apiKey, String userName, HttpClient client)
        {

            String queryString = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + apiKey + "&vanityUrl=" + userName;

            using (var response = await client.GetAsync(queryString))
            {
                // read response as a String and parse to Json
                String apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic parsedResponse = JsonConvert.DeserializeObject<dynamic>(apiResponse);

                    if (parsedResponse != null && parsedResponse.response.success == 1)
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
                else
                {
                    Console.WriteLine("Api Key or Steam Username incorrect");
                    return 0;
                }

            }
        }

        public async Task<List<GameInfo>> GetAppInfoFromListAsync(List<GameInfo> allGames, HttpClient client)
        {
            String queryString;
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < 20; i++)
            {
                str.Append(allGames[i].appid.ToString());
                if (i != 19)
                {
                    str.Append(", ");
                }
                else
                {
                    // str.Append();
                }
            }
            queryString = "http://store.steampowered.com/api/appdetails?appids=" + str;
            
            using (var response = await client.GetAsync(queryString))
            {
                // read response as a String and parse to Json
                String apiResponse = await response.Content.ReadAsStringAsync();
                //https://store.steampowered.com/api/appdetails/?appids=311210&cc=gb&filters=metacritic
                //https://store.steampowered.com/api/appdetails/?appids=311210&cc=gb&filters=genres
                if (response.IsSuccessStatusCode)
                {
                    dynamic parsedResponse = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                    var something = parsedResponse.data;
                }
                else
                {


                }

            }
            return null;
        }

        private void AddGameInfoToDatabase(List<GameInfo> gameList)
        {
            List<Game> dbGameList = _context.Game.ToList();
            foreach (var game in gameList)
            {
                if (!dbGameList.Any(x => x.appid == game.appid))
                {
                    //idk maybe find genre and rating here if we don't have it?
                    _context.Game.Add(new Game(game, 5, "action"));
                }
            }
            _context.Database.OpenConnection();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Game ON");
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Game OFF");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
