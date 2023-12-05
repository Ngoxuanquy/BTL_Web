using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Models;

public partial class BtlWebNcContext : DbContext
{
    public BtlWebNcContext()
    {
    }

    public BtlWebNcContext(DbContextOptions<BtlWebNcContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Thi> This { get; set; }

    public virtual DbSet<Transition> Transitions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=XUANQUY\\SQLEXPRESS;Database=BTL_Web_NC;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Adrees)
                .HasMaxLength(254)
                .IsFixedLength();
            entity.Property(e => e.Number)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status)
                .HasMaxLength(254)
                .IsFixedLength();

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Orders_Products");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductDes)
                .HasMaxLength(254)
                .IsFixedLength();
            entity.Property(e => e.ProductImg)
                .HasMaxLength(254)
                .IsFixedLength();
            entity.Property(e => e.ProductName)
                .HasMaxLength(254)
                .IsFixedLength();
        });

        modelBuilder.Entity<Thi>(entity =>
        {
            entity.HasKey(e => e.MaTs);

            entity.ToTable("Thi");

            entity.Property(e => e.MaTs).HasColumnName("MaTS");
            entity.Property(e => e.TenTs)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("TenTS");
        });

        modelBuilder.Entity<Transition>(entity =>
        {
            entity.ToTable("Transition");

            entity.Property(e => e.TransitionId).HasColumnName("transitionId");
            entity.Property(e => e.Diachi)
                .HasMaxLength(200)
                .IsFixedLength()
                .HasColumnName("diachi");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.Sodienthoai)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("sodienthoai");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Order).WithMany(p => p.Transitions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Transition_Orders");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Roles)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
