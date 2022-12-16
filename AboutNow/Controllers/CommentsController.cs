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

            return View(comm);
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            if (ModelState.IsValid)
            {

                comm.Content = requestComment.Content;

                db.SaveChanges();

                return Redirect("/Journals/Show/" + comm.JournalId);
            }
            else
            {
                return View(requestComment);
            }

        }
    }
}
