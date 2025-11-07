using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Favores_Back_mvc.Context;
using Favores_Back_mvc.Models;

namespace Favores_Back_mvc.Controllers
{
    public class FavorController : Controller
    {
        private readonly FavoresDBContext _context;

        public FavorController(FavoresDBContext context)
        {
            _context = context;
        }

        // GET: Favor
        public async Task<IActionResult> Index()
        {
            var favoresDBContext = _context.Favores.Include(f => f.Creador);
            return View(await favoresDBContext.ToListAsync());
        }

        // GET: Favor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favor = await _context.Favores
                .Include(f => f.Creador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favor == null)
            {
                return NotFound();
            }

            return View(favor);
        }

        // GET: Favor/Create
        public IActionResult Create()
        {
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Favor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,Ubicacion,Categoria,Recompensa,TipoRecompensa,Estado,CreadorId")] Favor favor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email", favor.CreadorId);
            return View(favor);
        }

        // GET: Favor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favor = await _context.Favores.FindAsync(id);
            if (favor == null)
            {
                return NotFound();
            }
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email", favor.CreadorId);
            return View(favor);
        }

        // POST: Favor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,Ubicacion,Categoria,Recompensa,TipoRecompensa,Estado,CreadorId")] Favor favor)
        {
            if (id != favor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavorExists(favor.Id))
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
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email", favor.CreadorId);
            return View(favor);
        }

        // GET: Favor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favor = await _context.Favores
                .Include(f => f.Creador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favor == null)
            {
                return NotFound();
            }

            return View(favor);
        }

        // POST: Favor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favor = await _context.Favores.FindAsync(id);
            if (favor != null)
            {
                _context.Favores.Remove(favor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavorExists(int id)
        {
            return _context.Favores.Any(e => e.Id == id);
        }
    }
}
