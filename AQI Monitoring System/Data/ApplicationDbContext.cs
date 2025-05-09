﻿using AQI_Monitoring_System.Models;
using Microsoft.EntityFrameworkCore;

namespace AQI_Monitoring_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<AqiReading> AqiReadings { get; set; }
        public DbSet<AlertThreshold> AlertThresholds { get; set; }
        public DbSet<SimulationConfig> SimulationConfigs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map the User entity to a table named "User"
            modelBuilder.Entity<User>().ToTable("User");
            // Map the Sensor entity to a table named "Sensors"
            modelBuilder.Entity<Sensor>().ToTable("Sensors");
            modelBuilder.Entity<AqiReading>().ToTable("AqiReadings");
            modelBuilder.Entity<Sensor>()
                .HasKey(s => s.SensorId); // SensorId is the primary key (string)

            modelBuilder.Entity<AqiReading>()
                .HasOne(r => r.Sensor)
                .WithMany()
                .HasForeignKey(r => r.SensorId)
                .HasPrincipalKey(s => s.SensorId) // Explicitly map to Sensor.SensorId
                .OnDelete(DeleteBehavior.Cascade); // Adjust as needed
        }
    }
}