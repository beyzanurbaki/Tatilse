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


        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .Include(h => h.rooms)
                .Include(h=> h.features)
                .ToListAsync();

            foreach (var hotel in hotels)
            {
                if (hotel.rooms.Any())
                {
                    hotel.hotel_price = hotel.rooms.Min(r => r.room_price); // en ucuz oda fiyatı
                }
                else
                {
                    hotel.hotel_price = 0;
                }
            }

            return View(hotels);
        }

        public IActionResult Search(string hotelName, DateTime? startDate, DateTime? endDate, int? guestCount)
        {
            if (!startDate.HasValue || !endDate.HasValue || !guestCount.HasValue)
            {
                return BadRequest("Giriş ve çıkış tarihi seçiniz, misafir sayısını giriniz.");
            }

            // Otel özelliklerini ve odaları (ve rezervasyonlarını) birlikte yükle
            IQueryable<Hotel> hotels = _context.Hotels
                .Include(h => h.features)
                .Include(h => h.rooms)
                    .ThenInclude(r => r.reservations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(hotelName))
            {
                hotels = hotels.Where(h => h.hotel_name.Contains(hotelName));
            }

            // Müsait odası olan otelleri filtrele
            hotels = hotels.Where(h => h.rooms.Any(room =>
                room.room_max_people >= guestCount &&
                room.reservations.All(res =>
                    res.end_date <= startDate || res.start_date >= endDate // çakışmayan rezervasyonlar
                )
            ));

            var result = hotels.ToList();
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
