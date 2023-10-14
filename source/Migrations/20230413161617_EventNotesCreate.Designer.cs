﻿// <auto-generated />
using ConventionGradingSystem.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230413161617_EventNotesCreate")]
    partial class EventNotesCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ExpertGrade", b =>
                {
                    b.Property<int>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GradeTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GradeValue")
                        .HasColumnType("INTEGER");

                    b.HasKey("Identifier");

                    b.ToTable("ExpertGrades");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ExpertNote", b =>
                {
                    b.Property<int>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("ExpertNotes");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ParticipantGrade", b =>
                {
                    b.Property<int>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GradeTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GradeValue")
                        .HasColumnType("INTEGER");

                    b.HasKey("Identifier");

                    b.ToTable("ParticipantGrades");
                });

            modelBuilder.Entity("ConventionGradingSystem.Database.Entities.ParticipantNote", b =>
                {
                    b.Property<int>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("ParticipantNotes");
                });
#pragma warning restore 612, 618
        }
    }
}
