using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Favores_Back_mvc.Context;
using Favores_Back_mvc.Models;

namespace Favores_Back_mvc.Controllers
{
    public class CalificacionController : Controller
    {
        private readonly FavoresDBContext _context;

        public CalificacionController(FavoresDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(int favorId, int puntuacion, string? comentario)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var favor = await _context.Favores
                .Include(f => f.Chat)
                .FirstOrDefaultAsync(f => f.Id == favorId);

            if (favor == null || favor.Estado != "Finalizado")
                return BadRequest();

            if (favor.Chat == null)
                return BadRequest();

            int evaluadoId;

            if (usuarioId == favor.Chat.CreadorId)
                evaluadoId = favor.Chat.EjecutorId;
            else if (usuarioId == favor.Chat.EjecutorId)
                evaluadoId = favor.Chat.CreadorId;
            else
                return Unauthorized();

            // Evitar doble calificación
            var yaCalifico = await _context.Calificaciones.AnyAsync(c =>
                c.FavorId == favorId && c.EvaluadorId == usuarioId);

            if (yaCalifico)
                return RedirectToAction("Details", "Chat", new { id = favor.Chat.Id });

            var calificacion = new Calificacion
            {
                FavorId = favorId,
                EvaluadorId = usuarioId.Value,
                EvaluadoId = evaluadoId,
                Puntuacion = puntuacion,
                Comentario = comentario,
                Fecha = DateTime.Now
            };

            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();

            // Verificar si ya hay 2 calificaciones
            var total = await _context.Calificaciones
                .CountAsync(c => c.FavorId == favorId);

            if (total >= 2)
            {
                favor.Estado = "CerradoDefinitivo";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Chat", new { id = favor.Chat.Id });
        }
    }
}