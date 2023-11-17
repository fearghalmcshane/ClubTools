﻿// <auto-generated />
using System;
using ClubTools.Api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClubTools.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231113223825_AddClubEvents")]
    partial class AddClubEvents
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ClubTools.Api.Entities.Activity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Detail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Activites");
                });

            modelBuilder.Entity("ClubTools.Api.Entities.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PublishedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("ClubTools.Api.Entities.ClubEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("EntryPrice")
                        .HasColumnType("float");

                    b.Property<DateTime>("EventDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPlanned")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Venue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ClubEvents");
                });

            modelBuilder.Entity("ClubTools.Api.Entities.Activity", b =>
                {
                    b.OwnsOne("System.Collections.Generic.List<string>", "Equipment", b1 =>
                        {
                            b1.Property<Guid>("ActivityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Capacity")
                                .HasColumnType("int");

                            b1.HasKey("ActivityId");

                            b1.ToTable("Activites");

                            b1.ToJson("Equipment");

                            b1.WithOwner()
                                .HasForeignKey("ActivityId");
                        });

                    b.OwnsOne("System.Collections.Generic.List<string>", "StepVariations", b1 =>
                        {
                            b1.Property<Guid>("ActivityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Capacity")
                                .HasColumnType("int");

                            b1.HasKey("ActivityId");

                            b1.ToTable("Activites");

                            b1.ToJson("StepVariations");

                            b1.WithOwner()
                                .HasForeignKey("ActivityId");
                        });

                    b.Navigation("Equipment")
                        .IsRequired();

                    b.Navigation("StepVariations")
                        .IsRequired();
                });

            modelBuilder.Entity("ClubTools.Api.Entities.Article", b =>
                {
                    b.OwnsOne("ClubTools.Api.Entities.Article.Tags#List", "Tags", b1 =>
                        {
                            b1.Property<Guid>("ArticleId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Capacity")
                                .HasColumnType("int");

                            b1.HasKey("ArticleId");

                            b1.ToTable("Articles");

                            b1.ToJson("Tags");

                            b1.WithOwner()
                                .HasForeignKey("ArticleId");
                        });

                    b.Navigation("Tags")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
