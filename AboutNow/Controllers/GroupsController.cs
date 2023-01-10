using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AboutNow.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public GroupsController(
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
            ViewBag.Groups = db.Groups.ToList();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [NonAction]
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();
            var group = db.Groups.Find(id);
            ViewBag.User = db.Users.Find(group.UserId);
            return View(group);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Group group = new Group();
            return View(group);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Group group)
        {
            group.CreatedDate = DateTime.Now;
            group.UserId = _userManager.GetUserId(User);
            group.User = db.Users.Find(group.UserId);
            
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost adaugat";
                return RedirectToAction("Index");
            }
            return View(group);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            var group = db.Groups.Find(id);

            if (group.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(group);
            }
            TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Group requestGroup)
        {
            var group = db.Groups.Find(id);
            if (ModelState.IsValid)
            {
                if (group.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    group.Name = requestGroup.Name;
                    group.Description = requestGroup.Description;
                    db.SaveChanges();
                    TempData["message"] = "Grup Modificat cu Succes";
                    return RedirectToAction("Index");
                }
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine";
                return RedirectToAction("Index");
            }
            return View(requestGroup);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            var group = db.Groups.Find(id);
            
            if (group.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost sters cu succes";
                return RedirectToAction("Index");
            }
            TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine";
            return RedirectToAction("Index");
        }
    }
}