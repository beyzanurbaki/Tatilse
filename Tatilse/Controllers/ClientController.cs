using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginRequest loginRequest, string? returnUrl = null)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c =>
                    c.client_username == loginRequest.client_username &&
                    c.client_passw == loginRequest.client_password);

            if (client != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, client.client_username),
            new Claim("fullname", client.client_name + " " + client.client_surname),
            new Claim("client_id", client.client_id.ToString()),
        };

                if (client.isAdmin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, RoleDefinition.Admin));
                }

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("MyCookieAuth", principal);

                HttpContext.Session.SetInt32("client_id", client.client_id);

                return Json(new
                {
                    success = true,
                    redirectUrl = !string.IsNullOrEmpty(returnUrl)
                        ? returnUrl
                        : Url.Action("Index", "Hotel"),
                    isAdmin = client.isAdmin
                });
            }

            return Json(new { success = false, message = "Kullanıcı adı veya şifre hatalı." });
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Client");
        }

        [Authorize(Roles = RoleDefinition.Admin)]
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync();
            return View(clients);
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(Client model)
        {
            // Kullanıcı adı daha önce alınmış mı kontrol et
            bool usernameExists = _context.Clients.Any(c => c.client_username == model.client_username);

            if (usernameExists)
            {
                ModelState.AddModelError("client_username", "Bu kullanıcı adı zaten kullanılıyor.");
                return View(model); // Aynı sayfayı modelle geri döner
            }

            if (ModelState.IsValid)
            {
                _context.Clients.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Authorize(Roles = RoleDefinition.Admin)]
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
        [Authorize(Roles = RoleDefinition.Admin)]
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
        [Authorize(Roles = RoleDefinition.Admin)]
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
        [Authorize(Roles = RoleDefinition.Admin)]
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
