using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; //session işlemleri için
using Tatilse.Data;
using Tatilse.Models;


namespace Tatilse.Controllers
{
    public class ClientController : Controller
    {
        private readonly DataContext _context;
        public ClientController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Login([FromForm]LoginRequest loginRequest)
        //{
        //    return Ok();
        //    //return View();
        //}
        //[HttpPost]

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginRequest loginRequest)
        {
            if (loginRequest.client_username == "admin" && loginRequest.client_password == "admin1234*")
            {
                HttpContext.Session.SetString("client_username", "admin");
                HttpContext.Session.SetString("role", "admin");
                HttpContext.Session.SetString("fullname", "Admin"); // eklenen kısım
                return Json(new { success = true, isAdmin = true });
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(c =>
                    c.client_username == loginRequest.client_username &&
                    c.client_passw == loginRequest.client_password);

            if (client != null)
            {
                HttpContext.Session.SetString("client_username", client.client_username);
                HttpContext.Session.SetString("role", "user");
                HttpContext.Session.SetString("fullname", client.client_name + " " + client.client_surname);

                return Json(new { success = true, isAdmin = false });
         
            }

            return Json(new { success = false, message = "Kullanıcı adı veya şifre hatalı." });
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // tüm sessionları sil
            return RedirectToAction("Login", "Client");
        }


        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync();
            return View(clients);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client model)
        {
            _context.Clients.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FirstOrDefaultAsync(h => h.client_id == id);
            if (client == null)
            {
                return NotFound();
            }

            var Client = await _context.Clients.FirstOrDefaultAsync(h => h.client_id == id);
            return View(client);


        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Yapılacak form saldırılarına karşı koruma

        public async Task<IActionResult> Edit(int id, Client model)
        {
            if (id != model.client_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync(); //yapılan kaydetme işlemini veri tabanına geçirir
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Clients.Any(h => h.client_id == model.client_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Client");
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

            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }


        [HttpPost]

        public async Task<IActionResult> Delete([FromForm] int id) //Model Binding [FromForm]
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();

            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
