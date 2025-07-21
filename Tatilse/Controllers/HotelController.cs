using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;

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

            ViewBag.Features = new MultiSelectList(
                _context.Features.ToList(),
                "feature_id",
                "feature_name",
                hotel.features.Select(f => f.feature_id)
            );

            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hotel model, int[] selectedFeatures)
        {
            if (id != model.hotel_id) return NotFound();

            var hotelToUpdate = await _context.Hotels
                .Include(h => h.features)
                .FirstOrDefaultAsync(h => h.hotel_id == id);

            if (hotelToUpdate == null) return NotFound();

            if (ModelState.IsValid)
            {
                hotelToUpdate.hotel_name = model.hotel_name;
                hotelToUpdate.hotel_price = model.hotel_price;
                hotelToUpdate.hotel_description = model.hotel_description;
                hotelToUpdate.hotel_image = model.hotel_image;

                // Güncellenen özellikleri ata
                hotelToUpdate.features = await _context.Features
                    .Where(f => selectedFeatures.Contains(f.feature_id))
                    .ToListAsync();

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Features = new MultiSelectList(
                _context.Features.ToList(),
                "feature_id",
                "feature_name",
                selectedFeatures
            );

            return View(model);
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
