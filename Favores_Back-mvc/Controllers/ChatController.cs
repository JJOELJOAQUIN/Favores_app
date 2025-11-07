using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Chat
        public async Task<IActionResult> Index()
        {
            var favoresDBContext = _context.Chats.Include(c => c.Creador).Include(c => c.Ejecutor).Include(c => c.Favor);
            return View(await favoresDBContext.ToListAsync());
        }

        // GET: Chat/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats
                .Include(c => c.Creador)
                .Include(c => c.Ejecutor)
                .Include(c => c.Favor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // GET: Chat/Create
        public IActionResult Create()
        {
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email");
            ViewData["EjecutorId"] = new SelectList(_context.Usuarios, "Id", "Email");
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria");
            return View();
        }

        // POST: Chat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FavorId,CreadorId,EjecutorId,FechaCreacion")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email", chat.CreadorId);
            ViewData["EjecutorId"] = new SelectList(_context.Usuarios, "Id", "Email", chat.EjecutorId);
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria", chat.FavorId);
            return View(chat);
        }

        // GET: Chat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email", chat.CreadorId);
            ViewData["EjecutorId"] = new SelectList(_context.Usuarios, "Id", "Email", chat.EjecutorId);
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria", chat.FavorId);
            return View(chat);
        }

        // POST: Chat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FavorId,CreadorId,EjecutorId,FechaCreacion")] Chat chat)
        {
            if (id != chat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatExists(chat.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreadorId"] = new SelectList(_context.Usuarios, "Id", "Email", chat.CreadorId);
            ViewData["EjecutorId"] = new SelectList(_context.Usuarios, "Id", "Email", chat.EjecutorId);
            ViewData["FavorId"] = new SelectList(_context.Favores, "Id", "Categoria", chat.FavorId);
            return View(chat);
        }

        // GET: Chat/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats
                .Include(c => c.Creador)
                .Include(c => c.Ejecutor)
                .Include(c => c.Favor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // POST: Chat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat != null)
            {
                _context.Chats.Remove(chat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatExists(int id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }
    }
}
