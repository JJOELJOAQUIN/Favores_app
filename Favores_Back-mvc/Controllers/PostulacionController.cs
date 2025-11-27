using Microsoft.AspNetCore.Mvc;
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
            var postulaciones = await _context.Postulaciones
                .Include(p => p.Favor)
                .Include(p => p.Usuario)
                .ToListAsync();

            return View(postulaciones);
        }

        // ============================================================
        // POSTULARSE A UN FAVOR
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Postularse(int favorId)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debes iniciar sesión.";
                return RedirectToAction("Details", "Favor", new { id = favorId });
            }

            // Evitar doble postulación
            bool yaPostulo = await _context.Postulaciones
                .AnyAsync(p => p.FavorId == favorId && p.UsuarioId == usuarioId.Value);

            if (yaPostulo)
            {
                TempData["Error"] = "Ya te postulaste a este favor.";
                return RedirectToAction("Details", "Favor", new { id = favorId });
            }

            var postulacion = new Postulacion
            {
                FavorId = favorId,
                UsuarioId = usuarioId.Value,
                Mensaje = "Me gustaría ayudarte",
                FechaPostulacion = DateTime.Now,
                Estado = "Pendiente"
            };

            _context.Postulaciones.Add(postulacion);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Postulación enviada correctamente.";
            return RedirectToAction("Details", "Favor", new { id = favorId });
        }

        // ============================================================
        // ACEPTAR POSTULACIÓN → CREA CHAT AUTOMÁTICAMENTE
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Aceptar(int id)
        {
            var postulacion = await _context.Postulaciones
                .Include(p => p.Favor)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (postulacion == null)
                return NotFound();

            if (postulacion.Favor == null || postulacion.Usuario == null)
            {
                TempData["Error"] = "La postulación no tiene datos suficientes.";
                return RedirectToAction("Index");
            }

            // 1) Cambiar estado de la postulación
            postulacion.Estado = "Aceptada";
            _context.Postulaciones.Update(postulacion);

            // 2) Cambiar estado del favor
            postulacion.Favor.Estado = "En curso";
            _context.Favores.Update(postulacion.Favor);

            // 3) Revisar si ya existe chat
            var existingChat = await _context.Chats
                .FirstOrDefaultAsync(c => c.FavorId == postulacion.FavorId);

            if (existingChat == null)
            {
                var chat = new Chat
                {
                    FavorId = postulacion.FavorId,
                    CreadorId = postulacion.Favor.CreadorId,
                    EjecutorId = postulacion.UsuarioId
                };

                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Ya existe, solo guardar cambios
                await _context.SaveChangesAsync();
            }

            TempData["Ok"] = "Postulación aceptada. Chat creado.";
            return RedirectToAction("Details", "Favor", new { id = postulacion.FavorId });
        }
    }
}
