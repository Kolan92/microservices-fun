﻿// <auto-generated />
using CommandService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CommandService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211123202129_ChangeForeignKey")]
    partial class ChangeForeignKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CommandService.Models.Command", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CommandLine")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HowTo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlatformId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PlatformId");

                    b.ToTable("Commands");
                });

            modelBuilder.Entity("CommandService.Models.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ExternalId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Platforms");
                });

            modelBuilder.Entity("CommandService.Models.Command", b =>
                {
                    b.HasOne("CommandService.Models.Platform", "Platform")
                        .WithMany("Commands")
                        .HasForeignKey("PlatformId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("CommandService.Models.Platform", b =>
                {
                    b.Navigation("Commands");
                });
#pragma warning restore 612, 618
        }
    }
}
