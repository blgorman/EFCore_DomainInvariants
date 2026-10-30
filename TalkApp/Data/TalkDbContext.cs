using Microsoft.EntityFrameworkCore;
using Anemic = OrderModels.Anemic;
using Domain = OrderModels.Domain;

namespace TalkApp.Data;

public class TalkDbContext : DbContext
{
    public TalkDbContext(DbContextOptions<TalkDbContext> options)
        : base(options)
    {
    }

    public DbSet<Anemic.Order> AnemicOrders => Set<Anemic.Order>();
    public DbSet<Domain.Order> DomainOrders => Set<Domain.Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anemic.Order>(order =>
        {
            order.ToTable("AnemicOrders");
            order.Property(o => o.Total).HasPrecision(18, 2);
            order.HasMany(o => o.Lines).WithOne().HasForeignKey("OrderId");
        });

        modelBuilder.Entity<Anemic.OrderLine>(line =>
        {
            line.ToTable("AnemicOrderLines");
            line.OwnsOne(l => l.UnitPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("UnitPrice_Amount").HasPrecision(18, 2);
                money.Property(m => m.Currency).HasColumnName("UnitPrice_Currency").HasMaxLength(10);
            });
        });

        modelBuilder.Entity<Domain.Order>(order =>
        {
            order.ToTable("DomainOrders");

            //TODO: Talk Step 6 — Delete the next line and uncomment the five lines below it
            order.Property(o => o.Total).HasPrecision(18, 2);
            //order.OwnsOne(o => o.Total, money =>
            //{
            //    money.Property(m => m.Amount).HasColumnName("Total_Amount").HasPrecision(18, 2);
            //    money.Property(m => m.Currency).HasColumnName("Total_Currency").HasMaxLength(10);
            //});

            order.HasMany(o => o.Lines).WithOne().HasForeignKey("OrderId");

            //TODO: Talk Step 7 — Uncomment the two lines below
            //order.Navigation(o => o.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
            //order.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Domain.OrderLine>(line =>
        {
            line.ToTable("DomainOrderLines");
            line.OwnsOne(l => l.UnitPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("UnitPrice_Amount").HasPrecision(18, 2);
                money.Property(m => m.Currency).HasColumnName("UnitPrice_Currency").HasMaxLength(10);
            });
        });
    }
}
