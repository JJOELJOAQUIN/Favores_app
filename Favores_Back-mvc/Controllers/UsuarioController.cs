using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Favores_Back_mvc.Context;
using Favores_Back_mvc.Models;

namespace Favores_Back_mvc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly FavoresDBContext _context;

        public UsuarioController(FavoresDBContext context)
        {
            _context = context;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Email,PasswordHash,FotoPerfil")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Activo = true;
                usuario.Reputacion = 0;

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Favor");
            }

            return View(usuario);
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
