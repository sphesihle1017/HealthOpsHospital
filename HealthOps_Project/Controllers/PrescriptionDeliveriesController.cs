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
    [Authorize(Roles = "Admin,ScriptManager")]
    public class PrescriptionDeliveriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionDeliveriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrescriptionDeliveries
        public async Task<IActionResult> Index()
        {
            var deliveries = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .ToListAsync();
            return View(deliveries);
        }

        // GET: PrescriptionDeliveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var delivery = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (delivery == null) return NotFound();
            return View(delivery);
        }

        // GET: PrescriptionDeliveries/Create
        public async Task<IActionResult> Create()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active")
                .Select(p => new {
                    p.PrescriptionId,
                    DisplayText = $"{p.Patient.FirstName} - {p.MedicationName}"
                }).ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText");
            return View();
        }

        // POST: PrescriptionDeliveries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrescriptionId,Status,RequestedAt,DeliveredAt")] PrescriptionDelivery delivery)
        {
            if (ModelState.IsValid)
            {
                delivery.RequestedAt = DateTime.UtcNow;
                delivery.Status = "Pending";
                _context.Add(delivery);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prescription delivery created successfully!";
                return RedirectToAction(nameof(Index));
            }

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active")
                .Select(p => new {
                    p.PrescriptionId,
                    DisplayText = $"{p.Patient.FirstName} - {p.MedicationName}"
                }).ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText", delivery.PrescriptionId);
            return View(delivery);
        }

        // GET: PrescriptionDeliveries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var delivery = await _context.PrescriptionDeliveries.FindAsync(id);
            if (delivery == null) return NotFound();

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active")
                .Select(p => new {
                    p.PrescriptionId,
                    DisplayText = $"{p.Patient.FirstName} - {p.MedicationName}"
                }).ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText", delivery.PrescriptionId);
            return View(delivery);
        }

        // POST: PrescriptionDeliveries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrescriptionId,Status,RequestedAt,DeliveredAt")] PrescriptionDelivery delivery)
        {
            if (id != delivery.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Prescription delivery updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionDeliveryExists(delivery.Id))
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

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active")
                .Select(p => new {
                    p.PrescriptionId,
                    DisplayText = $"{p.Patient.FirstName} - {p.MedicationName}"
                }).ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText", delivery.PrescriptionId);
            return View(delivery);
        }

        // POST: PrescriptionDeliveries/MarkDelivered/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var delivery = await _context.PrescriptionDeliveries.FindAsync(id);
            if (delivery == null) return NotFound();

            delivery.Status = "Delivered";
            delivery.DeliveredAt = DateTime.UtcNow;

            // Also update the main prescription status
            var prescription = await _context.Prescriptions.FindAsync(delivery.PrescriptionId);
            if (prescription != null)
            {
                prescription.Status = "Delivered";
                prescription.UpdatedAt = DateTime.UtcNow;
                _context.Update(prescription);
            }

            _context.Update(delivery);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Prescription marked as delivered!";
            return RedirectToAction(nameof(Index));
        }

        // GET: PrescriptionDeliveries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var delivery = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (delivery == null) return NotFound();
            return View(delivery);
        }

        // POST: PrescriptionDeliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var delivery = await _context.PrescriptionDeliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.PrescriptionDeliveries.Remove(delivery);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prescription delivery deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionDeliveryExists(int id)
        {
            return _context.PrescriptionDeliveries.Any(e => e.Id == id);
        }
    }
}