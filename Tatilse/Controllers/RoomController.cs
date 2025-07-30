using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create(RoomCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Hotels = _context.Hotels.ToList();
                return View(model);
            }

            var room = new Room
            {
                room_name = model.room_name,
                room_price = model.room_price,
                room_quantity = model.room_quantity,
                room_capacity = model.room_capacity,
                room_max_people = model.room_max_people,
                hotel_id = model.hotel_id
            };

            // Görsel yüklendiyse kaydet
            if (model.room_image != null && model.room_image.Length > 0)
            {
                var extension = Path.GetExtension(model.room_image.FileName);
                var fileName = $"{model.room_name}{extension}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "room", fileName);

                using (var stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    await model.room_image.CopyToAsync(stream);
                }

                room.room_image = fileName;
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Room");
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.Include(r => r.hotel).ToListAsync();
            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var dto = new RoomEditDTO
            {
                room_id = room.room_id,
                room_name = room.room_name,
                room_price = room.room_price,
                room_quantity = room.room_quantity,
                room_capacity = room.room_capacity,
                room_max_people = room.room_max_people,
                hotel_id = room.hotel_id
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomEditDTO model)
        {
            if (id != model.room_id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            // Alanları güncelle
            room.room_name = model.room_name;
            room.room_price = model.room_price;
            room.room_quantity = model.room_quantity;
            room.room_capacity = model.room_capacity;
            room.room_max_people = model.room_max_people;

            // Görsel yüklendiyse kaydet
            if (model.room_image != null)
            {
                var fileName = model.room_name + Path.GetExtension(model.room_image.FileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "room");

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await model.room_image.CopyToAsync(stream);
                }

                //room.room_image = "~/img/room/" + fileName;
                room.room_image = fileName;
            }

            _context.Update(room);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Delete([FromForm] int id)
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
