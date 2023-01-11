using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AboutNow.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele trebuie sa aiba sub 100 de caractere")]
        [MinLength(3, ErrorMessage = "Numele trebuie sa aiba mai mult de 3 caractere")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}