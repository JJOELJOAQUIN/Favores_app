using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Favores_Back_mvc.Context;
using Favores_Back_mvc.Models;

namespace Favores_Back_mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly FavoresDBContext _context;

        public LoginController(FavoresDBContext context)
        {
            _context = context;
        }

        // ============================
        // LOGIN (GET)
        // ============================
        public IActionResult Index()
        {
            return View();
        }

        // ============================
        // LOGIN (POST)
        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Completa todos los campos.";
                return View();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email.Trim() && u.PasswordHash == password);

            if (usuario == null)
            {
                TempData["Error"] = "Email o contraseña incorrectos.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol); // 🔥 AGREGADO

            return RedirectToAction("Index", "Favor");
        }

        // ============================
        // REGISTRO (GET)
        // ============================
        public IActionResult Register()
        {
            return View();
        }

        // ============================
        // REGISTRO (POST)
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string password)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                TempData["Error"] = "Ya existe un usuario con ese email.";
                return View();
            }

            var usuario = new Usuario
            {
                Nombre = nombre.Trim(),
                Email = email.Trim(),
                PasswordHash = password,
                Rol = "USER", // 🔥 Por defecto
                Activo = true,
                Reputacion = 0
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol);

            return RedirectToAction("Index", "Favor");
        }


        // ============================
        // LOGOUT
        // ============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
