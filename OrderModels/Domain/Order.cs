namespace OrderModels.Domain;

public class Order
{
    public int Id { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal Total { get; private set; }

    //TODO: Talk Step 4 — Delete the Total property above and uncomment the line below it
    //public Money Total { get; private set; } = null!;

    public DateTime OrderDate { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;

    public List<OrderLine> Lines { get; private set; } = new();

    //TODO: Talk Step 5 — Delete the Lines property above and uncomment the two lines below it
    //private readonly List<OrderLine> _lines = new();
    //public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    private Order() { }

    public static Order Place(string customerName, IEnumerable<(int productId, int quantity, Money unitPrice)> lines)
    {
        if (string.IsNullOrWhiteSpace(customerName)) throw new ArgumentException("An order needs a customer name.", nameof(customerName));
        if (!lines.Any()) throw new ArgumentException("An order needs at least one line.", nameof(lines));

        var order = new Order { CustomerName = customerName, OrderDate = DateTime.UtcNow, Status = OrderStatus.Placed };

        foreach (var (productId, quantity, unitPrice) in lines)
        {
            //TODO: Talk Step 5 — Delete the next line and uncomment the line below it
            order.Lines.Add(new OrderLine(productId, quantity, unitPrice));
            //order.AddLine(productId, quantity, unitPrice);
        }

        order.RecalculateTotal();
        return order;
    }

    public void AddLine(int productId, int quantity, Money unitPrice)
    {
        //TODO: Talk Step 5 — Uncomment the duplicate-product guard clause below
        //if (_lines.Any(orderLine => orderLine.ProductId == productId)) throw new InvalidOperationException($"Product {productId} is already on this order.");

        //TODO: Talk Step 5 — Delete the next line and uncomment the two lines below it
        throw new NotImplementedException();
        //_lines.Add(new OrderLine(productId, quantity, unitPrice));
        //RecalculateTotal();
    }

    public void Process()
    {
        //TODO: Talk Step 3 — Delete the next line and uncomment the three lines below it
        throw new NotImplementedException();
        //if (Status != OrderStatus.Placed)
        //    throw new InvalidOperationException($"Cannot process an order in {Status} status.");
        //Status = OrderStatus.Processing;
    }

    public void Confirm()
    {
        //TODO: Talk Step 3 — Delete the next line and uncomment the three lines below it
        throw new NotImplementedException();
        //if (Status != OrderStatus.Processing)
        //    throw new InvalidOperationException($"Cannot confirm an order in {Status} status.");
        //Status = OrderStatus.Confirmed;
    }

    public void Ship()
    {
        //TODO: Talk Step 3 — Delete the next line and uncomment the three lines below it
        throw new NotImplementedException();
        //if (Status != OrderStatus.Confirmed)
        //    throw new InvalidOperationException($"Cannot ship an order in {Status} status.");
        //Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        //TODO: Talk Step 3 — Delete the next line and uncomment the three lines below it
        throw new NotImplementedException();
        //if (Status == OrderStatus.Shipped)
        //    throw new InvalidOperationException("Cannot cancel an order that has shipped.");
        //Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotal()
    {
        //TODO: Talk Step 4 — Delete the next line and uncomment the three lines below it
        Total = Lines.Sum(orderLine => orderLine.LineTotal.Amount);
        //Total = Lines
        //    .Select(orderLine => orderLine.LineTotal)
        //    .Aggregate((runningTotal, nextLineTotal) => runningTotal.Add(nextLineTotal));
    }
}
