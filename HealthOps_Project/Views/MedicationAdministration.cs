//using HealthOps_Project.Data;
//using HealthOps_Project.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//namespace HealthOps_Project.Controllers
//{
//    [Authorize(Roles = "Nurse,NursingSister,Doctor,Admin")]
//    public class MedicationAdministration : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public MedicationAdministration(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ==========================
//        // SCHEDULED MEDICATIONS
//        // ==========================

//        // GET: MedicationAdministration/Scheduled
//        public async Task<IActionResult> Scheduled()
//        {
//            var scheduledMeds = await _context.Set<ScheduledMedication>()
//                .Include(s => s.Medication)
//                .Where(s => s.ScheduledMedicationStatus != ScheduledMedicationStatus.Deleted)
//                .ToListAsync();

//            return View(scheduledMeds);
//        }

//        // GET: MedicationAdministration/ScheduledDetails/5
//        public async Task<IActionResult> ScheduledDetails(int? id)
//        {
//            if (id == null) return NotFound();

//            var medication = await _context.Set<ScheduledMedication>()
//                .Include(m => m.Medication)
//                .FirstOrDefaultAsync(m => m.Id == id);

//            if (medication == null) return NotFound();

//            return View(medication);
//        }

//        // GET: MedicationAdministration/CreateScheduled
//        public IActionResult CreateScheduled()
//        {
//            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
//            return View();
//        }

//        // POST: MedicationAdministration/CreateScheduled
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> CreateScheduled(ScheduledMedication model)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(model);
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Scheduled medication added successfully.";
//                return RedirectToAction(nameof(Scheduled));
//            }

//            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
//            return View(model);
//        }

//        // GET: MedicationAdministration/EditScheduled/5
//        public async Task<IActionResult> EditScheduled(int? id)
//        {
//            if (id == null) return NotFound();

//            var medication = await _context.Set<ScheduledMedication>().FindAsync(id);
//            if (medication == null) return NotFound();

//            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
//            return View(medication);
//        }

//        // POST: MedicationAdministration/EditScheduled/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> EditScheduled(int id, ScheduledMedication model)
//        {
//            if (id != model.Id) return NotFound();

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(model);
//                    await _context.SaveChangesAsync();
//                    TempData["SuccessMessage"] = "Scheduled medication updated successfully.";
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!ScheduledMedicationExists(model.Id))
//                        return NotFound();
//                    throw;
//                }
//                return RedirectToAction(nameof(Scheduled));
//            }

//            ViewData["Medications"] = _context.Set<Medication>().ToList();
//            return View(model);
//        }

//        // GET: MedicationAdministration/DeleteScheduled/5
//        public async Task<IActionResult> DeleteScheduled(int? id)
//        {
//            if (id == null) return NotFound();

//            var med = await _context.Set<ScheduledMedication>()
//                .Include(m => m.Medication)
//                .FirstOrDefaultAsync(m => m.Id == id);

//            if (med == null) return NotFound();

//            return View(med);
//        }

//        // POST: MedicationAdministration/DeleteScheduled/5
//        [HttpPost, ActionName("DeleteScheduled")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteScheduledConfirmed(int id)
//        {
//            var med = await _context.Set<ScheduledMedication>().FindAsync(id);
//            if (med != null)
//            {
//                med.ScheduledMedicationStatus = ScheduledMedicationStatus.Deleted;
//                _context.Update(med);
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Scheduled medication deleted successfully.";
//            }

//            return RedirectToAction(nameof(Scheduled));
//        }

//        // ==========================
//        // NON-SCHEDULED MEDICATIONS
//        // ==========================

//        // GET: MedicationAdministration/NonScheduled
//        public async Task<IActionResult> NonScheduled()
//        {
//            var nonScheduledMeds = await _context.Set<NonScheduledMedication>()
//                .Include(n => n.Medication)
//                .Where(n => n.Status != NonScheduledMedicationStatus.Deleted)
//                .ToListAsync();

//            return View(nonScheduledMeds);
//        }

//        // GET: MedicationAdministration/CreateNonScheduled
//        public IActionResult CreateNonScheduled()
//        {
//            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
//            return View();
//        }

//        // POST: MedicationAdministration/CreateNonScheduled
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> CreateNonScheduled(NonScheduledMedication model)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(model);
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Non-scheduled medication added successfully.";
//                return RedirectToAction(nameof(NonScheduled));
//            }

//            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
//            return View(model);
//        }

//        // GET: MedicationAdministration/DeleteNonScheduled/5
//        public async Task<IActionResult> DeleteNonScheduled(int? id)
//        {
//            if (id == null) return NotFound();

//            var med = await _context.Set<NonScheduledMedication>()
//                .Include(m => m.Medication)
//                .FirstOrDefaultAsync(m => m.Id == id);

//            if (med == null) return NotFound();

//            return View(med);
//        }

//        // POST: MedicationAdministration/DeleteNonScheduled/5
//        [HttpPost, ActionName("DeleteNonScheduled")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteNonScheduledConfirmed(int id)
//        {
//            var med = await _context.Set<NonScheduledMedication>().FindAsync(id);
//            if (med != null)
//            {
//                med.Status = NonScheduledMedicationStatus.Deleted;
//                _context.Update(med);
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Non-scheduled medication deleted successfully.";
//            }

//            return RedirectToAction(nameof(NonScheduled));
//        }

//        // ==========================
//        // HELPER METHOD
//        // ==========================
//        private bool ScheduledMedicationExists(int id)
//        {
//            return _context.Set<ScheduledMedication>().Any(e => e.Id == id);
//        }
//    }
//}

