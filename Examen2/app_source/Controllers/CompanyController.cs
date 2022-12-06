using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using app_source.Data;
using app_source.Models;

namespace app_source.Controllers
{
    public class CompanyController : Controller
    {
        private readonly CompanyContext _context;

        public CompanyController(CompanyContext context)
        {
            _context = context;
        }

        // GET: Company
        public async Task<IActionResult> Index()
        {
              return View(await _context.CompanyModel.ToListAsync());
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.CompanyModel == null)
            {
                return NotFound();
            }

            var companyModel = await _context.CompanyModel
                .FirstOrDefaultAsync(m => m.Nombre == id);
            if (companyModel == null)
            {
                return NotFound();
            }

            return View(companyModel);
        }

        // GET: Company/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,TipoNegocio,PaisBase,ValorEstimado,EsTransacional")] CompanyModel companyModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(companyModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(companyModel);
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.CompanyModel == null)
            {
                return NotFound();
            }

            var companyModel = await _context.CompanyModel.FindAsync(id);
            if (companyModel == null)
            {
                return NotFound();
            }
            return View(companyModel);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nombre,TipoNegocio,PaisBase,ValorEstimado,EsTransacional")] CompanyModel companyModel)
        {
            if (id != companyModel.Nombre)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyModelExists(companyModel.Nombre))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(companyModel);
        }

        // GET: Company/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.CompanyModel == null)
            {
                return NotFound();
            }

            var companyModel = await _context.CompanyModel
                .FirstOrDefaultAsync(m => m.Nombre == id);
            if (companyModel == null)
            {
                return NotFound();
            }

            return View(companyModel);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.CompanyModel == null)
            {
                return Problem("Entity set 'CompanyContext.CompanyModel'  is null.");
            }
            var companyModel = await _context.CompanyModel.FindAsync(id);
            if (companyModel != null)
            {
                _context.CompanyModel.Remove(companyModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyModelExists(string id)
        {
          return _context.CompanyModel.Any(e => e.Nombre == id);
        }
    }
}
