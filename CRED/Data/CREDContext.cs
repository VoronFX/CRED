using System;
using CRED.Models;
using CRED.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CRED.Data
{
    public class CREDContext : IdentityDbContext<ApplicationUser>
    {
        public CREDContext(DbContextOptions<CREDContext> options) : base(options)
        {
        }

        public DbSet<TestData> TestData { get; set; }

        public DbSet<Pso2CsvKey> Pso2CsvKeys { get; set; }
        public DbSet<Pso2CsvFile> Pso2CsvFiles { get; set; }
        public DbSet<Pso2CsvFileItem> Pso2CsvFileItems { get; set; }
        public DbSet<Pso2CsvValue> Pso2CsvValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestData>().ToTable("TestData");

			modelBuilder.Entity<Pso2CsvKey>()
		        .HasAlternateKey(e => new { e.Key1Id, e.Key2Id, e.Key3Id, e.Key4 });

	        modelBuilder.Entity<Pso2CsvKey>()
		        .HasOne(e => e.Key1)
				.WithMany()
				.HasForeignKey(e => e.Key1Id)
				.HasPrincipalKey(e => e.Id)
				.IsRequired();

	        modelBuilder.Entity<Pso2CsvKey>()
		        .HasOne(e => e.Key2)
		        .WithMany()
		        .HasForeignKey(e => e.Key2Id)
		        .HasPrincipalKey(e => e.Id)
		        .IsRequired();

	        modelBuilder.Entity<Pso2CsvKey>()
		        .HasOne(e => e.Key3)
		        .WithMany()
		        .HasForeignKey(e => e.Key3Id)
		        .HasPrincipalKey(e => e.Id)
		        .IsRequired();

	        modelBuilder.Entity<Pso2CsvKeyKey1>()
		        .HasAlternateKey(e => e.Value);

	        modelBuilder.Entity<Pso2CsvKeyKey2>()
		        .HasAlternateKey(e => e.Value);

	        modelBuilder.Entity<Pso2CsvKeyKey3>()
		        .HasAlternateKey(e => e.Value);

			modelBuilder.Entity<Pso2CsvFile>()
		        .HasMany(e => e.Items)
		        .WithOne(e => e.File)
		        .HasForeignKey(e => e.FileId)
		        .HasPrincipalKey(e => e.Id)
		        .IsRequired();

	        modelBuilder.Entity<Pso2CsvFile>()
		        .HasAlternateKey(e => e.Name);

			//     modelBuilder.Entity<Pso2CsvFileItem>()
			//.HasOne(e => e.File)
			//      .WithMany(e => e.Items)
			//      .HasForeignKey(e => e.FileId)
			//      .HasPrincipalKey(e => e.Id)
			//.IsRequired();

			modelBuilder.Entity<Pso2CsvFileItem>()
		        .HasKey(e => new { e.Id, e.FileId });

	        modelBuilder.Entity<Pso2CsvFileItem>()
		        .HasOne(e => e.Key)
		        .WithMany()
		        .HasForeignKey(e => e.KeyId)
		        .HasPrincipalKey(e => e.Id)
				.IsRequired();

	        modelBuilder.Entity<Pso2CsvFileItem>()
		        .HasMany(e => e.Values)
		        .WithOne(e => e.FileItem)
				.HasForeignKey(e => new { e.FileItemId, e.FileId })
				.HasPrincipalKey(e => new { e.Id, e.FileId })
		        .IsRequired();

	   //     modelBuilder.Entity<Pso2CsvFileItem>()
		  //      .HasOne(e => e.Value)
		  //      .WithOne(e => e.FileItem)
		  //      .HasForeignKey<Pso2CsvValue>(e => new { e.FileItemId, e.FileId })
				//.HasPrincipalKey<Pso2CsvFileItem>(e => new { e.Id, e.FileId })
				//.IsRequired();

			//modelBuilder.Entity<Pso2CsvFileItem>()
			//	.Property(e => e.Value)
			//       .IsRequired();

			modelBuilder.Entity<Pso2CsvValue>()
		        .HasKey(e => new { e.Id, e.FileItemId, e.FileId});

			modelBuilder.Entity<Pso2CsvValue>()
		        .HasOne(e => e.File)
				.WithMany()
				.HasForeignKey(e => e.FileId)
				.HasPrincipalKey(e => e.Id)
		        .IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			base.OnModelCreating(modelBuilder);
        }
    }
}
