using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace AboutNow.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public FriendsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public ActionResult Index()
        {

            var all = db.Profiles.Include("User").ToList();
            List<int> profiles = new List<int>();
            foreach ( var a in all )
            {
                profiles.Add( a.Id );
            }

            List<ApplicationUser> users = new List<ApplicationUser>();
            foreach (var a in all)
            {
                users.Add(a.User);
            }


            var currentUser = db.Users.Find(_userManager.GetUserId(User));

            var UserFriendships = db.Friends.Where(f => f.User1_Id == currentUser.Id || f.User2_Id == currentUser.Id).ToList();
            var friends = new List<string>();
            var accepted = new List<bool>();
            foreach (var friendship in UserFriendships)
            {
                if (friendship.User1_Id == currentUser.Id)
                {
                    friends.Add(friendship.User2_Id);
                    accepted.Add((bool)friendship.Accepted);

                }
                else

                {
                    friends.Add(friendship.User1_Id);
                    accepted.Add((bool)friendship.Accepted);

                }

            }

            ViewBag.Accepted = accepted;
            

            ViewBag.Profiles = profiles;
            ViewBag.Users = users;
            ViewBag.UserLength = users.Count;
            ViewBag.CurrentUser = currentUser;
            ViewBag.UserFriends = friends;

            return View();
        }

        [HttpPost]
        public ActionResult AddFriend(IFormCollection formData)
        {
            string currentUser = _userManager.GetUserId(User);
            string friendToAdd = formData["UserId"]; // TODO: trebuie validare (verificare daca userul exista)

            Friend friendship = new Friend();
            friendship.User1_Id = currentUser;
            friendship.User2_Id = friendToAdd;
            friendship.Accepted = false; // Accepted = false, iar in lista de cereri -> accept
            friendship.RequestTime = DateTime.Now;

            // TODO: sa existe try si catch astfel incat sa nu se trimita o cerere de doua ori
            // verificare daca userul a primit deja cerere de la userul caruia doreste sa ii trimita
            // de verificat ca user1 sa nu fie deja prieten cu user2

            db.Friends.Add(friendship);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        
        public IActionResult AcceptFriend(int id)
        {

            var request = db.Friends.Find(id);
            request.Accepted = true;
            db.SaveChanges();

            var profileId = db.Profiles.Where(p => p.UserId == request.User2_Id).First().Id;
            return Redirect("/Profiles/Show/" + profileId);
        }
    }
}