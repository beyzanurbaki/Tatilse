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
        public IActionResult HotelDetails(int id)
        {
            var hotel = _context.Hotels
                .Include(h => h.features)
                .Include(h => h.rooms)
                .FirstOrDefault(h => h.hotel_id == id);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
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

            var totalDays = (endDate.Value - startDate.Value).Days;
            if (totalDays <= 0)
            {
                return BadRequest("Geçerli bir tarih aralığı giriniz.");
            }

            // Otel + oda + rezervasyon bilgilerini yüklüyoruz
            var hotels = _context.Hotels
                .Include(h => h.features)
                .Include(h => h.rooms)
                    .ThenInclude(r => r.reservations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(hotelName))
            {
                hotels = hotels.Where(h => h.hotel_name.Contains(hotelName));
            }

            // Müsait odası olan otelleri filtrele
            var result = hotels
                .Where(h => h.rooms.Any(room =>
                    room.room_max_people >= guestCount &&
                    room.reservations.All(res =>
                        res.end_date <= startDate || res.start_date >= endDate)))
                .ToList();

            // Fiyat hesaplaması: oteldeki ilk uygun odayı al ve gün sayısıyla çarp
            foreach (var hotel in result)
            {
                var uygunOda = hotel.rooms.FirstOrDefault(room =>
                    room.room_max_people >= guestCount &&
                    room.reservations.All(res =>
                        res.end_date <= startDate || res.start_date >= endDate));

                if (uygunOda != null)
                {
                    hotel.hotel_price = uygunOda.room_price * totalDays;
                }
            }

            return PartialView("SearchResults", result);
        }

        [HttpPost]
        public IActionResult Calculate(int hotelId, DateTime startDate, DateTime endDate, int guestCount)
        {
            if (startDate >= endDate || guestCount <= 0)
            {
                return BadRequest("Geçerli tarih ve kişi sayısı giriniz.");
            }

            var hotel = _context.Hotels
                .Include(h => h.rooms)
                    .ThenInclude(r => r.reservations)
                .FirstOrDefault(h => h.hotel_id == hotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            int totalDays = (endDate - startDate).Days;

            var roomData = hotel.rooms.Select(r =>
            {
                bool isAvailable = r.room_max_people >= guestCount &&
                    r.reservations.All(res =>
                        res.end_date <= startDate || res.start_date >= endDate);

                return new
                {
                    r.room_id,
                    r.room_image,
                    r.room_max_people,
                    r.room_price,
                    total_price = r.room_price * totalDays,
                    isAvailable
                };
            });

            return PartialView("_RoomPricePartial", roomData);
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
