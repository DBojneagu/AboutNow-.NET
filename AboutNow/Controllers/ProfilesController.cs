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
            var userId = _userManager.GetUserId(User);
            var profile = db.Profiles.Where(p => p.UserId == userId).FirstOrDefault();

            if (profile == null)
                ViewBag.Created = false;
            else {
                ViewBag.Created = true;
                ViewBag.Profile = profile;
            }

            if (TempData.ContainsKey("message")) {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            var profile = db.Profiles.Where(p => p.Id == id).FirstOrDefault();
            if (profile == null)
            {
                TempData["message"] = " Profilul nu exista";
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Admin") || profile.Privacy == "public" || _userManager.GetUserId(User) == profile.UserId) // || suntem prieteni
            {
                return View(profile);
            }
            //      var prieten = db.Friends
            //Where(fr => fr.User1_Id == id1 && fr.User2_Id == id2 || fr.User1_id=id2 && fr.User2_id =id1) daca e prieten sau nu cu celealt user
            TempData["message"] = " Profilul este privat ";
            return RedirectToAction("Index");
        }
        
        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            var test = db.Profiles.Where(p => p.UserId == _userManager.GetUserId(User)).FirstOrDefault();
            if (test != null)
                return RedirectToAction("Index");

            Profile profile = new Profile();
            return View(profile);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Profile profile)
        {
            profile.UserId = _userManager.GetUserId(User);

            if ((ModelState.IsValid) && (profile.Privacy == "public" || profile.Privacy == "private"))
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                TempData["message"] = "Profilul a fost creat";
                return RedirectToAction("Index");
            }
            return View(profile);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            var profile = db.Profiles.Find(id);

            if (profile.UserId == _userManager.GetUserId(User))
            {
                return View(profile);
            }
            TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra altui profil";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Profile requestProfile)
        {
            var profile = db.Profiles.Find(id);
            if (ModelState.IsValid)
            {
                if (profile.UserId == _userManager.GetUserId(User))
                {
                    profile.FirstName = requestProfile.FirstName;
                    profile.LastName = requestProfile.LastName;
                    profile.Description = requestProfile.Description;
                    if (requestProfile.Privacy == "public" || requestProfile.Privacy == "private")
                        profile.Privacy = requestProfile.Privacy;

                    db.SaveChanges();
                    TempData["message"] = "Profil modificat cu Succes";
                    return RedirectToAction("Index");
                }
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra altui profil";
                return RedirectToAction("Index");
            }
            return View(requestProfile);
        }
    }
}