using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace GroupF.Models
{
    public class Rating
    {
        [Key]
        public int appid { get; set; }
        public int rating { get; set; }
        public float likePercentage { get; set; }
        
        public Rating(int appid, int rating, float likePercentage)
        {
            this.appid = appid;
            this.rating = rating;
            this.likePercentage = likePercentage;
        }
    }
}
