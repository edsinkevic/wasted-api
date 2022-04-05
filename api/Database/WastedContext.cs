using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Wasted.Database.Interfaces;
using WastedApi.Models;

namespace WastedApi.Database;
public partial class WastedContext : DbContext, IWastedContext
{
    public WastedContext()
    {
    }

    public WastedContext(DbContextOptions<WastedContext> options)
        : base(options)
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Category>("category_enum");
    }

    public virtual DbSet<Customer> Customers { get; set; } = null!;
    public virtual DbSet<Member> Members { get; set; } = null!;
    public virtual DbSet<Offer> Offers { get; set; } = null!;
    public virtual DbSet<OfferEntry> OfferEntries { get; set; } = null!;
    public virtual DbSet<Reservation> Reservations { get; set; } = null!;
    public virtual DbSet<ReservationItem> ReservationItems { get; set; } = null!;
    public virtual DbSet<Vendor> Vendors { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseNpgsql("Host=localhost:54321;Database=Wasted;Username=postgres;Password=postgres");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("category_enum", new[] { "groceries", "drinks", "meat", "sweets", "other" });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");

            entity.HasIndex(e => e.Email, "customers_email_key")
                .IsUnique();

            entity.HasIndex(e => e.UserName, "customers_user_name_key")
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

            entity.Property(e => e.UserName)
                .HasColumnType("character varying")
                .HasColumnName("user_name");
        });

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

            entity.Property(e => e.Price).HasColumnName("price");

            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Vendor)
                .WithMany(p => p.Offers)
                .HasForeignKey(d => d.AddedBy)
                .HasConstraintName("offers_added_by_fkey");
        });

        modelBuilder.Entity<OfferEntry>(entity =>
        {
            entity.ToTable("offer_entries");

            entity.HasIndex(e => new { e.OfferId, e.Expiry }, "offer_entries_offer_id_expiry_key")
                .IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.Added).HasColumnName("added");

            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.Property(e => e.Expiry).HasColumnName("expiry");

            entity.Property(e => e.OfferId).HasColumnName("offer_id");

            entity.HasOne(d => d.Offer)
                .WithMany(p => p.OfferEntries)
                .HasForeignKey(d => d.OfferId)
                .HasConstraintName("offer_entries_offer_id_fkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("reservations");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");

            entity.Property(e => e.CreatedDate).HasColumnName("created_date");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");

            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("reservations_customer_id_fkey");
        });

        modelBuilder.Entity<ReservationItem>(entity =>
        {
            entity.ToTable("reservation_items");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.Property(e => e.EntryId).HasColumnName("entry_id");

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");

            entity.HasOne(d => d.Entry)
                .WithMany(p => p.ReservationItems)
                .HasForeignKey(d => d.EntryId)
                .HasConstraintName("reservation_items_entry_id_fkey");

            entity.HasOne(d => d.Reservation)
                .WithMany(p => p.ReservationItems)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("reservation_items_reservation_id_fkey");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.ToTable("vendors");

            entity.HasIndex(e => e.Name, "vendors_name_key")
                .IsUnique();

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
