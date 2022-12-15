using System.ComponentModel.DataAnnotations;

namespace AboutNow.Models
{
    public class Journal
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Continutul Jurnalului  este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
