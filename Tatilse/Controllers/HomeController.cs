using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Tatilse.Data;
using Tatilse.Models;

namespace Tatilse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels.ToListAsync();

            bool showAd = false;
            if (HttpContext.Session.GetInt32("client_id") is int clientId)
            {
                var client = await _context.Clients.FindAsync(clientId);
                if (client != null && client.client_gender == true)
                {
                    showAd = true; // kadýnsa
                }
            }

            ViewBag.ShowAd = showAd;

            return View(hotels);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
