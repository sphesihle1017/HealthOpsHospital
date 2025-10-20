using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin,StockManager")]
    public class ConsumableRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsumableRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ConsumableRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ConsumableRequests
                .Include(r => r.Consumable)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();
            return View(requests);
        }

        // GET: ConsumableRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Request ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var request = await _context.ConsumableRequests
                .Include(r => r.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (request == null)
            {
                TempData["Error"] = "Request not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // GET: ConsumableRequests/Create
        public IActionResult Create()
        {
            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name");
            return View();
        }

        // POST: ConsumableRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumableId,WardName,QuantityRequested")] ConsumableRequest request)
        {
            if (ModelState.IsValid)
            {
                request.RequestedAt = DateTime.UtcNow;
                request.Status = "Pending";
                _context.Add(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Consumable request created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name", request.ConsumableId);
            return View(request);
        }

        // GET: ConsumableRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Request ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null)
            {
                TempData["Error"] = "Request not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name", request.ConsumableId);
            return View(request);
        }

        // POST: ConsumableRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConsumableId,WardName,QuantityRequested,Status,RequestedAt,ReceivedAt")] ConsumableRequest request)
        {
            if (id != request.Id)
            {
                TempData["Error"] = "Request ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Consumable request updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumableRequestExists(request.Id))
                    {
                        TempData["Error"] = "Request not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsumableId"] = new SelectList(_context.Consumables, "ConsumableId", "Name", request.ConsumableId);
            return View(request);
        }

        // POST: ConsumableRequests/MarkApproved/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkApproved(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null)
            {
                TempData["Error"] = "Request not found.";
                return RedirectToAction(nameof(Index));
            }

            request.Status = "Approved";
            _context.Update(request);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Request approved successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: ConsumableRequests/MarkReceived/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReceived(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null)
            {
                TempData["Error"] = "Request not found.";
                return RedirectToAction(nameof(Index));
            }

            request.Status = "Received";
            request.ReceivedAt = DateTime.UtcNow;
            _context.Update(request);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Request marked as received!";
            return RedirectToAction(nameof(Index));
        }

        // POST: ConsumableRequests/MarkRejected/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRejected(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null)
            {
                TempData["Error"] = "Request not found.";
                return RedirectToAction(nameof(Index));
            }

            request.Status = "Rejected";
            _context.Update(request);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Request rejected!";
            return RedirectToAction(nameof(Index));
        }

        // GET: ConsumableRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Request ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var request = await _context.ConsumableRequests
                .Include(r => r.Consumable)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (request == null)
            {
                TempData["Error"] = "Request not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // POST: ConsumableRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ConsumableRequests.FindAsync(id);
            if (request == null)
            {
                TempData["Error"] = "Request not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.ConsumableRequests.Remove(request);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Request deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumableRequestExists(int id)
        {
            return _context.ConsumableRequests.Any(e => e.Id == id);
        }
    }
}