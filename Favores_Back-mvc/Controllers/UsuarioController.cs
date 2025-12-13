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

        // ============================
        // PERFIL
        // ============================
        public async Task<IActionResult> Perfil()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Logout", "Login");

            return View(usuario);
        }
        // ============================
        // EDITAR PERFIL (GET)
        // ============================
        public async Task<IActionResult> EditarPerfil()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Index", "Login");

            return View(usuario);
        }

        // ============================
        // EDITAR PERFIL (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(int id, string nombre, string email, IFormFile? foto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            // Validaciones
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Los campos Nombre y Email son obligatorios.";
                return View(usuario);
            }

            // Guardar la foto si se sube una
            if (foto != null && foto.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{foto.FileName}";
                var finalPath = Path.Combine(uploadsPath, fileName);

                using var stream = new FileStream(finalPath, FileMode.Create);
                await foto.CopyToAsync(stream);

                usuario.FotoPerfil = "/uploads/" + fileName;
            }

            usuario.Nombre = nombre.Trim();
            usuario.Email = email.Trim();

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Perfil actualizado correctamente.";

            // actualizar sesión por si cambia el email
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);

            return RedirectToAction("Perfil");
        }

    }
}
