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

        // GET: Favor
        public async Task<IActionResult> Index()
        {
            var favores = await _context.Favores
                .Include(f => f.Creador)
                .ToListAsync();

            return View(favores);
        }

        // GET: Favor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Favor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Descripcion,Ubicacion,Categoria,Recompensa,TipoRecompensa")] Favor favor)
        {
            if (ModelState.IsValid)
            {
                // Buscar el primer usuario disponible (temporal hasta tener sesión)
                var creador = await _context.Usuarios.FirstOrDefaultAsync();

                if (creador == null)
                {
                    ModelState.AddModelError("", "Debe existir al menos un usuario antes de crear un favor.");
                    return RedirectToAction("Create", "Usuario");
                }

                favor.CreadorId = creador.Id;
                favor.Estado = "Publicado";

                _context.Add(favor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(favor);
        }

        // GET: Favor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var favor = await _context.Favores
                .Include(f => f.Creador)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (favor == null)
                return NotFound();

            // Pasa el creador a la vista mediante ViewData
            ViewData["CreadorNombre"] = favor.Creador?.Nombre ?? "Usuario desconocido";
            ViewData["CreadorEmail"] = favor.Creador?.Email ?? "Sin correo";

            return View(favor);
        }
        // GET: Favor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var favor = await _context.Favores.FindAsync(id);
            if (favor == null)
                return NotFound();

            return View(favor);
        }

        // POST: Favor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,Ubicacion,Categoria,Recompensa,TipoRecompensa,Estado")] Favor favor)
        {
            if (id != favor.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Recuperar el registro original para conservar el CreadorId
                    var favorExistente = await _context.Favores.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

                    if (favorExistente == null)
                        return NotFound();

                    // Mantiene el creador original
                    favor.CreadorId = favorExistente.CreadorId;

                    _context.Update(favor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Favores.Any(f => f.Id == favor.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(favor);
        }


        // GET: Favor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var favor = await _context.Favores
                .Include(f => f.Creador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (favor == null)
                return NotFound();

            return View(favor);
        }

        // POST: Favor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favor = await _context.Favores.FindAsync(id);
            if (favor != null)
            {
                _context.Favores.Remove(favor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
