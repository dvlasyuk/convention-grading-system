using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Host.Database.Configurators;
using ConventionGradingSystem.Host.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace ConventionGradingSystem.Host.Database;

/// <summary>
/// Контекст для доступа к базе данных.
/// </summary>
public class DatabaseContext : DbContext
{
    /// <summary>
    /// Создаёт новый экземпляр <see cref="DatabaseContext"/>.
    /// </summary>
    /// <param name="options">Опции для конфигурирования контекста.</param>
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    { }

    /// <summary>
    /// Отзывы экспертов на мероприятия в рамках конкурсов мероприятий.
    /// </summary>
    public DbSet<ExpertFeedback> ExpertFeedbacks => Set<ExpertFeedback>();

    /// <summary>
    /// Оценки мероприятий в рамках конкурсов мероприятий, выставленные экспертами.
    /// </summary>
    public DbSet<ExpertGrade> ExpertGrades => Set<ExpertGrade>();

    /// <summary>
    /// Отзывы участников на мероприятия в рамках конкурсов мероприятий.
    /// </summary>
    public DbSet<ParticipantFeedback> ParticipantFeedbacks => Set<ParticipantFeedback>();

    /// <summary>
    /// Оценки мероприятий в рамках конкурсов мероприятий, выставленные участниками.
    /// </summary>
    public DbSet<ParticipantGrade> ParticipantGrades => Set<ParticipantGrade>();

    /// <summary>
    /// Отметки о посещении участниками мероприятий в рамках конкурсов мероприятий.
    /// </summary>
    public DbSet<AttendanceMark> AttendanceMarks => Set<AttendanceMark>();

    /// <summary>
    /// Голоса участников в рамках зрительских голосований.
    /// </summary>
    public DbSet<ParticipantVote> ParticipantVotes => Set<ParticipantVote>();

    /// <summary>
    /// Конфигурирует модель базы данных.
    /// </summary>
    /// <param name="modelBuilder">Конструктор для конфигурирования модели.</param>
    protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ExpertFeedbackConfigurator());
        modelBuilder.ApplyConfiguration(new ExpertGradeConfigurator());
        modelBuilder.ApplyConfiguration(new ParticipantFeedbackConfigurator());
        modelBuilder.ApplyConfiguration(new ParticipantGradeConfigurator());
        modelBuilder.ApplyConfiguration(new AttendanceMarkConfigurator());
        modelBuilder.ApplyConfiguration(new ParticipantVoteConfigurator());
    }
}
