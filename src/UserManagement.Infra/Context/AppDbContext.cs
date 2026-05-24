using Microsoft.EntityFrameworkCore;

namespace UserManagement.Infra.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.User> Users => Set<Domain.Entities.User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Domain.Entities.User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(254);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.DataNascimento).IsRequired().HasColumnType("date");
            entity.Property(e => e.DataCriacao).IsRequired().HasColumnType("date");
            entity.Property(e => e.DataEdicao).IsRequired().HasColumnType("date");
        });
    }
}