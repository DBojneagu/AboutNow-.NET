using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AboutNow.Controllers
{
    [Authorize]
    public class JournalsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public JournalsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //Se afiseaza lista tuturor journalelor
        //din baza de date impreuna cu categoria din care fac parte
        // HttpGet implicit
        //Pentru fiecare jurnal se afiseaza si utilizatorul care a postat jurnalul

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var journals = db.Journals.Include("Category").Include("User");

            //ViewBag.OriceDenumireSugestiva
            ViewBag.Journals = journals;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // Se afiseaza un singur articol in functie de id-ul sau impreuna cu categoria din
        // care face parte 
        // si o sa aiba HTTPGet implicit
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            Journal journal = db.Journals.Include("Category").Include("Comments").Include("User")
                                                  .Where(jrn => jrn.Id == id)
                                                  .First();

            SetAccessRights();

            return View(journal);
        }

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

        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Journals/Show/" + comment.JournalId);
            }

            else
            {
                Journal journal = db.Journals.Include("Category").Include("Comments")
                               .Where(journal => journal.Id == comment.JournalId)
                               .First();

                //return Redirect("/Articles/Show/" + comm.ArticleId);

                return View(journal);
            }
        }



        // Se afiseaza formularul in care se vor completa datele unui jurnalul
        // impreuna cu selectarea categoriei din care face parte jurnalul
        // Doar utilizatorii cu rolul editor sau admin pot adauga jurnale in platforma

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Journal journal = new Journal();
            journal.Categ = GetAllCategories();


            return View(journal);

        }

        //adaugarea jurnalului in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Journal journal)

        {
            journal.Date = DateTime.Now;
            journal.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Journals.Add(journal);
                db.SaveChanges();
                TempData["message"] = "Jurnalul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                journal.Categ = GetAllCategories();
                return View(journal);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Journal journal = db.Journals.Include("Category")
                                            .Where(jrn => jrn.Id == id)
                                            .First();

            journal.Categ = GetAllCategories();


            if (journal.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(journal);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index"); 
            }

            

        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Journal requestJournal)
        {   // in requestJournal am valorile modificate, care se salveaza
            // in variabila asta pt ca le am in edit si de acolo le modific.

            Journal journal = db.Journals.Find(id);
            

            if (ModelState.IsValid)
            {
                if (journal.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    journal.Title = requestJournal.Title;
                    journal.Content = requestJournal.Content;
                    journal.Date = requestJournal.Date;
                    journal.CategoryId = requestJournal.CategoryId;
                    db.SaveChanges();
                    TempData["message"] = "Jurnal Editat cu Succes";
                    return RedirectToAction("Index");
                }


                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    return RedirectToAction("Index");
                }

            }
            else
            {
                requestJournal.Categ = GetAllCategories();
                return View(requestJournal);
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Admin")]

        public IActionResult Delete(int id)
        {
            Journal journal = db.Journals.Include("Comments")
                                        .Where(jrn => jrn.Id == id)
                                        .First();
            if (journal.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Journals.Remove(journal);
                db.SaveChanges();
                TempData["message"] = "Jurnalul a fost sters cu succes";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui jurnal care nu va apartine";
                return RedirectToAction("Index");
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }



    }


}
