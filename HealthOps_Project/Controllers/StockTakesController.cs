using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using HealthOps_Project.Models;
using HealthOps_Project.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthOps_Project.Data;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin,StockManager")]
    public class StockTakesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;

        public StockTakesController(ApplicationDbContext context, IPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // GET: Stocktakes
        public async Task<IActionResult> Index()
        {
            var stocktakes = await _context.Stocktakes
                .Include(s => s.Consumable)
                .OrderByDescending(s => s.DateTaken)
                .ToListAsync();
            return View(stocktakes);
        }

        // GET: Stocktakes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Stocktake ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var stocktake = await _context.Stocktakes
                .Include(s => s.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (stocktake == null)
            {
                TempData["Error"] = "Stocktake record not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(stocktake);
        }

        // GET: Stocktakes/Create
        public IActionResult Create()
        {
            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name");
            return View();
        }

        // POST: Stocktakes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ConsumableId,WardName,QuantityCounted,DateTaken,Notes")] Stocktake stocktake)
        {
            if (ModelState.IsValid)
            {
                stocktake.DateTaken = DateTime.UtcNow;
                _context.Add(stocktake);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Stocktake recorded successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name", stocktake.ConsumableId);
            return View(stocktake);
        }

        // GET: Stocktakes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Stocktake ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var stocktake = await _context.Stocktakes.FindAsync(id);
            if (stocktake == null)
            {
                TempData["Error"] = "Stocktake record not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name", stocktake.ConsumableId);
            return View(stocktake);
        }

        // POST: Stocktakes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConsumableId,WardName,QuantityCounted,DateTaken,Notes")] Stocktake stocktake)
        {
            if (id != stocktake.Id)
            {
                TempData["Error"] = "Stocktake ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stocktake);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Stocktake updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StocktakeExists(stocktake.Id))
                    {
                        TempData["Error"] = "Stocktake record not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name", stocktake.ConsumableId);
            return View(stocktake);
        }

        // GET: Stocktakes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Stocktake ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var stocktake = await _context.Stocktakes
                .Include(s => s.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (stocktake == null)
            {
                TempData["Error"] = "Stocktake record not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(stocktake);
        }

        // POST: Stocktakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stocktake = await _context.Stocktakes.FindAsync(id);
            if (stocktake == null)
            {
                TempData["Error"] = "Stocktake record not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Stocktakes.Remove(stocktake);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Stocktake record deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Stocktakes/GenerateReport
        public async Task<IActionResult> GenerateReport()
        {
            try
            {
                var stocktakes = await _context.Stocktakes
                    .Include(s => s.Consumable)
                    .OrderByDescending(s => s.DateTaken)
                    .ToListAsync();

                if (!stocktakes.Any())
                {
                    TempData["Info"] = "No stocktakes found to generate report.";
                    return RedirectToAction(nameof(Index));
                }

                var pdf = _pdfService.GenerateStocktakeReport(stocktakes);
                TempData["Success"] = "Stocktake report PDF generated successfully!";
                return File(pdf, "application/pdf", $"StocktakeReport_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to generate stocktake report. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Stocktakes/StocktakeReport (HTML version)
        public async Task<IActionResult> StocktakeReport()
        {
            var stocktakes = await _context.Stocktakes
                .Include(s => s.Consumable)
                .OrderByDescending(s => s.DateTaken)
                .ToListAsync();

            return View(stocktakes);
        }

        private bool StocktakeExists(int id)
        {
            return _context.Stocktakes.Any(e => e.Id == id);
        }
    }
}