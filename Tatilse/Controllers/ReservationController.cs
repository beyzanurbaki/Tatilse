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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = new SelectList(await _context.Clients.ToListAsync(), "client_id", "client_name");
            ViewBag.Rooms = new SelectList(await _context.Rooms.ToListAsync(), "room_id", "room_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // POST sırasında model hatası varsa tekrar ViewBag doldur
            ViewBag.Clients = new SelectList(await _context.Clients.ToListAsync(), "client_id", "client_name");
            ViewBag.Rooms = new SelectList(await _context.Rooms.ToListAsync(), "room_id", "room_name");
            return View(reservation);
        }


    }


}
