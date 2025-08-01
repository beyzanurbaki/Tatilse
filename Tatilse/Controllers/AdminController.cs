using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;
using Tatilse.Models;
using Tatilse.Models.Request;
namespace Tatilse.Controllers
{
    [Authorize(Roles = RoleDefinition.Admin)]
    public class AdminController : Controller
    {
        private readonly DataContext _context;

        public AdminController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            //var role = HttpContext.Session.GetString("role");
            //if (role != "admin")
            //{
            //    return RedirectToAction("Login", "Client");  //eğer role admin değilse login sayfasına geri gönder
            //}

            return View();
        }

        public IActionResult HotelCreate()
        {
            var allFeatures = _context.Features.ToList();
            ViewBag.Features = new MultiSelectList(allFeatures, "feature_id", "feature_name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HotelCreate(Hotel model, IFormFile imageFile, int[] SelectedFeatureIds)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{model.hotel_name}{extension}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.hotel_image = fileName;
            }

            if (SelectedFeatureIds != null && SelectedFeatureIds.Length > 0)
            {
                var selectedFeatures = await _context.Features
                    .Where(f => SelectedFeatureIds.Contains(f.feature_id))
                    .ToListAsync();

                model.features = selectedFeatures;
            }

            _context.Hotels.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("HotelIndex");
        }

