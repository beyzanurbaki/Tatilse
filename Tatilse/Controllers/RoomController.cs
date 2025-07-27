using Microsoft.AspNetCore.Mvc; // controllerdan kalıtım alması için
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;
using Tatilse.Models.Request;

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
            var rooms = await _context.Rooms.Include(r => r.hotel).ToListAsync(); 
            return View(rooms);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var hotels = await _context.Hotels.ToListAsync();

            var dto = new RoomEditDTO
            {
                room_id = room.room_id,
                room_name = room.room_name,
                room_price = room.room_price,
                room_quantity = room.room_quantity,
                room_capacity = room.room_capacity,
                room_max_people = room.room_max_people,
                room_image = room.room_image,
                hotel_id = room.hotel_id
            };

            ViewBag.Hotels = new SelectList(hotels, "hotel_id", "hotel_name", dto.hotel_id);

            return View(dto); 
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomEditDTO model)
        {
            if (!ModelState.IsValid)
            {

                ViewData["Hotels"] = new SelectList(_context.Hotels, "hotel_id", "hotel_name", model.hotel_id);
                return View(model);
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            room.room_name = model.room_name;
            room.room_price = model.room_price;
            room.room_quantity = model.room_quantity;
            room.room_capacity = model.room_capacity;
            room.room_max_people = model.room_max_people;
            room.room_image = model.room_image;
            room.hotel_id = model.hotel_id;


            await _context.SaveChangesAsync();
            return RedirectToAction("Index" , "Room");
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
