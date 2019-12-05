using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupF.Data;
using GroupF.Models;

namespace GroupF.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Rating
        [HttpGet(Name = nameof(GetRating))]
        public async Task<ActionResult> GetRating()
        {
            var response = new
            {
                success = true,
                data = await _context.Rating.ToListAsync()
            };
            
            return Ok(response);
        }

        // GET: api/Rating/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var rating = await _context.Rating.FindAsync(id);

            if (rating == null)
            {
                return Ok(new { success = false, error = "No rating for appid "+id+" found"});
            }
            var response = new
            {
                success = true,
                data = rating
            };
            return Ok(response);
        }
        /*
        // PUT: api/Rating/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, Rating rating)
        {
            if (id != rating.appid)
            {
                return BadRequest();
            }

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Rating
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating)
        {
            _context.Rating.Add(rating);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRating", new { id = rating.appid }, rating);
        }

        // DELETE: api/Rating/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rating>> DeleteRating(int id)
        {
            var rating = await _context.Rating.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _context.Rating.Remove(rating);
            await _context.SaveChangesAsync();

            return rating;
        }

        private bool RatingExists(int id)
        {
            return _context.Rating.Any(e => e.appid == id);
        }
        */
    }
}