        public async Task<IActionResult> HotelIndex()
        {
            var hotels = await _context.Hotels
                .Include(h => h.features)  // Otel özelliklerini de dahil et
                .ToListAsync();

            return View(hotels);
        }

    
        public async Task<IActionResult> HotelEdit(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context.Hotels
                .Include(h => h.features)
                .FirstOrDefaultAsync(h => h.hotel_id == id);

            if (hotel == null) return NotFound();

            // DTO oluştur ve verileri doldur
            var model = new HotelEditDTO
            {
                hotel_id = hotel.hotel_id,
                hotel_name = hotel.hotel_name,
                hotel_city = hotel.hotel_city,
                hotel_township = hotel.hotel_township,
                hotel_price = hotel.hotel_price,
                hotel_description = hotel.hotel_description,
                SelectedFeatureIds = hotel.features.Select(f => f.feature_id.ToString()).ToArray()

            };

            ViewBag.Features = new MultiSelectList(_context.Features, "feature_id", "feature_name", model.SelectedFeatureIds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HotelEdit(HotelEditDTO model)
        {
            if (!_context.Hotels.Any(h => h.hotel_id == model.hotel_id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var hotel = await _context.Hotels
                    .Include(h => h.features)
                    .FirstOrDefaultAsync(h => h.hotel_id == model.hotel_id);

                if (hotel == null) return NotFound();

                // Temel bilgiler güncelleniyor
                hotel.hotel_name = model.hotel_name;
                hotel.hotel_city = model.hotel_city;
                hotel.hotel_township = model.hotel_township;
                hotel.hotel_price = model.hotel_price;
                hotel.hotel_description = model.hotel_description;

                // Yeni görsel yüklendiyse
                if (model.hotel_image != null && model.hotel_image.Length > 0)
                {
                    var extension = Path.GetExtension(model.hotel_image.FileName);
                    var fileName = model.hotel_name + extension;
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);

                    using (var stream = new FileStream(imagePath, FileMode.OpenOrCreate))
                    {
                        await model.hotel_image.CopyToAsync(stream);
                    }

                    hotel.hotel_image = fileName;
                }

                // Özellikler güncelleniyor
                var selectedFeatureIdsByte = model.SelectedFeatureIds.Select(s => byte.Parse(s)).ToList();

                var selectedFeatures = await _context.Features
                    .Where(f => selectedFeatureIdsByte.Contains(f.feature_id))
                    .ToListAsync();

                hotel.features.Clear();
                foreach (var feature in selectedFeatures)
                {
                    hotel.features.Add(feature);
                }

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("HotelIndex");
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Conflict(new { Message = "Başka biri tarafından güncellendi." });
                }
            }

            ViewBag.Features = new MultiSelectList(_context.Features, "feature_id", "feature_name", model.SelectedFeatureIds);
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> HotelDelete(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context
                .Hotels
                .Include(h => h.features)
                .FirstOrDefaultAsync(h => h.hotel_id == id);

            if (hotel == null) return NotFound();

            return View(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> HotelDelete([FromForm] int id)
        {
            var hotel = await _context
                .Hotels
                .Include(h => h.features)
                .FirstOrDefaultAsync(h => h.hotel_id == id);

            if (hotel == null) return NotFound();

            hotel.features.Clear(); // ilişkiyi kaldır
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return RedirectToAction("HotelIndex");
        }

        //public async Task<IActionResult> Details(int id)
        //{
        //    var hotel = await _context.Hotels
        //        .Include(h => h.features)
        //        .FirstOrDefaultAsync(h => h.hotel_id == id);

        //    if (hotel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hotel);
        //}

        public async Task<IActionResult> ReservationIndex()
        {
            var reservations = await _context.Reservations.ToListAsync();
            return View(reservations);
        }


        public async Task<IActionResult> ReservationEdit(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.client)
                .Include(r => r.room)
                .FirstOrDefaultAsync(r => r.reservation_id == id);

            if (reservation == null)
                return NotFound();

            var model = new ReservationEditDTO
            {
                reservation_id = reservation.reservation_id,  // DTO'da varsa
                start_date = reservation.start_date,
                end_date = reservation.end_date,
                client_id = reservation.client_id,
                room_id = reservation.room_id
            };

            ViewData["Clients"] = new SelectList(_context.Clients, "client_id", "client_name", model.client_id);
            ViewData["Rooms"] = new SelectList(_context.Rooms, "room_id", "room_name", model.room_id);

            return View(model); // DTO gönderiyoruz
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservationEdit(int id, ReservationEditDTO dto)
        {
            var room = await _context.Rooms
                .Include(r => r.reservations)
                .FirstOrDefaultAsync(r => r.room_id == dto.room_id);

            if (room == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await FillViewBagsAsync();
                ViewBag.SelectedRoom = room;
                return View(dto);
            }

            var overlappingReservationsCount = room.reservations
                .Count(r => r.reservation_id != id && r.start_date < dto.end_date && dto.start_date < r.end_date);

            if (overlappingReservationsCount >= room.room_quantity)
            {
                ModelState.AddModelError("", "Seçilen tarihler arasında bu odanın tüm birimleri doludur.");
                ViewBag.Alert = "Seçilen tarihler arasında bu odanın tüm birimleri doludur.";
                await FillViewBagsAsync();
                ViewBag.SelectedRoom = room;
                return View(dto);
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            reservation.start_date = dto.start_date;
            reservation.end_date = dto.end_date;
            reservation.client_id = dto.client_id;
            reservation.room_id = dto.room_id;

            await _context.SaveChangesAsync();
            return RedirectToAction("ReservationIndex");
        }

        private async Task FillViewBagsAsync()
        {
            var rooms = await _context.Rooms.Include(r => r.hotel).ToListAsync();
            ViewBag.Rooms = new SelectList(rooms, "room_id", "room_name");

            var clients = await _context.Clients.ToListAsync();
            ViewBag.Clients = new SelectList(clients, "client_id", "client_name");

            ViewBag.RoomDetails = rooms.Select(r => new
            {
                RoomId = r.room_id,
                RoomName = r.room_name,
                HotelName = r.hotel.hotel_name
            }).ToList();
        }



        [HttpGet]

        public async Task<IActionResult> ReservationDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }


        [HttpPost]

        public async Task<IActionResult> ReservationDelete([FromForm] int id) //Model Binding [FromForm]
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();

            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction("ReservationIndex", "Admin");
        }

    }



}
