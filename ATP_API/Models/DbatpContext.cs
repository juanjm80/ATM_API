using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ATP_API.Models;

public partial class DbatpContext : DbContext
{
    public DbatpContext()
    {
    }

    public DbatpContext(DbContextOptions<DbatpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HistorialRefreshToken> HistorialRefreshTokens { get; set; }

    public virtual DbSet<Retiro> Retiros { get; set; }

    public virtual DbSet<Tarjetum> Tarjeta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistorialRefreshToken>(entity =>
        {
            entity.HasKey(e => e.IdHistorialToken).HasName("PK__Historia__03DC48A5F754BD99");

            entity.ToTable("HistorialRefreshToken");

            entity.Property(e => e.EsActivo).HasComputedColumnSql("(case when [FechaExpiracion]<getdate() then CONVERT([bit],(0)) else CONVERT([bit],(1)) end)", false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTarjetaNavigation).WithMany(p => p.HistorialRefreshTokens)
                .HasForeignKey(d => d.IdTarjeta)
                .HasConstraintName("FK__Historial__IdTar__4CA06362");
        });

        modelBuilder.Entity<Retiro>(entity =>
        {
            entity.HasKey(e => e.IdRetiro).HasName("PK__Retiro__A0F85CFD9EB17437");

            entity.ToTable("Retiro");

            entity.Property(e => e.CodigoOperacion)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaExtraccion).HasColumnType("datetime");
            entity.Property(e => e.Retiro1)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Retiro");

            entity.HasOne(d => d.IdTarjetaNavigation).WithMany(p => p.Retiros)
                .HasForeignKey(d => d.IdTarjeta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Retiro__IdTarjet__49C3F6B7");
        });

        modelBuilder.Entity<Tarjetum>(entity =>
        {
            entity.HasKey(e => e.IdTarjeta).HasName("PK__Tarjeta__6AF43C150BDAC934");

            entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Codigo)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Disponible).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTarjeta)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Pin)
                .HasMaxLength(4)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
