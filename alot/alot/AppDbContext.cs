using alot.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace alot
{
    public class AppDbContext: IdentityDbContext<User>
    {
        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TokenValidator> validator { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("identity");
            modelBuilder.Entity<TodoList>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .IsRequired(true);

           
        }
    }
}
