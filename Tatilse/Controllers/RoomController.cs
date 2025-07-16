using Microsoft.AspNetCore.Mvc; // controllerdan kalıtım alması için
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;

namespace Tatilse.Controllers
{
    public class RoomController : Controller
    {
        private readonly DataContext _context;

        public RoomController(DataContext context)
        {
            _context = context;

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room model)
        {
            _context.Rooms.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }
    }
}
