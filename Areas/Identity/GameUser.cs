using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupF.Areas.Identity
{
    public class GameUser : IdentityUser
    {
        public string SteamUsername { get; set; }

    }
}
