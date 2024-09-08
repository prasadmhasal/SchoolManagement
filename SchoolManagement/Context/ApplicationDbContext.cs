using Microsoft.EntityFrameworkCore;
using SchoolManagement.Model;

namespace SchoolManagement.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<StudentRequest> studentRequests { get; set; }  
    }
}
