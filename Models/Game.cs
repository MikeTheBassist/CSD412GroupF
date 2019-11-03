using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupF.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string Name { get; set; }
        public DateTime PurchaseDate { get; set; }
        public float TimePlayed { get; set; }
    }
}
