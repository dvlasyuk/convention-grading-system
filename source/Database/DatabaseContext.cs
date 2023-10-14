using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace ConventionGradingSystem.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }

        public DbSet<ExpertGrade> ExpertGrades => Set<ExpertGrade>();
        public DbSet<ExpertNote> ExpertNotes => Set<ExpertNote>();

        public DbSet<ParticipantGrade> ParticipantGrades => Set<ParticipantGrade>();
        public DbSet<ParticipantNote> ParticipantNotes => Set<ParticipantNote>();

        public DbSet<ParticipationMark> ParticipationMarks => Set<ParticipationMark>();
        public DbSet<SpecialMark> SpecialMarks => Set<SpecialMark>();

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ExpertGrade>()
                .HasKey(entity => entity.Identifier);

            modelBuilder
                .Entity<ExpertNote>()
                .HasKey(entity => entity.Identifier);

            modelBuilder
                .Entity<ExpertNote>()
                .Property(entity => entity.Note)
                .HasMaxLength(1000)
                .IsRequired();

            modelBuilder
               .Entity<ParticipantGrade>()
               .HasKey(entity => entity.Identifier);

            modelBuilder
               .Entity<ParticipantNote>()
               .HasKey(entity => entity.Identifier);

            modelBuilder
                .Entity<ParticipantNote>()
                .Property(entity => entity.Note)
                .HasMaxLength(1000)
                .IsRequired();

            modelBuilder
                .Entity<ParticipationMark>()
                .HasKey(entity => new { entity.ParticipantId, entity.EventTypeId, entity.EventId });

            modelBuilder
                .Entity<ParticipationMark>()
                .Property(entity => entity.ParticipantId)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder
                .Entity<SpecialMark>()
                .HasKey(entity => new { entity.ParticipantId, entity.EventTypeId, entity.EventId });

            modelBuilder
                .Entity<SpecialMark>()
                .Property(entity => entity.ParticipantId)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
