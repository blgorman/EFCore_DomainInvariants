namespace OrderModels.Anemic;

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

    public OrderLine(int productId, int quantity, Money unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
