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
    public class PostulacionController : Controller
    {
        private readonly FavoresDBContext _context;

        public PostulacionController(FavoresDBContext context)
        {
            _context = context;
        }

        // GET: Postulacion
        public async Task<IActionResult> Index()
        {
            var favoresDBContext = _context.Postulaciones.Include(p => p.Favor).Include(p => p.Usuario);
            return View(await favoresDBContext.ToListAsync());
        }

        // GET: Postulacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postulacion = await _context.Postulaciones
                .Include(p => p.Favor)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postulacion == null)
            {
                return NotFound();
            }

            return View(postulacion);
        }

        // GET: Postulacion/Create
        public IActionResult Create()
        {
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Postulacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Mensaje,FavorId,UsuarioId,FechaPostulacion")] Postulacion postulacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postulacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria", postulacion.FavorId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", postulacion.UsuarioId);
            return View(postulacion);
        }

        // GET: Postulacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null)
            {
                return NotFound();
            }
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria", postulacion.FavorId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", postulacion.UsuarioId);
            return View(postulacion);
        }

        // POST: Postulacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Mensaje,FavorId,UsuarioId,FechaPostulacion")] Postulacion postulacion)
        {
            if (id != postulacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postulacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostulacionExists(postulacion.Id))
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
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria", postulacion.FavorId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", postulacion.UsuarioId);
            return View(postulacion);
        }

        // GET: Postulacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postulacion = await _context.Postulaciones
                .Include(p => p.Favor)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postulacion == null)
            {
                return NotFound();
            }

            return View(postulacion);
        }

        // POST: Postulacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion != null)
            {
                _context.Postulaciones.Remove(postulacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostulacionExists(int id)
        {
            return _context.Postulaciones.Any(e => e.Id == id);
        }
    }
}
