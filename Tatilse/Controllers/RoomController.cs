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
            ViewBag.Hotels = _context.Hotels.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room model)
        {
            _context.Rooms.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Room");
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.room_id == id);
            if (room == null)
            {
                return NotFound();
            }

            var Room = await _context.Rooms.FirstOrDefaultAsync(r => r.room_id == id);
            return View(room);


        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Yapılacak form saldırılarına karşı koruma

        public async Task<IActionResult> Edit(int id, Room model)
        {
            if (id != model.room_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();

                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Rooms.Any(r => r.room_id == model.room_id))
                    {
                        return NotFound();
                    }

                    else
                    {
                        throw;
                    }

                }
                return RedirectToAction("Index", "Room");

            }

            return View(model);
        }



        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


        [HttpPost]

        public async Task<IActionResult> Delete([FromForm] int id) //Model Binding [FromForm]
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();

            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Room");
        }




    }
}
