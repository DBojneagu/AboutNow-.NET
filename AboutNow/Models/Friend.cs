using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AboutNow.Models
{
    public class Friend
    {
        [Key]
        public int FriendshipId { get; set; }
        public string? User1_Id { get; set; }
        public virtual ApplicationUser? User1 { get; set; }

        public string? User2_Id { get; set; }
        public virtual ApplicationUser? User2 { get; set; }

        public DateTime RequestTime { get; set; }
        public bool? Accepted { get; set; }
    }
}