using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;

namespace Tatilse.Controllers
{
    public class ReservationController : Controller
    {
        private readonly DataContext _context;

        public ReservationController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations.ToListAsync();
            return View(reservations);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation model)
        {
            _context.Reservations.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Reservation");
        }


        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id, Reservation model)
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
                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.reservation_id == id);
            if (reservation == null) return NotFound();

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
