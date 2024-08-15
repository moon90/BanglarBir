using BanglarBir.Data;
using BanglarBir.Models;
using BanglarBir.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Security.Claims;

namespace BanglarBir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                VictimsNeedDonation = await _context.Victims.CountAsync(v => v.DonationNeeded),
                EstimatedDeath = await _context.Victims.CountAsync(v => v.Status == "Dead"),
                EstimatedInjuries = await _context.Victims.CountAsync(v => v.Status == "Injured"),
                TotalVictims = await _context.Victims.CountAsync(),
                Volunteers = await _context.Volunteers.CountAsync(),
                //BdtDonationConfirmed = await _context.Victims.CountAsync(v => v.DonationConfirmed), // Default to 0 if null
                StudentVictims = await _context.Victims.CountAsync(v => v.IsStudent),
                NonStudentVictims = await _context.Victims.CountAsync(v => !v.IsStudent)
            };

            return View(viewModel);
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpVM model, IFormFile StudentIdOrNidPhoto)
        {
            // Remove Picture property from ModelState validation
            ModelState.Remove("StudentIdOrNidPhoto");
            if (ModelState.IsValid)
            {
                // Check if EmailOrPhone already exists
                bool isExistingUser = await IsEmailOrPhoneInUse(model.EmailOrPhone);

                if (isExistingUser)
                {
                    ModelState.AddModelError("EmailOrPhone", "This email or phone number is already in use.");
                    return View(model);
                }

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

                    model.StudentIdOrNidPhoto = "/uploads/" + uniqueFileName;
                }

            var user = new Volunteer
                {
                    Name = model.Name,
                    EmailOrPhone = model.EmailOrPhone,
                    Password = model.Password,
                    Location = model.Location,
                    FbProfileUrl = model.FbProfileUrl,
                    NIdOrStudentId = model.NIdOrStudentId,
                    StudentIdOrNidPhoto = model.StudentIdOrNidPhoto,
                    Role = "Volunteer" // Default role
                };
                _context.Volunteers.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Home");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Volunteers.SingleOrDefault(u => u.EmailOrPhone == model.EmailOrPhone && u.Password == model.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Ensure ID is included
                        new Claim(ClaimTypes.Role, user.Role)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, "UserLogin");
                    await HttpContext.SignInAsync("CookieAuthentication", new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Volunteers", user.Id);
                }
                ModelState.AddModelError("", "Invalid email or phone number or password");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<bool> IsEmailOrPhoneInUse(string emailOrPhone)
        {
            return await _context.Volunteers.AnyAsync(u => u.EmailOrPhone == emailOrPhone);
        }

        //[HttpGet]
        //public async Task<JsonResult> LoadVictims(int page = 1, int pageSize = 10, string keyword = "", string donationNeeded = "", string status = "")
        //{
        //    try
        //    {
        //        var query = from v in _context.Victims
        //                    join vo in _context.Volunteers on v.VolunteerId equals vo.Id
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

        [HttpGet]
        public async Task<JsonResult> LoadVictims(int page = 1, int pageSize = 12, string keyword = "", string donationNeeded = "", string status = "")
        {
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


        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Volunteers
                    .FirstOrDefaultAsync(u => u.EmailOrPhone == model.EmailOrPhone);

                if (user != null)
                {
                    // Assuming you want to reset password here. Redirect to reset password page
                    return RedirectToAction("ResetPassword", new { emailOrPhone = model.EmailOrPhone });
                }

                ModelState.AddModelError(string.Empty, "No user found with this email or phone.");
            }

            return View(model);
        }

        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string emailOrPhone)
        {
            return View(new ResetPasswordVM { EmailOrPhone = emailOrPhone });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Volunteers
                    .FirstOrDefaultAsync(u => u.EmailOrPhone == model.EmailOrPhone);

                if (user != null)
                {
                   // user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password); // Assuming you are using BCrypt for hashing
                   user.Password = model.NewPassword; // Assuming you are using BCrypt for hashing
                   
                    _context.Volunteers.Update(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Login", "Home");
                }

                ModelState.AddModelError(string.Empty, "User not found.");
            }

            return View(model);
        }

        public IActionResult Contact()
        {
            return View();
        }

    }
}
