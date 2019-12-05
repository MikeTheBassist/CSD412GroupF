using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GroupF.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using GroupF.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GroupF.Areas.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GroupF.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

       
        
        public IActionResult Privacy()
        {
            return View();
        }

       




    }

}
