using Microsoft.AspNetCore.Mvc;

namespace Tatilse.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "Client");  //eğer role admin değilse login sayfasına geri gönder
            }

            return View();
        }
    }



}
