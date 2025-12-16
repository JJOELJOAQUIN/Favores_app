using Microsoft.AspNetCore.Mvc;
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

        // ============================
        // LISTADO DE FAVORES
        // ============================
        public async Task<IActionResult> Index()
        {
            var favores = await _context.Favores
                .Include(f => f.Creador)
                .ToListAsync();

            return View(favores);
        }

        // ============================
        // CREAR FAVOR (GET)
        // ============================
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (rol != "ADMIN")
            {
                TempData["Error"] = "Solo los administradores pueden crear favores.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // ============================
        // CREAR FAVOR (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Favor favor)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            if (rol != "ADMIN")
            {
                TempData["Error"] = "No tienes permiso para crear favores.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                return View(favor);

            favor.CreadorId = usuarioId.Value;
            favor.Estado = "Publicado";
            favor.FechaCreacion = DateTime.Now;

            _context.Add(favor);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Favor creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DETALLES DE FAVOR
        // ============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var favor = await _context.Favores
                .Include(f => f.Creador)
                .Include(f => f.Postulaciones).ThenInclude(p => p.Usuario)
                .Include(f => f.Chat)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (favor == null)
                return NotFound();

            return View(favor);
        }

        // ============================
        // EDITAR FAVOR (GET)
        // ============================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var favor = await _context.Favores.FindAsync(id);

            if (favor == null)
                return NotFound();

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            // Solo creador o admin puede editar
            if (favor.CreadorId != usuarioId && HttpContext.Session.GetString("UsuarioRol") != "ADMIN")
            {
                TempData["Error"] = "No tienes permiso para editar este favor.";
                return RedirectToAction("Index");
            }

            return View(favor);
        }

        // ============================
        // EDITAR FAVOR (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Favor favor)
        {
            if (id != favor.Id)
                return NotFound();

            var original = await _context.Favores.AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (original == null)
                return NotFound();

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            // Seguridad
            if (original.CreadorId != usuarioId && HttpContext.Session.GetString("UsuarioRol") != "ADMIN")
            {
                TempData["Error"] = "No puedes editar este favor.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                return View(favor);

            favor.CreadorId = original.CreadorId; // No permitir cambiar creador

            _context.Update(favor);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Favor actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // ELIMINAR FAVOR (GET)
        // ============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var favor = await _context.Favores
                .Include(f => f.Creador)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (favor == null)
                return NotFound();

            // 🔒 SOLO EL CREADOR
            if (favor.CreadorId != usuarioId)
            {
                TempData["Error"] = "Solo podés eliminar tus propios favores.";
                return RedirectToAction("Index");
            }

            return View(favor);
        }


        // ============================
        // ELIMINAR FAVOR (POST)
        // ============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var favor = await _context.Favores
                .Include(f => f.Chat)
                .Include(f => f.Postulaciones)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (favor == null)
                return RedirectToAction("Index");

            // 🔒 SOLO EL CREADOR
            if (favor.CreadorId != usuarioId)
            {
                TempData["Error"] = "Solo podés eliminar tus propios favores.";
                return RedirectToAction("Index");
            }

            // 🧹 BORRAR DEPENDENCIAS
            if (favor.Chat != null)
                _context.Chats.Remove(favor.Chat);

            if (favor.Postulaciones != null && favor.Postulaciones.Any())
                _context.Postulaciones.RemoveRange(favor.Postulaciones);

            _context.Favores.Remove(favor);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Favor eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
