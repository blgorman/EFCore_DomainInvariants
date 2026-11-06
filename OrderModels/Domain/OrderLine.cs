namespace OrderModels.Domain;

public class OrderLine
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private OrderLine()
    {
        UnitPrice = null!;
    }

    //TODO: Talk Step 5 — Delete the next line and uncomment the line below it
    public OrderLine(int productId, int quantity, Money unitPrice)
    //internal OrderLine(int productId, int quantity, Money unitPrice)
    {
        //TODO: Talk Step 5 — Uncomment the two guard clauses below
        //if (productId <= 0) throw new ArgumentException("A line needs a valid product id.", nameof(productId));
        //if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
