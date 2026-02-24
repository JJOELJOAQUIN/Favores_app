using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Favores_Back_mvc.Context;
using Favores_Back_mvc.Models;

namespace Favores_Back_mvc.Controllers
{
    public class ChatController : Controller
    {
        private readonly FavoresDBContext _context;

        public ChatController(FavoresDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var chat = await _context.Chats
                .Include(c => c.Favor)
                .Include(c => c.Creador)
                .Include(c => c.Ejecutor)
                .Include(c => c.Mensajes!)
                    .ThenInclude(m => m.Remitente)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
                return NotFound();

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (chat.Favor != null)
            {
                var calificaciones = await _context.Calificaciones
                    .Where(c => c.FavorId == chat.FavorId)
                    .ToListAsync();

                ViewBag.TotalCalificaciones = calificaciones.Count;

                if (calificaciones.Any())
                    ViewBag.Promedio = calificaciones.Average(c => c.Puntuacion);
                else
                    ViewBag.Promedio = 0;

                if (usuarioId != null)
                {
                    ViewBag.YaCalifico = calificaciones
                        .Any(c => c.EvaluadorId == usuarioId.Value);
                }
            }

            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje(int chatId, string texto)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var chat = await _context.Chats
                .Include(c => c.Favor)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
                return RedirectToAction("Index", "Favor");

            // BLOQUEAR MENSAJES SI ESTA CERRADO DEFINITIVO
            if (chat.Favor != null && chat.Favor.Estado == "CerradoDefinitivo")
            {
                TempData["Error"] = "El chat está cerrado definitivamente.";
                return RedirectToAction("Details", new { id = chatId });
            }

            if (string.IsNullOrWhiteSpace(texto))
            {
                TempData["Error"] = "El mensaje no puede estar vacío.";
                return RedirectToAction("Details", new { id = chatId });
            }

            var mensaje = new Mensaje
            {
                ChatId = chatId,
                RemitenteId = usuarioId.Value,
                Texto = texto,
                FechaHora = DateTime.Now
            };

            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = chatId });
        }

        [HttpPost]
        public async Task<IActionResult> CerrarFavor(int chatId)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var chat = await _context.Chats
                .Include(c => c.Favor)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
                return NotFound();

            if (chat.Favor == null)
                return BadRequest();

            // SOLO CREADOR
            if (usuarioId != chat.CreadorId)
                return Unauthorized();

            chat.Favor.Estado = "Finalizado";

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = chatId });
        }
    }
}