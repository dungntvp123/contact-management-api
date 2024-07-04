using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRN_ProjectAPI.Models;

public partial class PhoneBookDbContext : DbContext
{
    public PhoneBookDbContext()
    {
    }

    public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contacts__3214EC27356F223D");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ContactName).HasMaxLength(50);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Label)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasColumnType("text");

            entity.HasOne(d => d.Creator).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contacts_CreatorID");
        });

        modelBuilder.Entity<PhoneNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhoneNum__3214EC27BBA7D57C");

            entity.HasIndex(e => e.PhoneNumber1, "UQ__PhoneNum__85FB4E387F939D14").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Password)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber1)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("PhoneNumber");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
