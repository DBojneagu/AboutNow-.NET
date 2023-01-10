using System.ComponentModel.DataAnnotations;

namespace AboutNow.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Prenumele este obligatoriu")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public string? Privacy { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}