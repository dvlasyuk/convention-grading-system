using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Configurators;
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

    public DbSet<AttendanceMark> AttendanceMarks => Set<AttendanceMark>();

    protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ExpertFeedbackConfigurator());
        modelBuilder.ApplyConfiguration(new ExpertGradeConfigurator());

        modelBuilder.ApplyConfiguration(new ParticipantFeedbackConfigurator());
        modelBuilder.ApplyConfiguration(new ParticipantGradeConfigurator());

        modelBuilder.ApplyConfiguration(new AttendanceMarkConfigurator());
    }
}
