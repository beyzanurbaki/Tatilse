using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;
using Tatilse.Models;
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
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
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

        public IActionResult ReservationCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReservationCreate(Reservation model)
        {
            _context.Reservations.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("ReservationIndex", "Admin");
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

            // SelectList ile dropdown doldurabilirsin (Client ve Room seçimleri için)
            ViewData["Clients"] = new SelectList(_context.Clients, "client_id", "client_name");
            ViewData["Rooms"] = new SelectList(_context.Rooms, "room_id", "room_name"); // room_name diye bir alan varsa

            return View(reservation);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservationEdit(int id, ReservationEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Clients"] = new SelectList(_context.Clients, "client_id", "client_name", model.client_id);
                ViewData["Rooms"] = new SelectList(_context.Rooms, "room_id", "room_name", model.room_id);
                return View(model);
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            reservation.start_date = model.start_date;
            reservation.end_date = model.end_date;
            reservation.client_id = model.client_id;
            reservation.room_id = model.room_id;

            await _context.SaveChangesAsync();
            return RedirectToAction("ReservationIndex");
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
