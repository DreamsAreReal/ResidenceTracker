using Microsoft.EntityFrameworkCore;
using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Entities;

namespace ResidenceTracker.Infastructure.DataAccess.PostgreSql;

public class ApplicationDbContext : DbContext
{
    public DbSet<Bill> Bills { get; set; } = null!;
    public DbSet<Flat> Flats { get; set; } = null!;

    public DbSet<House> Houses { get; set; } = null!;
    public DbSet<Member> Humans { get; set; } = null!;
    public DbSet<ResidencyEventLog> ResidencyEventLogs { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
                                               CancellationToken cancellationToken = default
    )
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<House>().HasMany(x => x.Flats).WithOne().OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Flat>().HasMany(x => x.Members).WithOne().OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Bill>().HasOne(b => b.Flat).WithMany().OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ResidencyEventLog>()
               .HasOne<Flat>(x => x.Flat)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ResidencyEventLog>()
               .HasOne<Member>(x => x.Member)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);
    }

    private void UpdateTimestamps()
    {
        List<AbstractEntity?> entities = ChangeTracker.Entries()
                                                      .Where(
                                                          x => x is
                                                          {
                                                              State: EntityState.Modified,
                                                              Entity: AbstractEntity,
                                                          }
                                                      )
                                                      .Select(x => x.Entity as AbstractEntity)
                                                      .ToList();

        foreach (AbstractEntity? entity in entities)
            entity?.UpModifiedAt();
    }
}