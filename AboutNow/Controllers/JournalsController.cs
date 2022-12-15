using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Mvc;
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

            ViewBag.Journal = journal;

            ViewBag.Category = journal.Category;

            return View();
        }

        // Se afiseaza formularul in care se vor completa datele unui jurnalul
        // impreuna cu selectarea categoriei din care face parte jurnalul
        public IActionResult New()
        {
            var categories = from categ in db.Categories
                             select categ;

            ViewBag.Categories = categories;

            return View();
        }

        //adaugarea jurnalului in baza de date
        [HttpPost]
        public IActionResult New(Journal journal)

        {
            try
            {
                db.Journals.Add(journal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            catch (Exception)
            {
                return RedirectToAction("New");
            }
        }


        public IActionResult Edit(int id)
        {
            Journal journal = db.Journals.Include("Category")
                                            .Where(jrn => jrn.Id == id)
                                            .First();

            ViewBag.Journal = journal;
            ViewBag.Category = journal.Category;

            var categories = from categ in db.Categories
                             select categ;

            ViewBag.Categories = categories;

            return View();

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
            return RedirectToAction("Index");
        }



    }


}
