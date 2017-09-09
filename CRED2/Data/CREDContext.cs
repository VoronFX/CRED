using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CRED.Models;
using CRED2.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CRED.Data
{
	public class CREDContext : IdentityDbContext<ApplicationUser>
	{
		public CREDContext(DbContextOptions<CREDContext> options) : base(options)
		{

			//Keys.
		}

		//public DbSet<State> StateStore { get; private set; }
		//public DbSet<Branch> Branches { get; private set; }
		//public DbSet<Change> Changes { get; private set; }
		//public DbSet<Key> Keys { get; private set; }

		//public DbSet<TestData> TestData { get; set; }

		//public DbSet<Pso2CsvKey> Pso2CsvKeys { get; set; }
		//public DbSet<Pso2CsvFile> Pso2CsvFiles { get; set; }
		//public DbSet<Pso2CsvFileItem> Pso2CsvFileItems { get; set; }
		//public DbSet<Pso2CsvValue> Pso2CsvValues { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<Branch>()
			//	.HasKey(e => e.Name);

			//modelBuilder.Entity<Branch>()
			//	.Property(e => e.CommitHash)
			//	.IsConcurrencyToken()
			//	.HasMaxLength(40);

			//modelBuilder.Entity<Change>()
			//	.HasKey(e => new { e.CommitHash, e.KeyId });

			//modelBuilder.Entity<Change>()
			//	.HasOne(e => e.Key)
			//	.WithMany()
			//	.HasForeignKey(e => e.KeyId)
			//	.IsRequired();

			//modelBuilder.Entity<Change>()
			//	.Property(e => e.CommitHash)
			//	.HasMaxLength(40);

			//modelBuilder.Entity<Change>()
			//	.HasIndex(e => new { e.CommitHash, e.KeyId })
			//	.IsUnique();

			//modelBuilder.Entity<Key>()
			//	.HasIndex(e => new { e.Path, e.KeyParts })
			//	.IsUnique();

			//modelBuilder.Entity<State>()
			//	.HasKey(e => e.Key);

			base.OnModelCreating(modelBuilder);
		}
	}
}
