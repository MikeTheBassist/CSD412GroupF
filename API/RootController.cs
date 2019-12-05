using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupF.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroupF.API
{
    [Route("api/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var response = new
            {
                href = Url.Link(nameof(GetRoot), null),
                rating = new
                {
                    href = Url.Link(nameof(RatingController.GetRating), null)
                },
                recommendation = new
                {
                    href = Url.Link(nameof(RecommendationController.GetRecommendation), null)
                }
            };
            return Ok(response);
        }
    }
}