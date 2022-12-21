using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AboutNow.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProfilesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var profiles = db.Profiles.Include("User");

            ViewBag.Nume = User.Identity?.Name;
            //ViewBag.OriceDenumireSugestiv

            ViewBag.profile = profiles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            Profile profile = db.Profiles.Include("User")
                                        .Where(prf => prf.Id == id)
                                        .First();





            return View(profile);
        }





    }

}
