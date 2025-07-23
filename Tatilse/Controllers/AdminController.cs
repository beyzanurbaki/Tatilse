using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tatilse.Models;

namespace Tatilse.Controllers
{
    [Authorize(Roles = RoleDefinition.Admin)]
    public class AdminController : Controller
    {
      
        public IActionResult Dashboard()
        {
            //var role = HttpContext.Session.GetString("role");
            //if (role != "admin")
            //{
            //    return RedirectToAction("Login", "Client");  //eğer role admin değilse login sayfasına geri gönder
            //}

            return View();
        }
    }



}
