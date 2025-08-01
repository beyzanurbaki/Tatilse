using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tatilse.Data;
using Tatilse.Models.Request;

public class ReservationController : Controller
{
    private readonly DataContext _context;

    public ReservationController(DataContext context)
    {
        _context = context;
    }

    private int? GetClientId()
    {
        var clientIdString = User.FindFirstValue("client_id");
        if (string.IsNullOrWhiteSpace(clientIdString))
        {
            return null;
        }
        return int.Parse(clientIdString);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ReservationCreatePageParametersDTO parameters)
    {
        var clientId = GetClientId();
        if (clientId == null)
        {
            return RedirectToAction("Login", "Client", new
            {
                returnUrl = Url.Action("Create", "Reservation", new
                {
                    guestCount = parameters.guestCount,
                    startdate = parameters.startdate.ToString("yyyy-MM-dd"),
                    enddate = parameters.enddate.ToString("yyyy-MM-dd"),
                    roomid = parameters.roomid
                })
            });
        }

        await FillViewBagsAsync();

        // Seçilen odayı ViewBag.SelectedRoom'a koymak için:
        var selectedRoom = await _context.Rooms
            .Include(r => r.hotel)
            .FirstOrDefaultAsync(r => r.room_id == parameters.roomid);
        ViewBag.SelectedRoom = selectedRoom;

        var dto = new ReservationCreateDTO
        {
            room_id = parameters.roomid,
            start_date = parameters.startdate,
            end_date = parameters.enddate
        };

        return View(dto);
    }


    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReservationCreate(ReservationCreateDTO dto)
    {
        var clientId = GetClientId();
        if (clientId == null)
        {
            return RedirectToAction("Login", "Client", new { returnUrl = Url.Action("ReservationCreate", "Reservation") });
        }

        var room = await _context.Rooms
            .Include(r => r.reservations)
            .Include(r => r.hotel)
            .FirstOrDefaultAsync(r => r.room_id == dto.room_id);

        if (room == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await FillViewBagsAsync();
            ViewBag.SelectedRoom = room;
            return View("Create", dto);
        }

      // Seçilen tarih aralığında odaya yapılmış rezervasyonları say
      var overlappingReservationsCount = room.reservations
          .Count(r => r.start_date < dto.end_date && dto.start_date < r.end_date);

      if (overlappingReservationsCount >= room.room_quantity)
      {
          ModelState.AddModelError("", "Seçilen tarihler arasında bu odanın tüm birimleri doludur.");
            ViewBag.Alert = "Seçilen tarihler arasında bu odanın tüm birimleri doludur.";
            await FillViewBagsAsync();
            ViewBag.SelectedRoom = room;
            return View("Create", dto);
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


    [HttpGet("reservation/payment/{reservationId}")]
    public IActionResult Payment(int reservationId)
    {
        ViewBag.ReservationId = reservationId;
        return View();
    }

}
