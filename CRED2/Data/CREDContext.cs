using System;
using System.Collections.Generic;
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

	    public DbSet<State> StateStore { get; private set; }
		public DbSet<BranchHead> Heads { get; private set; }
		public DbSet<Commit> Commits { get; private set; }
		public DbSet<Change> Changes { get; private set; }
		public DbSet<Key> Keys { get; private set; }

		//public DbSet<TestData> TestData { get; set; }

		//public DbSet<Pso2CsvKey> Pso2CsvKeys { get; set; }
		//public DbSet<Pso2CsvFile> Pso2CsvFiles { get; set; }
		//public DbSet<Pso2CsvFileItem> Pso2CsvFileItems { get; set; }
		//public DbSet<Pso2CsvValue> Pso2CsvValues { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<BranchHead>()
				.HasKey(e => e.Name);

			modelBuilder.Entity<BranchHead>()
				.HasOne(e => e.Head)
				.WithOne();

			//.IsConcurrencyToken()
			//.IsRequired();

			modelBuilder.Entity<Commit>()
				.HasKey(e => e.Hash);

			modelBuilder.Entity<Commit>()
				.Property(e => e.Hash)
				.HasMaxLength(40);

			modelBuilder.Entity<Commit>()
				.HasMany(e => e.Parents)
				.WithOne(e => e.Child)
				.HasForeignKey(e => e.ChildHash)
				.HasPrincipalKey(e => e.Hash)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Commit>()
				.HasMany(e => e.Children)
				.WithOne(e => e.Parent)
				.HasForeignKey(e => e.ParentHash)
				.HasPrincipalKey(e => e.Hash)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Commit>()
				.HasMany(e => e.Changes)
				.WithOne(e => e.Commit)
				.HasForeignKey(e => e.CommitHash);

			modelBuilder.Entity<CommitParentPair>()
				.HasKey(e => new { e.ChildHash, e.ParentHash });

			modelBuilder.Entity<CommitParentPair>()
				.Property(e => e.ChildHash)
				.HasMaxLength(40);

			modelBuilder.Entity<CommitParentPair>()
				.Property(e => e.ParentHash)
				.HasMaxLength(40);

			modelBuilder.Entity<Change>()
				.HasKey(e => new { e.CommitHash, e.KeyId});

			modelBuilder.Entity<Change>()
				.HasOne(e => e.Key)
				.WithMany()
				.HasForeignKey(e => e.KeyId)
				.IsRequired();

			modelBuilder.Entity<Change>()
				.Property(e => e.CommitHash)
				.HasMaxLength(40);

			modelBuilder.Entity<Change>()
				.HasIndex(e => new { e.CommitHash, e.KeyId })
				.IsUnique();

			modelBuilder.Entity<Key>()
				.HasIndex(e=> new { e.Path, e.KeyParts})
				.IsUnique();

			modelBuilder.Entity<State>()
				.HasKey(e => e.Key);

			base.OnModelCreating(modelBuilder);
        }
    }
}
