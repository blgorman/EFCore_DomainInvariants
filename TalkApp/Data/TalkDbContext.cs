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
        modelBuilder.Entity<Anemic.Order>(orderBuilder =>
        {
            orderBuilder.ToTable("AnemicOrders");
            orderBuilder.Property(order => order.Total).HasPrecision(18, 2);
            orderBuilder.HasMany(order => order.Lines).WithOne().HasForeignKey("OrderId");
        });

        modelBuilder.Entity<Anemic.OrderLine>(orderLineBuilder =>
        {
            orderLineBuilder.ToTable("AnemicOrderLines");
            orderLineBuilder.OwnsOne(orderLine => orderLine.UnitPrice, moneyBuilder =>
            {
                moneyBuilder.Property(moneyValue => moneyValue.Amount).HasColumnName("UnitPrice_Amount").HasPrecision(18, 2);
                moneyBuilder.Property(moneyValue => moneyValue.Currency).HasColumnName("UnitPrice_Currency").HasMaxLength(10);
            });
        });

        modelBuilder.Entity<Domain.Order>(orderBuilder =>
        {
            orderBuilder.ToTable("DomainOrders");

            orderBuilder.Property(order => order.Status).HasConversion<string>().HasMaxLength(20);

            //TODO: Talk Step 4 — Delete the next line and uncomment the five lines below it
            orderBuilder.Property(order => order.Total).HasPrecision(18, 2);
            //orderBuilder.OwnsOne(order => order.Total, moneyBuilder =>
            //{
            //    moneyBuilder.Property(moneyValue => moneyValue.Amount).HasColumnName("Total_Amount").HasPrecision(18, 2);
            //    moneyBuilder.Property(moneyValue => moneyValue.Currency).HasColumnName("Total_Currency").HasMaxLength(10);
            //});

            orderBuilder.HasMany(order => order.Lines).WithOne().HasForeignKey("OrderId");

            //TODO: Talk Step 5 — Uncomment the line below
            //orderBuilder.Navigation(order => order.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Domain.OrderLine>(orderLineBuilder =>
        {
            orderLineBuilder.ToTable("DomainOrderLines");
            orderLineBuilder.OwnsOne(orderLine => orderLine.UnitPrice, moneyBuilder =>
            {
                moneyBuilder.Property(moneyValue => moneyValue.Amount).HasColumnName("UnitPrice_Amount").HasPrecision(18, 2);
                moneyBuilder.Property(moneyValue => moneyValue.Currency).HasColumnName("UnitPrice_Currency").HasMaxLength(10);
            });
        });
    }
}
