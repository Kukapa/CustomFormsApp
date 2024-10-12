﻿using Microsoft.EntityFrameworkCore;
using CustomFormsApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CustomFormsApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TemplateModel> Templates { get; set; }
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<FormModel> Forms { get; set; }
        public DbSet<AnswerModel> Answers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
