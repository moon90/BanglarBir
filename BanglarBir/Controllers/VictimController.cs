using BanglarBir.Data;
using BanglarBir.Models;
using BanglarBir.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BanglarBir.Controllers
{
    [Route("victims")]
    public class VictimController : Controller
    {
        private readonly ILogger<VictimController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public VictimController(ILogger<VictimController> logger, ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            //var victims = await (from v in _context.Victims join
            //               vo in _context.Volunteers on v.VolunteerId equals vo.Id
            //               select new VictimVM 
            //               {
            //                   Id = v.Id,
            //                   Name = v.Name,
            //                   Location = v.Location,
            //                   Phone = v.Phone,
            //                   IsStudent = v.IsStudent == true ? "Yes" : "No",
            //                   bKashNumber = v.bKashNumber,
            //                   Status = v.Status,
            //                   DonationNeeded = v.DonationNeeded == true ? "Yes" : "No",
            //                   DonationConfirmed = v.DonationConfirmed,
            //                   Picture = v.Picture,
            //                   VolunteerId = v.VolunteerId,
            //                   VolunteerName = vo.Name
            //               }).ToListAsync();

            //await _context.Victims.ToListAsync();
            return View();
        }

        [HttpGet("LoadVictims")]
        public async Task<ActionResult> LoadVictims(int page = 1, int pageSize = 10, string keyword = "", string donationNeeded = "", string status = "")
        {
            
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var query = from v in _context.Victims
                            join vo in _context.Volunteers on v.VolunteerId equals vo.Id
                            select new VictimVM
                            {
                                Id = v.Id,
                                Name = v.Name,
                                Location = v.Location,
                                Phone = v.Phone,
                                IsStudent = v.IsStudent ? "Yes" : "No",
                                bKashNumber = v.bKashNumber,
                                Status = v.Status,
                                DonationNeeded = v.DonationNeeded ? "Yes" : "No",
                                DonationConfirmed = v.DonationConfirmed,
                                Picture = v.Picture,
                                VolunteerId = v.VolunteerId,
                                VolunteerName = vo.Name
                            };

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(v => v.Name.ToLower().Contains(keyword) || v.Location.ToLower().Contains(keyword));
                }

                if (!string.IsNullOrEmpty(donationNeeded))
                {
                    string donationNeededBool = donationNeeded;
                    query = query.Where(v => v.DonationNeeded.ToLower() == donationNeededBool);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(v => v.Status.ToLower().Equals(status.ToLower()));
                }

                // Get the total count of records
                var totalRecords = await query.CountAsync();

                // If there are no records, return an empty result
                if (totalRecords == 0)
                {
                    return Json(new { success = true, data = new List<VictimVM>(), totalRecords = 0 });
                }

                // Apply pagination
                var victims = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = victims,
                    totalRecords,
                    pageSize,
                    currentPage = page
                });
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("victimsbyuser")]
        public IActionResult VictimsByUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        //[HttpGet("LoadVictimsByUser")]
        //public async Task<ActionResult> LoadVictimsByUser(int page = 1, int pageSize = 10, string keyword = "", string donationNeeded = "", string status = "")
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (userId == null)
        //    {
        //        return Unauthorized();
        //    }
        //    try
        //    {
        //        var query = from v in _context.Victims
        //                    join vo in _context.Volunteers on v.VolunteerId equals vo.Id
        //                    where v.VolunteerId == Convert.ToInt32(userId)
        //                    select new VictimVM
        //                    {
        //                        Id = v.Id,
        //                        Name = v.Name,
        //                        Location = v.Location,
        //                        Phone = v.Phone,
        //                        IsStudent = v.IsStudent ? "Yes" : "No",
        //                        bKashNumber = v.bKashNumber,
        //                        Status = v.Status,
        //                        DonationNeeded = v.DonationNeeded ? "Yes" : "No",
        //                        DonationConfirmed = v.DonationConfirmed,
        //                        Picture = v.Picture,
        //                        VolunteerId = v.VolunteerId,
        //                        VolunteerName = vo.Name
        //                    };

        //        if (!string.IsNullOrEmpty(keyword))
        //        {
        //            query = query.Where(v => v.Name.ToLower().Contains(keyword) || v.Location.ToLower().Contains(keyword));
        //        }

        //        if (!string.IsNullOrEmpty(donationNeeded))
        //        {
        //            string donationNeededBool = donationNeeded;
        //            query = query.Where(v => v.DonationNeeded.ToLower() == donationNeededBool);
        //        }

        //        if (!string.IsNullOrEmpty(status))
        //        {
        //            query = query.Where(v => v.Status.ToLower().Equals(status.ToLower()));
        //        }

        //        // Get the total count of records
        //        var totalRecords = await query.CountAsync();

        //        // If there are no records, return an empty result
        //        if (totalRecords == 0)
        //        {
        //            return Json(new { success = true, data = new List<VictimVM>() });
        //        }


        //        // Apply pagination
        //        var victims = await query
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync();

        //        // Check if the next page has records
        //        bool hasMoreRecords = (page * pageSize) < totalRecords;

        //        return Json(new
        //        {
        //            success = true,
        //            data = victims,
        //            hasMoreRecords
        //        });
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        [HttpGet("LoadVictimsByUser")]
        public async Task<ActionResult> LoadVictimsByUser(int page = 1, int pageSize = 12, string keyword = "", string donationNeeded = "", string status = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized();
                }

                var query = from v in _context.Victims
                            join vo in _context.Volunteers on v.VolunteerId equals vo.Id
                            where v.VolunteerId == Convert.ToInt32(userId)
                            select new VictimVM
                            {
                                Id = v.Id,
                                Name = v.Name,
                                Location = v.Location,
                                Phone = v.Phone,
                                IsStudent = v.IsStudent ? "Yes" : "No",
                                bKashNumber = v.bKashNumber,
                                Status = v.Status,
                                DonationNeeded = v.DonationNeeded ? "Yes" : "No",
                                DonationConfirmed = v.DonationConfirmed,
                                Picture = v.Picture,
                                VolunteerId = v.VolunteerId,
                                VolunteerName = vo.Name
                            };

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(v => v.Name.ToLower().Contains(keyword) || v.Location.ToLower().Contains(keyword));
                }

                if (!string.IsNullOrEmpty(donationNeeded))
                {
                    string donationNeededBool = donationNeeded;
                    query = query.Where(v => v.DonationNeeded.ToLower() == donationNeededBool);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(v => v.Status.ToLower().Equals(status.ToLower()));
                }

                // Get the total count of records
                var totalRecords = await query.CountAsync();

                // If there are no records, return an empty result
                if (totalRecords == 0)
                {
                    return Json(new { success = true, data = new List<VictimVM>(), totalRecords = 0 });
                }

                // Apply pagination
                var victims = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = victims,
                    totalRecords,
                    pageSize,
                    currentPage = page
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Dead", Text = "Dead" },
                new SelectListItem { Value = "Injured", Text = "Injured" }
            };

            ViewBag.DonationNeededOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };

            ViewBag.IsStudentOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };

            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Victim victim, IFormFile picture)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Dead", Text = "Dead", Selected = victim.Status == "Dead" },
                new SelectListItem { Value = "Injured", Text = "Injured", Selected = victim.Status == "Injured" }
            };

            ViewBag.DonationNeededOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes", Selected = victim.DonationNeeded },
                new SelectListItem { Value = "false", Text = "No", Selected = !victim.DonationNeeded }
            };

            ViewBag.IsStudentOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes", Selected = victim.IsStudent },
                new SelectListItem { Value = "false", Text = "No", Selected = !victim.IsStudent }
            };

            // Remove Picture property from ModelState validation
            ModelState.Remove("Picture");

            if (ModelState.IsValid)
            {
                // Check if EmailOrPhone already exists
                bool isExistingUser = await IsPhoneInUse(victim.Phone);

                if (isExistingUser)
                {
                    ModelState.AddModelError(string.Empty, "This phone number is already in use.");
                    return View(victim);
                }

                if (picture != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + picture.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await picture.CopyToAsync(fileStream);
                        }

                        victim.Picture = "/uploads/" + uniqueFileName;
                    }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized();
                }

                victim.VolunteerId = Convert.ToInt32(userId);

                _context.Victims.Add(victim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           }

            return View(victim);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var victim = await _context.Victims.FindAsync(id);
            if (victim == null)
            {
                return NotFound();
            }

            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Dead", Text = "Dead", Selected = victim.Status == "Dead" },
                new SelectListItem { Value = "Injured", Text = "Injured", Selected = victim.Status == "Injured" }
            };

            ViewBag.DonationNeededOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Need", Selected = victim.DonationNeeded },
                new SelectListItem { Value = "false", Text = "No Need", Selected = !victim.DonationNeeded }
            };

            ViewBag.IsStudentOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes", Selected = victim.IsStudent },
                new SelectListItem { Value = "false", Text = "No", Selected = !victim.IsStudent }
            };

            return View(victim);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Victim victim, IFormFile picture)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            if (id != victim.Id)
            {
                return BadRequest();
            }

            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Dead", Text = "Dead", Selected = victim.Status == "Dead" },
                new SelectListItem { Value = "Injured", Text = "Injured", Selected = victim.Status == "Injured" }
            };

            ViewBag.DonationNeededOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes", Selected = victim.DonationNeeded },
                new SelectListItem { Value = "false", Text = "No", Selected = !victim.DonationNeeded }
            };

            ViewBag.IsStudentOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Yes", Selected = victim.IsStudent },
                new SelectListItem { Value = "false", Text = "No", Selected = !victim.IsStudent }
            };

            // Remove Picture property from ModelState validation
            ModelState.Remove("Picture");

            if (ModelState.IsValid)
            {
                if (picture != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + picture.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await picture.CopyToAsync(fileStream);
                    }

                    victim.Picture = "/uploads/" + uniqueFileName;
                }
                else
                {
                    // Retain the existing photo by getting the current value from the database
                    var existingVictim = await _context.Victims.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
                    if (existingVictim != null)
                    {
                        victim.Picture = existingVictim.Picture;
                    }
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            victim.VolunteerId = Convert.ToInt32(userId);

            _context.Entry(victim).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           }

            return View(victim);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var victim = await _context.Victims.FindAsync(id);
            if (victim == null)
            {
                return NotFound();
            }
            return View(victim);
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var victim = await _context.Victims.FindAsync(id);
            if (victim == null)
            {
                return NotFound();
            }

            _context.Victims.Remove(victim);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<bool> IsPhoneInUse(string Phone)
        {
            return await _context.Victims.AnyAsync(u => u.Phone == Phone);
        }
    }
}
