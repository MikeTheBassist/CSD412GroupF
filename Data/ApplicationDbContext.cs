using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GroupF.Models;

namespace GroupF.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GroupF.Models.Rating> Rating { get; set; }
        public DbSet<GroupF.Areas.Identity.GameUser> GameUser { get; set; }
        public DbSet<GroupF.Models.GameInfoPlus> GameInfoPlus { get; set; }
    }
}
