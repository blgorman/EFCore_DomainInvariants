namespace OrderModels.Domain;

public enum OrderStatus
{
    Placed = 0,
    Processing = 1,
    Confirmed = 2,
    Shipped = 3,
    Cancelled = 4
}
