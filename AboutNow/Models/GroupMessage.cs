using System.ComponentModel.DataAnnotations;
namespace AboutNow.Models
{
    public class GroupMessage
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string? Content { get; set; }
        public DateTime Posted { get; set; }
        public int GroupId { get; set; }
        public virtual Group? Group { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}