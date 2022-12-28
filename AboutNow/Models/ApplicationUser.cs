using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AboutNow.Models
{
    // Pasul 1 - useri si roluri
    public class ApplicationUser : IdentityUser
    {

        [ForeignKey("User1_Id")]

        
        public virtual ICollection<Friend>? SentRequests { get; set; }

        
        [ForeignKey("User2_Id")]
       
        public virtual ICollection<Friend>? ReceivedRequests { get; set; }
    }
}
