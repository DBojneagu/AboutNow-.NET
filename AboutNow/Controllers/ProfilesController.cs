using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography.Pkcs;

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
        public IActionResult ShowProfiles()
        {
            var profiles = db.Profiles.Include("User");

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                //  profiles = profiles.Contains(search);
                List<int> profileIds = db.Profiles.Where(at => at.FirstName.Contains(search) || at.LastName.Contains(search)).Select(a => a.Id).ToList();

                profiles = db.Profiles.Where(profile => profileIds.Contains(profile.Id)).Include("User");
                
            }
            ViewBag.SearchString = search;
            ViewBag.Profiles = profiles;
            return View();

        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {

            var all = db.Profiles.Include("User").ToList();
            List<int> profiles = new List<int>();
            foreach (var a in all)
            {
                profiles.Add(a.Id);
            }

            List<ApplicationUser> users = new List<ApplicationUser>();
            foreach (var a in all)
            {
                users.Add(a.User);
            }
            var profile = db.Profiles.Where(p => p.Id == id).FirstOrDefault();

            var currentUser = db.Users.Find(_userManager.GetUserId(User));

            var UserFriendships = db.Friends.Where(f => f.User1_Id == currentUser.Id || f.User2_Id == currentUser.Id).ToList();



            var friends = new List<string>();
            var actualfriends = new List<string>();
            foreach (var friendship in UserFriendships)
            {
                if (friendship.Accepted == true)
                {
                    if (friendship.User1_Id == currentUser.Id)
                    {
                        friends.Add(friendship.User2_Id);
                        var name = db.Users.Where(u => u.Id == friendship.User2_Id).First().UserName;
                        actualfriends.Add(name);
                    }
                    else
                    {
                        friends.Add(friendship.User1_Id);
                        var name = db.Users.Where(u => u.Id == friendship.User1_Id).First().UserName;
                        actualfriends.Add(name);
                    }
                }
            }

            var requestFriendsId = new List<int>();
            var requestUserNames = new List<string>();
            foreach (var friendship in UserFriendships)
            {
                if (friendship.Accepted == false)
                {
                    if (friendship.User2_Id == currentUser.Id)
                    {
                        requestFriendsId.Add(friendship.FriendId);
                        var name = db.Users.Where(u => u.Id == friendship.User1_Id).First().UserName;
                        requestUserNames.Add(name);
                    }
                }
            }

            if (profile == null)
            {
                TempData["message"] = " Profilul nu exista";
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Admin") || profile.Privacy == "public" || _userManager.GetUserId(User) == profile.UserId  ||friends.Contains(profile.UserId)) // || suntem prieteni
            {
                ViewBag.RequestFriendsId = requestFriendsId;
                ViewBag.RequestUserNames = requestUserNames;
                ViewBag.FriendsLength = requestFriendsId.Count;
                ViewBag.NamesLength = requestUserNames.Count;

                ViewBag.UserCurent = currentUser;
                ViewBag.Friends = UserFriendships;
                ViewBag.ActualFriends = actualfriends;
                
                return View(profile);
                
            }

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