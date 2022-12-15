using AboutNow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AboutNow.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Journal> Journals { get; set; }
        public DbSet<Category> Categories {get; set; }
        public DbSet<Comment> Comments { get; set; }


    }
}