using Microsoft.EntityFrameworkCore;
using CustomFormsApp.Models;

namespace CustomFormsApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TemplateModel> Templates { get; set; }
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<FormModel> Forms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
