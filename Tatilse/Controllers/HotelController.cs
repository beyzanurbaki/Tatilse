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

        public IActionResult Search(string hotelName, DateTime? startDate, DateTime? endDate, int? guestCount)
        {
            List<Hotel> result = new List<Hotel>();

            if (!startDate.HasValue || !endDate.HasValue || !guestCount.HasValue)
            {
                return View("SearchResults", result);
            }

            IQueryable<Hotel> hotels = _context.Hotels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(hotelName))
            {
                hotels = hotels.Where(h => h.hotel_name.Contains(hotelName));
            }

            hotels = hotels.Where(h => h.rooms.Any(room =>
                room.room_max_people >= guestCount &&
                (room.reservations.Count(res => (res.start_date < startDate && res.end_date < endDate)
                || (res.start_date > startDate && res.end_date > endDate)) != room.room_quantity)
            ));

            result = hotels.ToList();
            return PartialView("SearchResults", result);
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
