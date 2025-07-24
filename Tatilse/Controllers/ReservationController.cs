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

        
    }
}
