﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql;

#nullable disable

namespace ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.Bill", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("AmountInRubles")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FlatId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsReadonly")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("PaidIn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FlatId");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.Flat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("HouseId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsReadonly")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("HouseId");

                    b.ToTable("Flats");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.House", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsReadonly")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Houses");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("FlatId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsReadonly")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FlatId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.ResidencyEventLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EventType")
                        .HasColumnType("integer");

                    b.Property<Guid>("FlatId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsReadonly")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FlatId");

                    b.HasIndex("MemberId");

                    b.ToTable("ResidencyEventLogs");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.Bill", b =>
                {
                    b.HasOne("ResidenceTracker.Domain.Entities.Flat", "Flat")
                        .WithMany("Bills")
                        .HasForeignKey("FlatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Flat");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.Flat", b =>
                {
                    b.HasOne("ResidenceTracker.Domain.Entities.House", null)
                        .WithMany("Flats")
                        .HasForeignKey("HouseId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.Member", b =>
                {
                    b.HasOne("ResidenceTracker.Domain.Entities.Flat", null)
                        .WithMany("Members")
                        .HasForeignKey("FlatId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.ResidencyEventLog", b =>
                {
                    b.HasOne("ResidenceTracker.Domain.Entities.Flat", "Flat")
                        .WithMany()
                        .HasForeignKey("FlatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ResidenceTracker.Domain.Entities.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Flat");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.Flat", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("ResidenceTracker.Domain.Entities.House", b =>
                {
                    b.Navigation("Flats");
                });
#pragma warning restore 612, 618
        }
    }
}
