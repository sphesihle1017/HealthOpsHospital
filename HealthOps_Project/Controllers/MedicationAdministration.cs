using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Nurse,NursingSister,Doctor,Admin")]
    public class MedicationAdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicationAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // SCHEDULED MEDICATIONS
        // ==========================

        // GET: MedicationAdministration/Scheduled
        public async Task<IActionResult> Scheduled()
        {
            var scheduledMeds = await _context.Set<ScheduledMedication>()
                .Include(s => s.Medication)
                .Where(s => s.ScheduledMedicationStatus != ScheduledMedicationStatus.Deleted)
                .ToListAsync();

            return View(scheduledMeds);
        }

        // GET: MedicationAdministration/ScheduledDetails/5
        public async Task<IActionResult> ScheduledDetails(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Set<ScheduledMedication>()
                .Include(m => m.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medication == null) return NotFound();

            return View(medication);
        }

        // GET: MedicationAdministration/CreateScheduled
        public IActionResult CreateScheduled()
        {
            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
            return View();
        }

        // POST: MedicationAdministration/CreateScheduled
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateScheduled(ScheduledMedication model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Scheduled medication added successfully.";
                return RedirectToAction(nameof(Scheduled));
            }

            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
            return View(model);
        }

        // GET: MedicationAdministration/EditScheduled/5
        public async Task<IActionResult> EditScheduled(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Set<ScheduledMedication>().FindAsync(id);
            if (medication == null) return NotFound();

            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
            return View(medication);
        }

        // POST: MedicationAdministration/EditScheduled/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditScheduled(int id, ScheduledMedication model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Scheduled medication updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduledMedicationExists(model.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Scheduled));
            }

            ViewData["Medications"] = _context.Set<Medication>().ToList();
            return View(model);
        }

        // GET: MedicationAdministration/DeleteScheduled/5
        public async Task<IActionResult> DeleteScheduled(int? id)
        {
            if (id == null) return NotFound();

            var med = await _context.Set<ScheduledMedication>()
                .Include(m => m.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (med == null) return NotFound();

            return View(med);
        }

        // POST: MedicationAdministration/DeleteScheduled/5
        [HttpPost, ActionName("DeleteScheduled")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScheduledConfirmed(int id)
        {
            var med = await _context.Set<ScheduledMedication>().FindAsync(id);
            if (med != null)
            {
                med.ScheduledMedicationStatus = ScheduledMedicationStatus.Deleted;
                _context.Update(med);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Scheduled medication deleted successfully.";
            }

            return RedirectToAction(nameof(Scheduled));
        }

        // ==========================
        // NON-SCHEDULED MEDICATIONS
        // ==========================

        // GET: MedicationAdministration/NonScheduled
        public async Task<IActionResult> NonScheduled()
        {
            var nonScheduledMeds = await _context.Set<NonScheduledMedication>()
                .Include(n => n.Medication)
                .Where(n => n.Status != NonScheduledMedicationStatus.Deleted)
                .ToListAsync();

            return View(nonScheduledMeds);
        }

        // GET: MedicationAdministration/CreateNonScheduled
        public IActionResult CreateNonScheduled()
        {
            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
            return View();
        }

        // POST: MedicationAdministration/CreateNonScheduled
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNonScheduled(NonScheduledMedication model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Non-scheduled medication added successfully.";
                return RedirectToAction(nameof(NonScheduled));
            }

            ViewData["Medications"] = _context.Set<Medication>().Where(m => m.DeletionStatus == MedicationStatus.Active).ToList();
            return View(model);
        }

        // GET: MedicationAdministration/DeleteNonScheduled/5
        public async Task<IActionResult> DeleteNonScheduled(int? id)
        {
            if (id == null) return NotFound();

            var med = await _context.Set<NonScheduledMedication>()
                .Include(m => m.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (med == null) return NotFound();

            return View(med);
        }

        // POST: MedicationAdministration/DeleteNonScheduled/5
        [HttpPost, ActionName("DeleteNonScheduled")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNonScheduledConfirmed(int id)
        {
            var med = await _context.Set<NonScheduledMedication>().FindAsync(id);
            if (med != null)
            {
                med.Status = NonScheduledMedicationStatus.Deleted;
                _context.Update(med);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Non-scheduled medication deleted successfully.";
            }

            return RedirectToAction(nameof(NonScheduled));
        }

        // ==========================
        // MEDICATION CRUD
        // ==========================

        // GET: MedicationAdministration/Medications
        public async Task<IActionResult> Medications()
        {
            var meds = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active)
                .ToListAsync();
            return View(meds);
        }

        // GET: MedicationAdministration/DetailsMedication/5
        public async Task<IActionResult> DetailsMedication(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.MedicationId == id);

            if (medication == null) return NotFound();

            return View(medication);
        }

        // GET: MedicationAdministration/CreateMedication
        public IActionResult CreateMedication()
        {
            PopulateMedicationDropdowns();
            return View();
        }

        // POST: MedicationAdministration/CreateMedication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedication(Medication model)
        {
            if (ModelState.IsValid)
            {
                model.DeletionStatus = MedicationStatus.Active; // ensure new med is active
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medication created successfully.";
                return RedirectToAction(nameof(Medications));
            }

            PopulateMedicationDropdowns(model);
            return View(model);
        }

        // GET: MedicationAdministration/EditMedication/5
        public async Task<IActionResult> EditMedication(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Medications.FindAsync(id);
            if (medication == null) return NotFound();

            PopulateMedicationDropdowns(medication);
            return View(medication);
        }

        // POST: MedicationAdministration/EditMedication/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedication(int id, Medication model)
        {
            if (id != model.MedicationId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Medication updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Medications.Any(m => m.MedicationId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Medications));
            }

            PopulateMedicationDropdowns(model);
            return View(model);
        }

        // GET: MedicationAdministration/DeleteMedication/5
        public async Task<IActionResult> DeleteMedication(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == id);
            if (medication == null) return NotFound();

            return View(medication);
        }

        // POST: MedicationAdministration/DeleteMedication/5
        [HttpPost, ActionName("DeleteMedication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicationConfirmed(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication != null)
            {
                medication.DeletionStatus = MedicationStatus.Deleted; // soft delete
                _context.Update(medication);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medication deleted successfully.";
            }
            return RedirectToAction(nameof(Medications));
        }

        // ==========================
        // HELPER METHODS
        // ==========================
        private void PopulateMedicationDropdowns(Medication? medication = null)
        {
            ViewBag.TypeList = Enum.GetValues(typeof(MedicationType))
                .Cast<MedicationType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.ToString(),
                    Selected = medication != null && medication.Type == t
                })
                .ToList();

            ViewBag.ScheduleList = Enum.GetValues(typeof(MedicationSchedule))
                .Cast<MedicationSchedule>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = medication != null && medication.Schedule == s
                })
                .ToList();

            ViewBag.StatusList = Enum.GetValues(typeof(MedicationStatus))
                .Cast<MedicationStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = medication != null && medication.DeletionStatus == s
                })
                .ToList();
        }
    

    // ==========================
    // HELPER METHOD
    // ==========================
    private bool ScheduledMedicationExists(int id)
        {
            return _context.Set<ScheduledMedication>().Any(e => e.Id == id);
        }
    }
}

