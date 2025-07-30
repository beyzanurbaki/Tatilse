using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tatilse.Data;
using Tatilse.Models.Request;

namespace Tatilse.Controllers
{
    public class ReservationController : Controller
    {
        private readonly DataContext _context;

        public ReservationController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ReservationCreate(int? roomId)
        {
            var clientId = HttpContext.Session.GetInt32("client_id");
            if (clientId == null)
            {
                var returnUrl = Url.Action("ReservationCreate", "Reservation", new { roomId });
                return RedirectToAction("Login", "Client", new { returnUrl });
            }

            var rooms = _context.Rooms.Include(r => r.hotel).Include(r => r.reservations).ToList();

            ViewBag.Rooms = new SelectList(rooms, "room_id", "room_name");

            // Admin için client dropdown'ı
            if (User.IsInRole("Admin"))
            {
                var clients = _context.Clients.ToList();
                ViewBag.Clients = new SelectList(clients, "client_id", "client_username");
            }

            ViewBag.RoomDetails = rooms.Select(r => new
            {
                RoomId = r.room_id,
                RoomName = r.room_name,
                HotelName = r.hotel.hotel_name
            }).ToList();

            ReservationCreateDTO dto;

            if (roomId.HasValue)
            {
                var selectedRoom = rooms.FirstOrDefault(r => r.room_id == roomId.Value);
                if (selectedRoom == null)
                    return NotFound();

                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                bool isAvailable = selectedRoom.reservations.All(r =>
                    r.end_date <= today || r.start_date >= tomorrow);

                if (!isAvailable)
                {
                    TempData["Error"] = "Bu oda seçilen tarihlerde müsait değil.";
                    return RedirectToAction("Details", "Hotel", new { id = selectedRoom.hotel_id });
                }

                dto = new ReservationCreateDTO
                {
                    room_id = roomId.Value,
                    start_date = today,
                    end_date = tomorrow
                };
            }
            else
            {
                dto = new ReservationCreateDTO
                {
                    start_date = DateTime.Today,
                    end_date = DateTime.Today.AddDays(1)
                };
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservationCreate(ReservationCreateDTO dto)
        {
            var clientId = HttpContext.Session.GetInt32("client_id");
            if (clientId == null)
                return RedirectToAction("Login", "Client");

            var room = await _context.Rooms
                .Include(r => r.reservations)
                .Include(r => r.hotel)
                .FirstOrDefaultAsync(r => r.room_id == dto.room_id);

            if (room == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await FillViewBagsAsync();
                return View(dto);
            }

            // Müsaitlik kontrolü
            bool isAvailable = room.reservations.All(r =>
                r.end_date <= dto.start_date || r.start_date >= dto.end_date);

            if (!isAvailable)
            {
                ModelState.AddModelError("", "Seçilen tarihlerde bu oda rezerve edilmiştir.");
                await FillViewBagsAsync();
                return View(dto);
            }

            var reservation = new Reservation
            {
                client_id = clientId.Value,
                start_date = dto.start_date,
                end_date = dto.end_date,
                room_id = dto.room_id
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", new { id = reservation.reservation_id });
        }

        private async Task FillViewBagsAsync()
        {
            var rooms = await _context.Rooms.Include(r => r.hotel).ToListAsync();
            ViewBag.Rooms = new SelectList(rooms, "room_id", "room_name");

            if (User.IsInRole("Admin"))
            {
                var clients = await _context.Clients.ToListAsync();
                ViewBag.Clients = new SelectList(clients, "client_id", "client_username");
            }

            ViewBag.RoomDetails = rooms.Select(r => new
            {
                RoomId = r.room_id,
                RoomName = r.room_name,
                HotelName = r.hotel.hotel_name
            }).ToList();
        }


        public IActionResult Payment(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.room)
                .ThenInclude(r => r.hotel)
                .Include(r => r.client)
                .FirstOrDefault(r => r.reservation_id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

    }
}
