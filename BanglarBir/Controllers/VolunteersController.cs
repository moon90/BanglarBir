using BanglarBir.Data;
using BanglarBir.Models;
using BanglarBir.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BanglarBir.Controllers
{
    [Route("volunteers")]
    public class VolunteersController : Controller
    {
        private readonly ILogger<VolunteersController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public VolunteersController(ILogger<VolunteersController> logger, ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var volunteers = await _context.Volunteers
                .Where(v => v.Id.ToString() == userId) // Filter based on the logged-in user's ID
                .FirstOrDefaultAsync();

            return View(volunteers);
        }

        [HttpGet("volunteerlist")]
        public async Task<IActionResult> VolunteerList()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var volunteers = await _context.Volunteers.ToListAsync();

            return View(volunteers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Volunteer", Text = "Volunteer" },
                new SelectListItem { Value = "Admin", Text = "Admin" }
            };

            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Volunteer volunteer, IFormFile StudentIdOrNidPhoto)
        {
            ModelState.Remove("StudentIdOrNidPhoto");

            if (ModelState.IsValid)
            {

                // Check if EmailOrPhone already exists
                bool isExistingUser = await IsEmailOrPhoneInUse(volunteer.EmailOrPhone);

                if (isExistingUser)
                {
                    ModelState.AddModelError("EmailOrPhone", "This email or phone number is already in use.");
                    return View(volunteer);
                }

                ViewBag.Roles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Volunteer", Text = "Volunteer", Selected = volunteer.Role == "Volunteer" },
                    new SelectListItem { Value = "Admin", Text = "Admin", Selected = volunteer.Role == "Admin" }
                };

                if (StudentIdOrNidPhoto != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + StudentIdOrNidPhoto.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await StudentIdOrNidPhoto.CopyToAsync(fileStream);
                        }

                        volunteer.StudentIdOrNidPhoto = "/uploads/" + uniqueFileName;
                    }

                volunteer.Role = "Volunteer";

                _context.Volunteers.Add(volunteer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Volunteer", Text = "Volunteer", Selected = volunteer.Role == "Volunteer" },
                new SelectListItem { Value = "Admin", Text = "Admin", Selected = volunteer.Role == "Admin" }
            };

            return View(volunteer);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Volunteer volunteer, IFormFile StudentIdOrNidPhoto)
        {
            if (id != volunteer.Id)
            {
                return BadRequest();
            }

            ModelState.Remove("StudentIdOrNidPhoto");
            ModelState.Remove("EmailOrPhone");

            if (ModelState.IsValid)
            {
                // Check if EmailOrPhone already exists
                //bool isExistingUser = await IsEmailOrPhoneInUse(volunteer.EmailOrPhone);

                //if (isExistingUser)
                //{
                //    ModelState.AddModelError("EmailOrPhone", "This email or phone number is already in use.");
                //    return View(volunteer);
                //}

                ViewBag.Roles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Volunteer", Text = "Volunteer", Selected = volunteer.Role == "Volunteer" },
                    new SelectListItem { Value = "Admin", Text = "Admin", Selected = volunteer.Role == "Admin" }
                };


                // Preserve the existing photo if no new file is uploaded
                if (StudentIdOrNidPhoto != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + StudentIdOrNidPhoto.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await StudentIdOrNidPhoto.CopyToAsync(fileStream);
                    }

                    // Update the volunteer's photo with the new file path
                    volunteer.StudentIdOrNidPhoto = "/uploads/" + uniqueFileName;
                }
                else
                {
                    // Retain the existing photo by getting the current value from the database
                    var existingVolunteer = await _context.Volunteers.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
                    if (existingVolunteer != null)
                    {
                        volunteer.StudentIdOrNidPhoto = existingVolunteer.StudentIdOrNidPhoto;
                    }
                }
                //var updateVolunteer = new Volunteer
                //{
                //    Name = volunteer.Name,
                //    Location = volunteer.Location,
                //    FbProfileUrl = volunteer.FbProfileUrl,
                //    Password = volunteer.Password,
                //    StudentIdOrNidPhoto = volunteer.StudentIdOrNidPhoto
                //};

                _context.Entry(volunteer).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer != null)
            {
                _context.Volunteers.Remove(volunteer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.Id == id);
        }

        public async Task<bool> IsEmailOrPhoneInUse(string emailOrPhone)
        {
            return await _context.Volunteers.AnyAsync(u => u.EmailOrPhone == emailOrPhone);
        }
    }
}
