using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AboutNow.Controllers
{
    public class JournalsController : Controller
    {
        private readonly ApplicationDbContext db;
        public JournalsController(ApplicationDbContext context)
        {
            db = context;
        }

        //Se afiseaza lista tuturor journalelor
        //din baza de date impreuna cu categoria din care fac parte
        // HttpGet implicit
        public IActionResult Index()
        {
            var journals = db.Journals.Include("Category");

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
        public IActionResult Show(int id)
        {
            Journal journal = db.Journals.Include("Category").Include("Comments")
                                                  .Where(jrn => jrn.Id == id)
                                                  .First();



            return View(journal);
        }

        // Se afiseaza formularul in care se vor completa datele unui jurnalul
        // impreuna cu selectarea categoriei din care face parte jurnalul
        public IActionResult New()
        {
            Journal journal = new Journal();
            journal.Categ = GetAllCategories();


            return View(journal);

        }

        //adaugarea jurnalului in baza de date
        [HttpPost]
        public IActionResult New(Journal journal)

        {
            journal.Date = DateTime.Now;
            journal.Categ = GetAllCategories();
            try
            {
                db.Journals.Add(journal);
                db.SaveChanges();
                TempData["message"] = "Jurnalul a fost adaugat";
                return RedirectToAction("Index");
            }

            catch (Exception)
            {
                return View(journal);
            }
        }


        public IActionResult Edit(int id)
        {
            Journal journal = db.Journals.Include("Category")
                                            .Where(jrn => jrn.Id == id)
                                            .First();

            journal.Categ = GetAllCategories();




            return View(journal);

        }

        [HttpPost]
        public IActionResult Edit(int id, Journal requestJournal)
        {   // in requestJournal am valorile modificate, care se salveaza
            // in variabila asta pt ca le am in edit si de acolo le modific.

            Journal journal = db.Journals.Find(id);
            try
            {
                {
                    journal.Title = requestJournal.Title;
                    journal.Content = requestJournal.Content;
                    journal.Date = requestJournal.Date;
                    journal.CategoryId = requestJournal.CategoryId;
                    db.SaveChanges();
                }
                TempData["message"] = "Jurnal Editat cu Succes";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", id);
            }
        }


        [HttpPost]

        public IActionResult Delete(int id)
        {
            Journal journal = db.Journals.Find(id);
            db.Journals.Remove(journal);
            db.SaveChanges();
            TempData["message"] = "Jurnalul a fost sters cu succes";
            return RedirectToAction("Index");
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
