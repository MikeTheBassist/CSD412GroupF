using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace GroupF.Models
{
    public class Game
    {
        [Key]
        public int appid { get; set; }
        public int rating { get; set; }
        public string genre { get; set; }
        public DateTime updated { get; set; }
        public Game()
        {

        }
        public Game(GameInfo game, int rating, string genre)
        {
            this.appid = game.appid;
            this.rating = rating;
            this.genre = genre;
            this.updated = DateTime.UtcNow;
        }
    }
}
