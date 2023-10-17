﻿// <auto-generated />
using System;
using ConventionGradingSystem.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConventionGradingSystem.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20231017212507_ParticipantVotesCreation")]
    partial class ParticipantVotesCreation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.AttendanceMark", b =>
                {
                    b.Property<string>("ParticipantId")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("EventId")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("SpecialMark")
                        .HasColumnType("INTEGER");

                    b.HasKey("ParticipantId", "EventId");

                    b.ToTable("AttendanceMarks");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ExpertFeedback", b =>
                {
                    b.Property<Guid>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("EventId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("ExpertFeedbacks");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ExpertGrade", b =>
                {
                    b.Property<Guid>("FeedbackId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CriterionId")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("GradeValue")
                        .HasColumnType("INTEGER");

                    b.HasKey("FeedbackId", "CriterionId");

                    b.ToTable("ExpertGrades");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ParticipantFeedback", b =>
                {
                    b.Property<Guid>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("EventId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("ParticipantFeedbacks");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ParticipantGrade", b =>
                {
                    b.Property<Guid>("FeedbackId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CriterionId")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("GradeValue")
                        .HasColumnType("INTEGER");

                    b.HasKey("FeedbackId", "CriterionId");

                    b.ToTable("ParticipantGrades");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ParticipantVote", b =>
                {
                    b.Property<Guid>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("ParticipantId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("VoitingParticipantId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("ParticipantVotes");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ExpertGrade", b =>
                {
                    b.HasOne("ConventionGradingSystem.Database.Entities.ExpertFeedback", "Feedback")
                        .WithMany("Grades")
                        .HasForeignKey("FeedbackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feedback");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ParticipantGrade", b =>
                {
                    b.HasOne("ConventionGradingSystem.Database.Entities.ParticipantFeedback", "Feedback")
                        .WithMany("Grades")
                        .HasForeignKey("FeedbackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feedback");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ExpertFeedback", b =>
                {
                    b.Navigation("Grades");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ParticipantFeedback", b =>
                {
                    b.Navigation("Grades");
                });
#pragma warning restore 612, 618
        }
    }
}
