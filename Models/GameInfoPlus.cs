﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GroupF.Models
{
    public class GameInfoPlus
    {
        [Key]
        public int appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }
        public string img_icon_url { get; set; }
        public string img_logo_url { get; set; }
        public bool has_community_visible_stats { get; set; }
        public int playtime_windows_forever { get; set; }
        public int playtime_mac_forever { get; set; }
        public int playtime_linux_forever { get; set; }
        public int rating { get; set; }
        public float likePercentage { get; set; }


        public GameInfoPlus()
        {

        }

        public GameInfoPlus(GameInfo gameInfo, Rating rating)
        {
            appid = gameInfo.appid;
            name = gameInfo.name;
            playtime_forever = gameInfo.playtime_forever;
            img_icon_url = gameInfo.img_icon_url;
            img_logo_url = gameInfo.img_logo_url;
            has_community_visible_stats = gameInfo.has_community_visible_stats;
            playtime_windows_forever = gameInfo.playtime_windows_forever;
            playtime_mac_forever = gameInfo.playtime_mac_forever;
            playtime_linux_forever = gameInfo.playtime_linux_forever;
            this.rating = rating.rating;
            likePercentage = rating.likePercentage;
        }
    }
}
