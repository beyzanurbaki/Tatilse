using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;
using System.Collections.Generic;
using System.Linq;

namespace Tatilse.Controllers
{
    public class HotelController : Controller
    {
        private readonly DataContext _context;

        public HotelController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .Include(h => h.rooms)
                .Include(h => h.features)
                .ToListAsync();

            foreach (var hotel in hotels)
            {
                hotel.hotel_price = hotel.rooms.Any() ? hotel.rooms.Min(r => r.room_price) : 0;
            }

            // Özellik listesini ViewBag ile gönderiyoruz (filtre paneli için)
            ViewBag.Features = await _context.Features.ToListAsync();

            return View(hotels);
        }

        [HttpPost]
        public IActionResult Search(string hotelName, DateTime? startDate, DateTime? endDate, int? guestCount)
        {
            if (!startDate.HasValue || !endDate.HasValue || !guestCount.HasValue)
                return BadRequest("Lütfen tarih ve misafir sayısı bilgilerini giriniz.");

            if ((endDate.Value - startDate.Value).Days <= 0)
                return BadRequest("Geçerli bir tarih aralığı giriniz.");

            var totalDays = (endDate.Value - startDate.Value).Days;

            var hotels = _context.Hotels
                .Include(h => h.features)
                .Include(h => h.rooms)
                    .ThenInclude(r => r.reservations)
                .AsQueryable();

            if (!string.IsNullOrEmpty(hotelName))
                hotels = hotels.Where(h => h.hotel_name.Contains(hotelName));

            var filteredHotels = hotels
                .Where(h => h.rooms.Any(room =>
                    room.room_max_people >= guestCount &&
                    room.reservations.Count(res =>
                        //res.end_date <= startDate || res.start_date >= endDate)
                        !(res.end_date < startDate || res.start_date > endDate)
                    ) < room.room_quantity
                    ))
                .ToList();

            foreach (var hotel in filteredHotels)
            {
                var suitableRoom = hotel.rooms.FirstOrDefault(room =>
                    room.room_max_people >= guestCount &&
                    room.reservations.Count(res =>
                        //res.end_date <= startDate || res.start_date >= endDate)
                        !(res.end_date < startDate || res.start_date > endDate)
                    ) < room.room_quantity
                    );

                hotel.hotel_price = suitableRoom != null ? suitableRoom.room_price * totalDays : 0;
            }

            return PartialView("SearchResults", filteredHotels);
        }

        [HttpPost]
        public IActionResult FilterByFeatures([FromForm] List<byte> featureIds)
        {
            if (featureIds == null || !featureIds.Any())
            {
                // Eğer hiç özellik seçilmediyse, tüm otelleri dön
                var allHotels = _context.Hotels
                    .Include(h => h.features)
                    .Include(h => h.rooms)
                    .ToList();

                foreach (var hotel in allHotels)
                {
                    hotel.hotel_price = hotel.rooms.Any() ? hotel.rooms.Min(r => r.room_price) : 0;
                }

                return PartialView("SearchResults", allHotels);
            }

            var filteredHotels = _context.Hotels
                .Include(h => h.features)
                .Include(h => h.rooms)
                .Where(h => featureIds.All(fId => h.features.Any(f => f.feature_id == fId)))
                .ToList();

            foreach (var hotel in filteredHotels)
            {
                hotel.hotel_price = hotel.rooms.Any() ? hotel.rooms.Min(r => r.room_price) : 0;
            }

            return PartialView("SearchResults", filteredHotels);
        }
        [HttpPost]
        public IActionResult Calculate(int hotelId, DateTime startDate, DateTime endDate, int guestCount)
        {
            if (startDate >= endDate || guestCount <= 0)
                return BadRequest("Geçerli tarih ve kişi sayısı giriniz.");

            var hotel = _context.Hotels
                .Include(h => h.rooms)
                .FirstOrDefault(h => h.hotel_id == hotelId);

            if (hotel == null)
                return NotFound();

            int totalDays = (endDate - startDate).Days;

            var roomData = hotel.rooms.Select(r =>
            {
                bool isAvailable = r.room_max_people >= guestCount; // İstersen rezervasyon kontrolleri eklenebilir

                return new
                {
                    r.room_id,
                    r.room_image,
                    r.room_name,
                    r.room_max_people,
                    total_price = r.room_price * totalDays,
                    isAvailable
                };
            }).ToList();

            return PartialView("_RoomPricePartial", roomData);
        }

        public IActionResult HotelDetails(int id)
        {
            var hotel = _context.Hotels
                .Include(h => h.features)
                .Include(h => h.rooms)
                .FirstOrDefault(h => h.hotel_id == id);

            if (hotel == null)
                return NotFound();

            return View(hotel);
        }
    }
}
