using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tatilse.Data;

namespace Tatilse.Controllers
{

    public class FeatureController : Controller
    {

        private readonly DataContext _context;

        public FeatureController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Feature model)
        {
            _context.Features.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index()
        {
            var features = await
                _context.Features.ToListAsync();
            return View(features);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            //var hotel = await _context.Hotels.FindAsync(id); //sadece idye göre listelenir
            var feature = await _context.Features.FirstOrDefaultAsync(h => h.feature_id == id);  //id hariç başka şeyler de olabilir

            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Yapılacak form saldırılarına karşı koruma

        public async Task<IActionResult> Edit(int id, Feature model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Features.Any(f => f.feature_id == model.feature_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Feature");
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

            var feature = await _context.Features.FirstOrDefaultAsync(h => h.feature_id == id);
           // var feature = await _context.Features.FindAsync(id);

            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }


        [HttpPost]

        public async Task<IActionResult> Delete([FromForm] int id) //Model Binding [FromForm]
        {
            var feature = await _context.Features.FirstOrDefaultAsync(h => h.feature_id == id);
          //  var feature = await _context.Features.FindAsync(id);

            _context.Features.Remove(feature);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Feature");
        }
    }

}




