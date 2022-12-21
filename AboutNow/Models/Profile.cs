using System.ComponentModel.DataAnnotations;

namespace AboutNow.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Description { get; set; }

        public int? Status { get; set; }

        public string? UserId { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
