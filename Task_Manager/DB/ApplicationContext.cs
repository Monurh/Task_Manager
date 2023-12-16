using Microsoft.EntityFrameworkCore;
using Task_Manager.Model;

namespace Task_Manager.DB
{
    public class ApplicationContext:DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Tasks> Task {  get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasKey(u => u.UserId);
            modelBuilder.Entity<Tasks>()
                .HasKey(u => u.TaskId);
        }
    }
}
