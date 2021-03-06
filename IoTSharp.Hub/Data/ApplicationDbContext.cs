﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace IoTSharp.Hub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
            var _DataBase = configuration["DataBase"] ?? "sqlite";
            if (Enum.TryParse(_DataBase, out DatabaseType databaseType))
            {
                DatabaseType = databaseType;
            }
            if (Database.GetPendingMigrations().Count() > 0)
            {
                Database.Migrate();
            }
        }

        public DatabaseType DatabaseType { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DataStorage>()
           .HasDiscriminator<DataCatalog>(nameof(Data.DataStorage.Catalog))
           .HasValue<DataStorage>(DataCatalog.None)
           .HasValue<AttributeData>(DataCatalog.AttributeData)
                  .HasValue<AttributeLatest>(DataCatalog.AttributeLatest)
                         .HasValue<TelemetryData>(DataCatalog.TelemetryData)
           .HasValue<TelemetryLatest>(DataCatalog.TelemetryLatest);

            modelBuilder.Entity<AttributeData>().HasDiscriminator<DataCatalog>(nameof(Data.DataStorage.Catalog));
            modelBuilder.Entity<AttributeLatest>().HasDiscriminator<DataCatalog>(nameof(Data.DataStorage.Catalog));
            modelBuilder.Entity<TelemetryData>().HasDiscriminator<DataCatalog>(nameof(Data.DataStorage.Catalog));
            modelBuilder.Entity<TelemetryLatest>().HasDiscriminator<DataCatalog>(nameof(Data.DataStorage.Catalog));

            switch (DatabaseType)
            {
                case DatabaseType.mssql:
                    ForSqlServer(modelBuilder);
                    break;

                case DatabaseType.npgsql:
                    ForNpgsql(modelBuilder);
                    break;

                case DatabaseType.sqlite:
                    break;

                default:
                    break;
            }
        }

        private void ForNpgsql(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelemetryData>()
            .Property(b => b.Value_Json)
            .HasColumnType("jsonb");

            modelBuilder.Entity<TelemetryData>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");

            modelBuilder.Entity<AttributeLatest>()
            .Property(b => b.Value_Json)
            .HasColumnType("jsonb");

            modelBuilder.Entity<AttributeLatest>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");

            modelBuilder.Entity<AttributeData>()
            .Property(b => b.Value_Json)
            .HasColumnType("jsonb");

            modelBuilder.Entity<AttributeData>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");

            modelBuilder.Entity<TelemetryLatest>()
            .Property(b => b.Value_Json)
            .HasColumnType("jsonb");

            modelBuilder.Entity<TelemetryLatest>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");

            modelBuilder.Entity<AuditLog>()
            .Property(b => b.ActionData)
            .HasColumnType("jsonb");

            modelBuilder.Entity<AuditLog>()
            .Property(b => b.ActionResult)
            .HasColumnType("jsonb");

 
        }

        private void ForSqlServer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelemetryData>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");

            modelBuilder.Entity<AttributeLatest>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");

            modelBuilder.Entity<DataStorage>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");

            modelBuilder.Entity<TelemetryLatest>()
            .Property(b => b.Value_XML)
            .HasColumnType("xml");
        }

        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Relationship> Relationship { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<TelemetryData> TelemetryData { get; set; }
        public DbSet<AttributeLatest> AttributeLatest { get; set; }
        public DbSet<DataStorage> DataStorage { get; set; }
        public DbSet<AttributeData> AttributeData { get; set; }
        public DbSet<TelemetryLatest> TelemetryLatest { get; set; }
        public DbSet<DeviceIdentity> DeviceIdentities { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
    }
}