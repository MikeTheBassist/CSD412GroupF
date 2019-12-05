using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupF.Areas.Identity;
using GroupF.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupF.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<GameUser> _signInManager;
        private readonly UserManager<GameUser> _userManager;

        public RecommendationController(ApplicationDbContext context, SignInManager<GameUser> signInManager, UserManager<GameUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/Recommendation/5
        [HttpGet(Name =nameof(GetRecommendation))]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRecommendation(string id)
        {
            if (id == null)
            {
                return Ok(new { success = false, error = "No Steam Id or Vanity Url Detected" });
            }
            var recommendationList = await  new Controllers.RecommendationController(_context,_signInManager,_userManager).GetRecommendations(id,false);

            if (recommendationList == null)
            {
                return Ok(new { success = false , error = "No account found for "+id});
            }
            var response = new
            {
                success = true,
                data = recommendationList
            };
            return Ok(response);
        }
    }
}
