using MakeItWork.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeItWork.ApplicationDbContext
{
    public class ApplicationDb : DbContext
    {
        public DbSet<UserModel> users { get; set; } = null!;
        public DbSet<UserAuthModel> userAuth { get; set; } = null!;
        public DbSet<TaskModel> tasks { get; set; } = null!;
        public DbSet<ProjectModel> projects { get; set; } = null!;
        public ApplicationDb(DbContextOptions<ApplicationDb> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.taskAuthor)
                .WithMany()
                .HasForeignKey(t => t.taskAuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasMany(t => t.assignedUsers)
                .WithMany();


            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.userAuthModel)
                .WithOne(u => u.userModel)
                .HasForeignKey<UserAuthModel>(u => u.userId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ProjectModel>()
                .HasOne(p => p.owner)
                .WithMany()
                .HasForeignKey(p => p.ownerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectModel>()
                .HasMany(p => p.users)
                .WithMany();

            modelBuilder.Entity<ProjectModel>()
                .HasMany(p => p.tasks)
                .WithOne(t => t.taskProject)
                .HasForeignKey(t => t.taskProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
