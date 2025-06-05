// File: backend/MoralNavigator.API/Infrastructure/Data/AppDbContext.cs

using System;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoralNavigator.API.Domain.Entities;

namespace MoralNavigator.API.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // -------------------------
        //  DbSet для всех сущностей
        // -------------------------
        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestResult> Results { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------------------------------------
            // 1) Конфигурация сущности User
            // ---------------------------------------------------
            modelBuilder.Entity<User>(builder =>
            {
                builder.ToTable("Users");
                builder.HasKey(u => u.Id);
                builder.Property(u => u.Username)
                       .IsRequired()
                       .HasMaxLength(100);
                builder.Property(u => u.PasswordHash)
                       .IsRequired()
                       .HasMaxLength(200);

                // Навигационное свойство: один пользователь → много результатов
                builder.HasMany(u => u.Results)
                       .WithOne(tr => tr.User)
                       .HasForeignKey(tr => tr.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------------------------------
            // 2) Конфигурация сущности Test
            // ---------------------------------------------------
            modelBuilder.Entity<Test>(builder =>
            {
                builder.ToTable("Tests");
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Title)
                       .IsRequired()
                       .HasMaxLength(200);

                // Один Test → много Question
                builder.HasMany(t => t.Questions)
                       .WithOne(q => q.Test)
                       .HasForeignKey(q => q.TestId)
                       .OnDelete(DeleteBehavior.Cascade);

                // Один Test → много TestResult
                builder.HasMany(t => t.TestResults)
                       .WithOne(tr => tr.Test)
                       .HasForeignKey(tr => tr.TestId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------------------------------
            // 3) Конфигурация сущности Question (включая массив строк Options)
            // ---------------------------------------------------
            modelBuilder.Entity<Question>(builder =>
            {
                builder.ToTable("Questions");
                builder.HasKey(q => q.Id);
                builder.Property(q => q.Text)
                       .IsRequired();

                // Сериализация массива строк Options в JSON
                builder.Property(q => q.Options)
                       .HasConversion(
                           v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                           v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<string>()
                       )
                       .HasColumnType("text")
                       .Metadata
                       .SetValueComparer(new ValueComparer<string[]>(
                           (arr1, arr2) =>
                               arr1 != null && arr2 != null && arr1.SequenceEqual(arr2),
                           arr => arr != null
                               ? arr.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()))
                               : 0,
                           arr => arr != null ? arr.ToArray() : Array.Empty<string>()
                       ));

                builder.Property(q => q.CorrectOption)
                       .IsRequired();

                // Связь: один Question → много UserAnswer
                builder.HasMany(q => q.UserAnswers)
                       .WithOne(ua => ua.Question)
                       .HasForeignKey(ua => ua.QuestionId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------------------------------
            // 4) Конфигурация сущности TestResult
            // ---------------------------------------------------
            modelBuilder.Entity<TestResult>(builder =>
            {
                builder.ToTable("TestResults");
                builder.HasKey(tr => tr.Id);

                builder.Property(tr => tr.TakenAt)
                       .IsRequired();

                builder.Property(tr => tr.Score)
                       .IsRequired();

                // Связь: один TestResult → много UserAnswer
                builder.HasMany(tr => tr.UserAnswers)
                       .WithOne(ua => ua.TestResult)
                       .HasForeignKey(ua => ua.TestResultId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------------------------------
            // 5) Конфигурация сущности UserAnswer
            // ---------------------------------------------------
            modelBuilder.Entity<UserAnswer>(builder =>
            {
                builder.ToTable("UserAnswers");
                builder.HasKey(ua => ua.Id);

                builder.Property(ua => ua.SelectedOption)
                       .IsRequired();

                // (UserAnswer связывается только с Question и TestResult,
                //  с User напрямую связь не настраиваем, потому что TestResult уже содержит UserId)
            });

            // ---------------------------------------------------
            // Здесь можно добавить конфигурации для других сущностей...
            // ---------------------------------------------------
        }
    }
}
