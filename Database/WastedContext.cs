using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using WastedApi.Models;
using WastedApi.Requests;

namespace WastedApi.Database
{
    public partial class WastedContext : DbContext
    {
        public WastedContext()
        {
        }

        public WastedContext(DbContextOptions<WastedContext> options)
            : base(options)
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Category>("category_enum");
        }
        public virtual DbSet<Member> Members { get; set; } = null!;
        public virtual DbSet<Offer> Offers { get; set; } = null!;
        public virtual DbSet<OfferEntry> OfferEntries { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Vendor> Vendors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost:54321;Database=wasted;Username=postgres;Password=postgres");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum("category_enum", new[] { "groceries", "drinks", "meat", "sweets", "other" });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("members");

                entity.HasIndex(e => e.Email, "members_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "members_user_name_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnType("character varying")
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasColumnType("character varying")
                    .HasColumnName("first_name");

                entity.Property(e => e.Hash)
                    .HasColumnType("character varying")
                    .HasColumnName("hash");

                entity.Property(e => e.LastName)
                    .HasColumnType("character varying")
                    .HasColumnName("last_name");

                entity.Property(e => e.Role)
                    .HasColumnType("character varying")
                    .HasColumnName("role");

                entity.Property(e => e.UserName)
                    .HasColumnType("character varying")
                    .HasColumnName("user_name");

                entity.Property(e => e.VendorId).HasColumnName("vendor_id");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.VendorId)
                    .HasConstraintName("members_vendor_id_fkey");
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.ToTable("offers");

                entity.HasIndex(e => new { e.Name, e.Weight }, "offers_name_weight_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AddedBy).HasColumnName("added_by");

                entity.Property(e => e.Name)
                    .HasColumnType("character varying")
                    .HasColumnName("name");

                entity.Property(e => e.Weight).HasColumnName("weight");
                entity.Property(e => e.Category).HasColumnName("category");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Offers)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("offers_added_by_fkey");
            });

            modelBuilder.Entity<OfferEntry>(entity =>
            {
                entity.ToTable("offer_entries");

                entity.HasIndex(e => new { e.OfferId, e.Expiry, e.Added }, "offer_entries_offer_id_expiry_added_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Added)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("added");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Expiry)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("expiry");

                entity.Property(e => e.OfferId).HasColumnName("offer_id");

                entity.HasOne(d => d.Offer)
                    .WithMany(p => p.OfferEntries)
                    .HasForeignKey(d => d.OfferId)
                    .HasConstraintName("offer_entries_offer_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "users_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "users_user_name_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnType("character varying")
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasColumnType("character varying")
                    .HasColumnName("first_name");

                entity.Property(e => e.Hash)
                    .HasColumnType("character varying")
                    .HasColumnName("hash");

                entity.Property(e => e.LastName)
                    .HasColumnType("character varying")
                    .HasColumnName("last_name");

                entity.Property(e => e.Role)
                    .HasColumnType("character varying")
                    .HasColumnName("role");

                entity.Property(e => e.UserName)
                    .HasColumnType("character varying")
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("vendors");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnType("character varying")
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
