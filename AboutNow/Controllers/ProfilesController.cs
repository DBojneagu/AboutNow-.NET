using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            ViewBag.IdU = _userManager.GetUserId(User);


            var profiles = from profile in db.Profiles
                           orderby profile.LastName
                           select profile;
            ViewBag.Profile = profiles;



            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(string id)
        {
            
            if (db.Profiles.Include("User").Where(prf => prf.UserId == id).FirstOrDefault()!= null)
            {
                Profile profile = db.Profiles.Include("User")
                                       .Where(prf => prf.UserId == id)
                                       .FirstOrDefault();
                return View(profile);
            }
            else
            {
                return RedirectToAction("Index");
            }

         
        }



        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Profile profile = new Profile();
          


            return View(profile);

        }

        //adaugarea jurnalului in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Profile profile)

        {
  
            profile.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                TempData["message"] = "Profilul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                return View(profile);
            }
        }




    }

}
