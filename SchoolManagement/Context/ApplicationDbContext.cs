using Microsoft.EntityFrameworkCore;
using SchoolManagement.Model;

namespace SchoolManagement.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<StudentRequest> studentRequests { get; set; }  

        public DbSet<AddStudent> Student {  get; set; }

        public DbSet<Users> Users { get; set; }
        public DbSet<AddLibrarian> Librarian { get; set; }


     }
}
