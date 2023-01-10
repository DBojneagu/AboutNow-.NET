using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace AboutNow.Controllers
{
    public class GroupMessagesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public GroupMessagesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            var group = db.Groups.Find(id);
            var messages = db.GroupMessages.Where(m => m.GroupId == id).ToList();

            ViewBag.Group = group;
            ViewBag.Messages = messages;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult SendMessage(GroupMessage Message)
        {
            if (ModelState.IsValid) {
                Message.Posted = DateTime.Now;
                Message.UserId = _userManager.GetUserId(User);
                Message.UserName = _userManager.GetUserName(User);
                db.GroupMessages.Add(Message);
                db.SaveChanges();
                return Redirect("/GroupMessages/Show/" + Message.GroupId);
            }
            TempData["message"] = "Nu puteti trimite un mesaj gol";
            return Redirect("/GroupMessages/Show/" + Message.GroupId);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            var message = db.GroupMessages.Find(id);

            if (message.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(message);
            }
            TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui mesaj care nu va apartine";
            return Redirect("/GroupMessages/Show/" + message.GroupId);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, GroupMessage requestMessage)
        {
            var message = db.GroupMessages.Find(id);
            if (ModelState.IsValid)
            {
                if (message.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    message.Content = requestMessage.Content;
                    db.SaveChanges();
                    TempData["message"] = "Mesaj modificat cu Succes";
                    return Redirect("/GroupMessages/Show/" + message.GroupId);
                }
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui mesaj care nu va apartine";
                return Redirect("/GroupMessages/Show/" + message.GroupId);
            }
            return View(message);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            var message = db.GroupMessages.Find(id);

            if (message.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.GroupMessages.Remove(message);
                db.SaveChanges();
                TempData["message"] = "Mesajul a fost sters cu succes";
                return Redirect("/GroupMessages/Show/" + message.GroupId);
            }
            TempData["message"] = "Nu aveti dreptul sa stergeti un mesaj care nu va apartine";
            return Redirect("/GroupMessages/Show/" + message.GroupId);
        }
    }
}