namespace OrderModels.Anemic;

public class Order
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderLine> Lines { get; set; } = new();
}
