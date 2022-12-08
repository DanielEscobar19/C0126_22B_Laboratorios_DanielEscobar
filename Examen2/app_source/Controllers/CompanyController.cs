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
            List<CompanyModel> sortedCompanies = (await _context.CompanyModel.ToListAsync()).OrderBy(x => x.Id).ToList();
            sortedCompanies.Reverse();
            return View(sortedCompanies);
        }

        // Post: checks company availability
        [HttpPost]
        public JsonResult IsCompanyNameAvailable(string nombre, int id)
        {
            bool nombreValido = false;
            var actualCompany = _context.CompanyModel.Find(id);

            if (actualCompany == null)
            {
                // actualCompany == null significa que no existe el negocio
                // situacion de creacion de la empresa
                nombreValido = !_context.CompanyModel.Any(x => x.Nombre.ToLower() == nombre.ToLower());
            } else if(actualCompany.Nombre == nombre)
            {
                // actualCompany != null significa que si existe la empresa y se esta actualizando su nombre
                // situacion de update del nombre de la empresa
                // si el nombre en la base es igual al nuevo nombre se permite la actualizacion
                nombreValido = true;
            } else
            {
                // si el nuevo nombre no es igual al que esta en la base se debe verificar si esta disponible el nombre nuevo
                nombreValido = !_context.CompanyModel.Any(x => x.Nombre.ToLower() == nombre.ToLower());
            }
            return Json(nombreValido);
        }

        // GET: Company/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,TipoNegocio,PaisBase,ValorEstimado,EsTransnacional")] CompanyModel companyModel)
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
        public async Task<IActionResult> Edit(int? id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,TipoNegocio,PaisBase,ValorEstimado,EsTransnacional")] CompanyModel companyModel)
        {
            if (id != companyModel.Id)
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
                    if (!CompanyModelExists(companyModel.Id))
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CompanyModel == null)
            {
                return NotFound();
            }

            var companyModel = await _context.CompanyModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyModel == null)
            {
                return NotFound();
            }

            return View(companyModel);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

        private bool CompanyModelExists(int id)
        {
          return _context.CompanyModel.Any(e => e.Id == id);
        }
    }
}
