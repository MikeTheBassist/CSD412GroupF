using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupF.Data;
using GroupF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupF.Areas.Identity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GroupF.Controllers
{
    [Route("api/[controller]")]
    public class GameListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<GameUser> _userManager;

        public GameListController(ApplicationDbContext context, UserManager<GameUser> userMgr)
        {
            _context = context;
            _userManager = userMgr;
        }

        // GET: api/<controller>
        //[Authorize]
        [HttpGet]
        public async Task<IEnumerable<Game>> Get()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);

            return _context.Game.ToList();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
