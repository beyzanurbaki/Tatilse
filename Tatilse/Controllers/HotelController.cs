using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;
using Tatilse.Models;

namespace Tatilse.Controllers
{


    public class HotelController : Controller
    {
        private readonly DataContext _context;

        public HotelController(DataContext context)
        {
            _context = context;
        }
        public IActionResult HotelDetails()
        {
            //var role = HttpContext.Session.GetString("role");
            //if (role != "admin")
            //{
            //    return RedirectToAction("Login", "Client");  //eğer role admin değilse login sayfasına geri gönder
            //}

            return View();
        }

        //public async Task<IActionResult> HotelIndex()
        //{
        //    var hotels = await _context
        //        .Hotels
        //        .ToListAsync();

        //    return View(hotels);
        //}
    }
}
