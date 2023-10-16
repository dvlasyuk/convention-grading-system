using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace ConventionGradingSystem.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    { }

    public DbSet<ExpertFeedback> ExpertFeedbacks => Set<ExpertFeedback>();
    public DbSet<ExpertGrade> ExpertGrades => Set<ExpertGrade>();

    public DbSet<ParticipantFeedback> ParticipantFeedbacks => Set<ParticipantFeedback>();
    public DbSet<ParticipantGrade> ParticipantGrades => Set<ParticipantGrade>();

    public DbSet<ParticipationMark> ParticipationMarks => Set<ParticipationMark>();
    public DbSet<SpecialMark> SpecialMarks => Set<SpecialMark>();

    protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<ExpertFeedback>()
            .HasKey(entity => entity.Identifier);

        modelBuilder
            .Entity<ExpertFeedback>()
            .HasMany(entity => entity.Grades)
            .WithOne(entity => entity.Feedback)
            .HasForeignKey(entity => entity.FeedbackId);

        modelBuilder
            .Entity<ExpertFeedback>()
            .Property(entity => entity.Note)
            .HasMaxLength(1000);

        modelBuilder
            .Entity<ExpertGrade>()
            .HasKey(entity => entity.Identifier);

        modelBuilder
            .Entity<ParticipantFeedback>()
            .HasKey(entity => entity.Identifier);

        modelBuilder
            .Entity<ParticipantFeedback>()
            .HasMany(entity => entity.Grades)
            .WithOne(entity => entity.Feedback)
            .HasForeignKey(entity => entity.FeedbackId);

        modelBuilder
            .Entity<ParticipantFeedback>()
            .Property(entity => entity.Note)
            .HasMaxLength(1000);

        modelBuilder
           .Entity<ParticipantGrade>()
           .HasKey(entity => entity.Identifier);

        modelBuilder
            .Entity<ParticipationMark>()
            .HasKey(entity => new { entity.ParticipantId, entity.EventId });

        modelBuilder
            .Entity<ParticipationMark>()
            .Property(entity => entity.ParticipantId)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder
            .Entity<SpecialMark>()
            .HasKey(entity => new { entity.ParticipantId, entity.EventId });

        modelBuilder
            .Entity<SpecialMark>()
            .Property(entity => entity.ParticipantId)
            .HasMaxLength(100)
            .IsRequired();
    }
}
