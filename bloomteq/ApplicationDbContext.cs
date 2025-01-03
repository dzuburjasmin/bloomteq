using bloomteq.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace bloomteq
{
    public interface IApplicationDbContext
    {
        DbSet<Shift> Shifts { get; set; }
        DbSet<User> Users { get; set; }
        int SaveChanges();
        void Remove<T>(T entity) where T : class;

    }
    public class ApplicationDbContext : IdentityDbContext<User> , IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        public void Remove<T>(T entity) where T : class
        {
            Set<T>().Remove(entity);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server =(localdb)\\mssqllocaldb;Database=BLOOMTEQ;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "USERS");
            });
            builder.Entity<Shift>()
                .HasOne(t => t.User)
                .WithMany(c => c.Shifts)
                .HasForeignKey(d => d.UserId).
                OnDelete(DeleteBehavior.Cascade);
        }
    }
}
