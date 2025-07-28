using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tatilse.Data;

namespace Tatilse.Controllers
{

    [Authorize]
    public class ReservationController : Controller
    {
        private readonly DataContext _context;

        public ReservationController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Create()
        //{
        //    return View();
        //}

    }
}
