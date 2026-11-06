namespace OrderModels.Domain;

public class Order
{
    public int Id { get; private set; }

    public OrderStatus Status { get; private set; }


    public Money Total { get; private set; } = null!;

    public DateTime OrderDate { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;


    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    private Order() { }

    public static Order Place(string customerName, IEnumerable<(int productId, int quantity, Money unitPrice)> lines)
    {
        if (string.IsNullOrWhiteSpace(customerName)) throw new ArgumentException("An order needs a customer name.", nameof(customerName));
        if (!lines.Any()) throw new ArgumentException("An order needs at least one line.", nameof(lines));

        var order = new Order { CustomerName = customerName, OrderDate = DateTime.UtcNow, Status = OrderStatus.Placed };

        foreach (var (productId, quantity, unitPrice) in lines)
        {
            order.AddLine(productId, quantity, unitPrice);
        }

        order.RecalculateTotal();
        return order;
    }

    public void AddLine(int productId, int quantity, Money unitPrice)
    {
        if (_lines.Any(orderLine => orderLine.ProductId == productId)) throw new InvalidOperationException($"Product {productId} is already on this order.");

        _lines.Add(new OrderLine(productId, quantity, unitPrice));
        RecalculateTotal();
    }

    public void Process()
    {
        if (Status != OrderStatus.Placed)
            throw new InvalidOperationException($"Cannot process an order in {Status} status.");
        Status = OrderStatus.Processing;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException($"Cannot confirm an order in {Status} status.");
        Status = OrderStatus.Confirmed;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException($"Cannot ship an order in {Status} status.");
        Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel an order that has shipped.");
        Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotal()
    {
        Total = Lines
            .Select(orderLine => orderLine.LineTotal)
            .Aggregate((runningTotal, nextLineTotal) => runningTotal.Add(nextLineTotal));
    }
}
