using bloomteq.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        public void Remove<T>(T entity) where T : class
        {
            Set<T>().Remove(entity);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
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
