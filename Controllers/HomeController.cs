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
using System.Threading;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GroupF.Areas.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GroupF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<GameUser> _signInManager;
        private readonly UserManager<GameUser> _userManager;
        public const int MAX_NEW_RATINGS = 50;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, SignInManager<GameUser> signInManager, UserManager<GameUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [Authorize] 
        public async Task<IActionResult> Recommendations(String steamUserName = null)
        {
            if (steamUserName == null)
               steamUserName =  (await _userManager.GetUserAsync(User)).SteamUsername;
            // get a Steam API Key using your login and 127.0.0.1 as your Domain Name: steamcommunity.com/dev/apikey
            String apiKey = Environment.GetEnvironmentVariable("Steam_API_Key");

            // create new HttpClient for sending and receiving information via Http protocol
            var httpClient = new HttpClient();

            String userName = steamUserName;

            ViewData["steamUserName"] = userName;

            long steamId;

            // treat the incoming input as a SteamID if it is successfully parsed to a long
            if (!long.TryParse(string.Format("{0}", userName), out steamId))
            {
                steamId = await GetSteamIdFromUserName(apiKey, userName, httpClient);
            }

            List<GameInfo> gameList = await ParseGetOwnedGamesAsync(apiKey, steamId, httpClient);

            List<GameInfoPlus> gameInfoPlusList = new List<GameInfoPlus>();
            List<GameInfoPlus> recommendations = new List<GameInfoPlus>();

            if (steamId == 0 || gameList.Count == 0 || gameList == null)
            {
                return View();
            }
            else
            {
                await AddGameInfoToDatabase(gameList);

                foreach (var game in gameList)
                {
                    Rating rating = _context.Rating.Find(game.appid);
                    if (rating != null)//get every game in the database that the player owns and we have information for
                    {
                        if (rating.likePercentage > 0 && game.playtime_forever < (4 * 60))
                        {
                            gameInfoPlusList.Add(new GameInfoPlus(game, rating));
                        }
                    }
                }

                // Sort remaining list by rating
                gameInfoPlusList.Sort(new CompareByRatingDescending());

                int listCount = 20;

                if (gameInfoPlusList.Count < listCount)
                {
                    listCount = gameInfoPlusList.Count;
                }

                for (int i = 0; i < listCount; i++)
                {

                    recommendations.Add(gameInfoPlusList[i]);

                }
            }

            ViewData["gameList"] = recommendations;

            return View(recommendations);

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

            String queryString = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key=" + apiKey + "&vanityUrl=" + userName;

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
                //https://store.steampowered.com/api/appdetails/?appids=311210&cc=gb&filters=metacritic,genres

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

        private async Task<bool> AddGameInfoToDatabase(List<GameInfo> gameList, int MaxRatingsToAdd = MAX_NEW_RATINGS)
        {
            HttpClient client = new HttpClient();

            List<Rating> ratingList = _context.Rating.ToList();

            var ratingsToAdd = new List<Rating>();
            gameList = gameList.OrderBy(o => o.playtime_forever).ToList();
            foreach (var game in gameList)
            {
                Rating rating = ratingList.Find(x => x.appid == game.appid);
                if (rating == null)
                {
                    ratingsToAdd.Add(await GetGameRating(game, client)); //if the game is not in the db add it
                }
                if (ratingsToAdd.Count >= MaxRatingsToAdd)
                {
                    break;
                }
            }
            _context.Rating.AddRange(ratingsToAdd);
            _context.Database.OpenConnection();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Rating ON");
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Rating OFF");
            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<Rating> GetGameRating(GameInfo game, HttpClient client)
        {

            string queryString = "https://store.steampowered.com/appreviews/" + game.appid + "?json=1&num_per_page=0";
            var apiResponse = await client.GetAsync(queryString);
            float likePercentage = 0;
            int rating = 0;

            if (apiResponse != null && apiResponse.IsSuccessStatusCode)
            {
                string response = await apiResponse.Content.ReadAsStringAsync();
                var parsedResponse = JsonConvert.DeserializeObject<dynamic>(response);
                try
                {
                    rating = parsedResponse.query_summary.review_score;
                }
                catch (Exception e)
                {

                }
                try
                {
                    if ((float)(parsedResponse.query_summary.total_reviews) != 0)
                        likePercentage = (int)Math.Floor((float)(parsedResponse.query_summary.total_positive) / (float)(parsedResponse.query_summary.total_reviews) * 100);
                }
                catch (Exception e)
                {

                }
            }
            return new Rating(game.appid, rating, likePercentage);
        }




    }

}
