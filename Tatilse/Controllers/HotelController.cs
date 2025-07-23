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

        public IActionResult Create()
        {
            var allFeatures = _context.Features.ToList();
            ViewBag.Features = new MultiSelectList(allFeatures, "feature_id", "feature_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel model, int[] selectedFeatures)
        {
            var selected = await _context.Features
                                         .Where(f => selectedFeatures.Contains(f.feature_id))
                                         .ToListAsync();

            model.features = selected;

            _context.Hotels.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _context
                .Hotels
                .Include(h => h.features)
                .ToListAsync();

            return View(hotels);
        }

        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(HotelEditDTO hotelEditRequest)
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

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(hotel);
                    await _context.SaveChangesAsync(); //yapılan kaydetme işlemini veri tabanına geçirir
                    return RedirectToAction("Index", "Hotel");
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
        public async Task<IActionResult> Delete(int? id)
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
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var hotel = await _context
                .Hotels
                .Include(h => h.features)
                .FirstOrDefaultAsync(h => h.hotel_id == id);

            if (hotel == null) return NotFound();

            hotel.features.Clear(); // ilişkiyi kaldır
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.features)
                .FirstOrDefaultAsync(h => h.hotel_id == id);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }
    }
}
