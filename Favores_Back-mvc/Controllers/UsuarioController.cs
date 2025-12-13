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
        // LISTADO (Solo Admin)
        // ============================
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "ADMIN")
            {
                TempData["Error"] = "No tienes permiso para ver usuarios.";
                return RedirectToAction("Index", "Favor");
            }

            return View(await _context.Usuarios.ToListAsync());
        }

        // ============================
        // CREAR USUARIO (GET) – SOLO ADMIN
        // ============================
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "ADMIN")
                return RedirectToAction("Index", "Login");

            return View();
        }

        // ============================
        // CREAR USUARIO (POST) – SOLO ADMIN
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombre, string email, string passwordHash, string rol)
        {
            var sessionRol = HttpContext.Session.GetString("UsuarioRol");
            if (sessionRol != "ADMIN")
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(passwordHash))
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                return View();
            }

            var usuario = new Usuario
            {
                Nombre = nombre.Trim(),
                Email = email.Trim(),
                PasswordHash = passwordHash.Trim(),
                Rol = rol.Trim(),
                FechaRegistro = DateTime.Now,
                Activo = true,
                Reputacion = 0
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // PERFIL – USUARIO LOGUEADO
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

            // actualizar sesión si cambia el email
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);

            TempData["Ok"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Perfil");
        }

        // ============================
        // ELIMINAR USUARIO (SOLO ADMIN)
        // ============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "ADMIN")
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Confirm delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "ADMIN")
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
                _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            TempData["Ok"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
