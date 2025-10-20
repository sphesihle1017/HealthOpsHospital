using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HealthOps_Project.Controllers
{

    [Authorize(Roles = "Admin,Doctor")]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PatientsController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var list = await _db.Patients.Where(n => n.IsActive == true).ToListAsync();
            return View(list);
        }

        public IActionResult Create() => View();


        // GET: Patients
        // Index action showing only active patients with search functionality
        public async Task<IActionResult> PatientIndex(string query)
        {
            // Start with only active patients
            IQueryable<Patient> patientsQuery = _db.Patients.Where(p => p.IsActive == true);

            if (!string.IsNullOrEmpty(query))
            {
                // Filter by Patient ID, First Name, or Last Name
                patientsQuery = patientsQuery.Where(p =>
                    p.PatientId.ToString().Contains(query) ||
                    p.FirstName.Contains(query) ||
                    p.LastName.Contains(query)
                );
            }

            var patients = await patientsQuery.ToListAsync();
            return View(patients);
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                var patients = await _db.Patients.ToListAsync();
                return View("PatientIndex", patients);
            }

            var filteredPatients = await _db.Patients
                                  .Where(p => p.PatientId.ToString().Contains(query))
                                  .ToListAsync();

            return View("PatientIndex", filteredPatients);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _db.Patients
                .Include(p => p.Bed)
                .FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }


        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            patient.AdmissionDate = DateTime.Now;
            patient.IsActive = true;

            if (!ModelState.IsValid)
            {
                ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (await _db.Patients.AnyAsync(p => p.PatientId == patient.PatientId))
            {
                ModelState.AddModelError("IdentityDocument", "This  ID already exists.");
                ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (patient.BedId.HasValue && !await _db.Beds.AnyAsync(b => b.BedId == patient.BedId.Value))
            {
                ModelState.AddModelError("BedId", "Selected bed does not exist.");
                ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            // Clear if-else redirect
            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("PatientIndex");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _db.Patients
                .Include(p => p.Bed)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null) return NotFound();

            ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber", patient.BedId);
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.PatientId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (await _db.Patients.AnyAsync(p => p.PatientId == patient.PatientId && p.PatientId != patient.PatientId))
            {
                ModelState.AddModelError("SouthAfricanID", "This South African ID already exists.");
                ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (patient.BedId.HasValue && !await _db.Beds.AnyAsync(b => b.BedId == patient.BedId.Value))
            {
                ModelState.AddModelError("BedId", "Selected bed does not exist.");
                ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            try
            {
                patient.IsActive = true;
                _db.Update(patient);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Patients.AnyAsync(e => e.PatientId == patient.PatientId))
                    return NotFound();
                else throw;
            }

            // Clear if-else redirect
            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("PatientIndex");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Patients/Discharge/5
        public async Task<IActionResult> Discharge(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // POST: Patients/Discharge/5
        [HttpPost, ActionName("Discharge")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DischargeConfirmed(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsActive = false; // soft delete
                await _db.SaveChangesAsync();
            }

            // Clear if-else redirect
            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("PatientIndex");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


    }
}
