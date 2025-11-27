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

        // GET: Chat/Details/5
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

            return View(chat);
        }

        // POST: Chat/EnviarMensaje
        [HttpPost]
        public async Task<IActionResult> EnviarMensaje(int chatId, string texto)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debes iniciar sesión para enviar mensajes.";
                return RedirectToAction("Details", new { id = chatId });
            }

            var chat = await _context.Chats
                .Include(c => c.Mensajes)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                TempData["Error"] = "El chat no existe.";
                return RedirectToAction("Index", "Favor");
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
    }
}
