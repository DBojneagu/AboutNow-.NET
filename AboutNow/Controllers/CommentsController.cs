using AboutNow.Data;
using AboutNow.Models;
using Microsoft.AspNetCore.Mvc;

namespace AboutNow.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        public CommentsController(ApplicationDbContext context)
        {
            db = context;
        }


        // Adaugarea unui comentariu asociat unui articol in baza de date
        [HttpPost]
        public IActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;

            try
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Journals/Show/" + comm.JournalId);
            }

            catch (Exception)
            {
                return Redirect("/Journals/Show/" + comm.JournalId);
            }

        }

        // Stergerea unui comentariu asociat unui articol din baza de date
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Journals/Show/" + comm.JournalId);
        }

        // In acest moment vom implementa editarea intr-o pagina View separata
        // Se editeaza un comentariu existent

        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            ViewBag.Comment = comm;
            return View();
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            try
            {

                comm.Content = requestComment.Content;

                db.SaveChanges();

                return Redirect("/Journals/Show/" + comm.JournalId);
            }
            catch (Exception e)
            {
                return Redirect("/Journals/Show/" + comm.JournalId);
            }

        }
    }
}
