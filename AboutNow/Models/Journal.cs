using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AboutNow.Models
{
    public class Journal
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul trebuie sub 100 de caractere")]
        [MinLength(3, ErrorMessage = "Titlul trebuie sa aiba mai mult de 3 caractere")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Continutul Jurnalului  este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }

        public string? UserId { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }


    }
}
