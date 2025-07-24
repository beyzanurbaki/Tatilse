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
        public async Task<IActionResult> HotelCreate(Hotel model, int[] selectedFeatures)
        {
            var selected = await _context.Features
                                         .Where(f => selectedFeatures.Contains(f.feature_id))
                                         .ToListAsync();

            model.features = selected;

            _context.Hotels.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("HotelIndex");
        }

        public async Task<IActionResult> HotelIndex()
        {
            var hotels = await _context
                .Hotels
                .Include(h => h.features)
                .ToListAsync();

            return View(hotels);
        }

        public async Task<IActionResult> HotelEdit(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context
                .Hotels
                .Include(h => h.features)
                .FirstOrDefaultAsync(h => h.hotel_id == id);

            if (hotel == null) return NotFound();

            //ViewBag.Features = new MultiSelectList(
            //    _context.Features.ToList(),
            //    "feature_id",
            //    "feature_name",
            //    hotel.features.Select(f => f.feature_id)
            //);

            return View(hotel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> HotelEdit(HotelEditDTO hotelEditRequest)
        {
            //if (id != model.hotel_id)
            //{
            //    return NotFound();
            //}

            if (!_context.Hotels.Any(h => h.hotel_id == hotelEditRequest.hotel_id))
            {
                return NotFound();
            }

            Hotel hotel = _context.Hotels
                          .Where(x => x.hotel_id == hotelEditRequest.hotel_id)
                          .First();
            //Hotel hotel = new Hotel(hotelEditRequest.hotel_id);
            //_context.Attach(hotel);
            hotel.hotel_description = hotelEditRequest.hotel_description;
            hotel.hotel_price = hotelEditRequest.hotel_price;
            hotel.hotel_name = hotelEditRequest.hotel_name;
            hotel.hotel_city = hotelEditRequest.hotel_city;
            hotel.hotel_township = hotelEditRequest.hotel_township;

            if (hotelEditRequest.hotel_image != null && hotelEditRequest.hotel_image.Length > 0)
            {
                // Dosya adını al (örneğin mert.png)
                //var fileName = Path.GetFileName(hotel.hotel_image.FileName);
                var extension = Path.GetExtension(hotelEditRequest.hotel_image.FileName); // .png

                // wwwroot/img klasörü yolu
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", hotel.hotel_name + extension);

                // Dosyayı wwwroot/img içine kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hotelEditRequest.hotel_image.CopyToAsync(stream);
                }

                // Veritabanına kaydedilecek dosya adı (örneğin: mert.png)
                //hotel.hotel_image = fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(hotel);
                    await _context.SaveChangesAsync(); //yapılan kaydetme işlemini veri tabanına geçirir
                    return RedirectToAction("HotelIndex", "Admin");
                }

                catch (DbUpdateConcurrencyException ex)
                {
                    return Conflict(new
                    {
                        Message = "Başka birisi tarafından güncellendi."
                    });
                }
            }

            return Json(new
            {
                Message = "Parametreleri kontrol ediniz."
            });
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
        public async Task<IActionResult> ReservationEdit(int id, Reservation model)
        {
            if (id != model.reservation_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reservations.Any(r => r.reservation_id == model.reservation_id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("ReservationIndex");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ReservationDelete(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.reservation_id == id);
            if (reservation == null) return NotFound();

            return View(reservation);
        }

        [HttpPost, ActionName("ReservationDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservationDeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("ReservationIndex");
        }
    }



}
