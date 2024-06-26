﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScheduleService.Data;

#nullable disable

namespace ScheduleService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ScheduleService.Models.Appointment", b =>
                {
                    b.Property<Guid>("AppointmentUID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AppointmentSlotUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("BookedDateTimeUTC")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ClientUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit");

                    b.HasKey("AppointmentUID");

                    b.HasIndex("AppointmentSlotUid");

                    b.HasIndex("ClientUid");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("ScheduleService.Models.AppointmentSlot", b =>
                {
                    b.Property<Guid>("AppointmentSlotUid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDateTimeUTC")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProviderUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDateTimeUTC")
                        .HasColumnType("datetime2");

                    b.HasKey("AppointmentSlotUid");

                    b.HasIndex("ProviderUid");

                    b.ToTable("AppointmentSlots");
                });

            modelBuilder.Entity("ScheduleService.Models.Client", b =>
                {
                    b.Property<Guid>("ClientUid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimeZoneId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClientUid");

                    b.ToTable("Clients");

                    b.HasData(
                        new
                        {
                            ClientUid = new Guid("a0b2a246-7532-4940-acb1-951099d75df1"),
                            Name = "client",
                            TimeZoneId = "Eastern Standard Time"
                        });
                });

            modelBuilder.Entity("ScheduleService.Models.Provider", b =>
                {
                    b.Property<Guid>("ProviderUid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimeZoneId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProviderUid");

                    b.ToTable("Providers");

                    b.HasData(
                        new
                        {
                            ProviderUid = new Guid("4a63037a-c0d8-4c6b-a1ca-ca9cdd9501a3"),
                            Name = "provider",
                            TimeZoneId = "Central Standard Time"
                        });
                });

            modelBuilder.Entity("ScheduleService.Models.Appointment", b =>
                {
                    b.HasOne("ScheduleService.Models.AppointmentSlot", "AppointmentSlot")
                        .WithMany()
                        .HasForeignKey("AppointmentSlotUid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ScheduleService.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientUid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppointmentSlot");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("ScheduleService.Models.AppointmentSlot", b =>
                {
                    b.HasOne("ScheduleService.Models.Provider", "Provider")
                        .WithMany()
                        .HasForeignKey("ProviderUid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");
                });
#pragma warning restore 612, 618
        }
    }
}
