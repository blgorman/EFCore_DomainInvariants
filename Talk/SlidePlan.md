# Creating Domain Invariants — Slide Plan

> 33 slides. Five steps cover seven techniques in build order: close the way in, lock the properties, add the status behavior, fix `Money`, then fix the lines. Each guard clause goes in with the code it protects. Every step runs in the same order: a problem slide (the anemic code and what goes wrong), the technique slide(s), then a demo slide. Presenter guidance (which TODOs to apply, which option to run) lives in the speaker notes, never on a slide.
> Built on `Template.pptx`. Code slides show the domain model after that step's TODOs are applied, with unapplied later-step TODO comments left out. Runtime values quoted in `**Say:**` come from a `tools/verify_steps.sh` run on 2026-11-06, plus all five options run after each step.
> Titles name the concept and its solution. A problem slide is titled "<Concept>: The Problem".
> Speaker notes for each slide = the `**Guidance:**` line (demo slides only) followed by the `**Say:**` text.

---

## Segment: 0–5 min: The anemic `Order` and the desired invariants

## Slide 1 — Creating Domain Invariants

**Layout:** Template title slide (Name Card). Title and subtitle only.

**Text on slide (verbatim, shown all at once):**
```
Creating Domain Invariants
Hardening an anemic Order, one technique at a time
```

**Say:**
"This talk starts with the anemic `Order` from the EF Core and DDD course. Our code hardens a copy of that `Order` one technique at a time, while the original anemic `Order` stays broken beside the hardened copy."

---

## Slide 2 — Who am I?

**Layout:** Template bio slide, unchanged.

**Text on slide (verbatim, shown all at once):**
```
(Template bio slide, unchanged)
```

**Say:**
"My name is Brian Gorman. I train developers on .NET, Azure, and EF Core, and I write the Practical Entity Framework Core books with Apress."

---

## Slide 3 — Agenda

**Layout:** Template agenda slide: photo on the left half, four bullets on the right.

**Text on slide (verbatim, revealed one at a time):**
```
The anemic Order
The desired Order invariants
Seven techniques to make the invariant model
The anemic Order next to the domain Order
```

**Say:**
"The talk opens with the anemic `Order`. [Bullet 1 appears] Next come the rules we want that `Order` to keep. [Bullet 2 appears] Each of the seven techniques starts with a problem on the anemic `Order` and fixes that problem on the domain `Order`. [Bullet 3 appears] The recap puts the two `Order` classes side by side. [Bullet 4 appears]"

---

## Slide 4 — The Anemic Order

**Layout:** Title Only layout. One dark code panel with the file path label above it. Code copied from `OrderModels/Anemic/Order.cs` (namespace line omitted).

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic/Order.cs

public class Order
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderLine> Lines { get; set; } = new();
}
```

**Say:**
"The anemic `Order` has six properties, and every property has a public setter. Any caller can set `Status` to any string and set `Total` to any decimal. The `Order` class has no method that recalculates `Total` when `Lines` changes."

---

## Slide 5 — The Desired Order Invariants

**Layout:** Title Only layout. Six cards in a 3 x 2 grid, each with a number and one invariant. Each card fades in on click, in number order.

**Text on slide (verbatim, revealed one at a time):**
```
1  Every order has a customer and at least one line.
2  The order total equals the sum of its lines.
3  Every quantity is positive, and each product appears once.
4  Every line on an order uses the same currency.
5  An order moves from Placed to Processing to Confirmed to Shipped.
6  A cancelled order cannot be shipped.
```

**Say:**
"An invariant is a rule that holds for every `Order` at every moment. [Card 1 appears] An order needs a customer and a line. [Card 2 appears] The total matches the lines. [Card 3 appears] Quantities are positive, and products are unique. [Card 4 appears] An order uses one currency. [Card 5 appears] The status moves forward in order, [Card 6 appears] and a cancelled order stays unshipped. The anemic `Order` keeps none of these six rules."

---

## Segment: 5–13 min: Step 1, private constructor, factory method, and `Order.Place` guards

## Slide 6 — Private Constructor: The Problem

**Layout:** Title Only layout. Code panel labeled `OrderModels/Anemic`: the call and the printed results, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic

var order = new Order();

Customer:    ''
Status:      ''
Order date:  1/1/0001
Lines:       0
Total:       0

The public constructor lets any caller skip Order.Place.
```

**Say:**
"The anemic `Order` has a public constructor, so `new Order()` creates an order with an empty customer name, an empty `Status`, an order date of 1/1/0001, and zero lines. The domain `Order` still has the same public constructor, so a caller can skip `Order.Place`."

---

## Slide 7 — Creating an Order: The Problem

**Layout:** Title Only layout. Code panel labeled `OrderModels/Anemic`: the object initializer from `TalkDemo.cs` and the reloaded values as comments, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic

var order = new Order { Status = "Banana", Total = -99.99m };

// Saved and reloaded through EF Core:
// Customer: ''   Status: 'Banana'   Order date: 1/1/0001
// Lines: 0       Total: -99.99

Any caller can build any order, and EF Core saves it.
```

**Say:**
"The anemic `Order` lets the caller decide every value. Our code builds an order with a `Status` of Banana and a `Total` of -99.99, and never sets a customer, a date, or a line. EF Core saves that order to SQL Server and reloads every one of those values."

---

## Slide 8 — The Order Constructor Becomes Private

**Layout:** Title Only layout. Two code panels stacked: the constructor from `Order.cs` after Step 1 on top, the line mapping from `TalkDbContext.cs` below, with two plain-text lines below the panels.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 1
private Order() { }

TalkApp/Data/TalkDbContext.cs
order.HasMany(o => o.Lines).WithOne().HasForeignKey("OrderId");

EF Core rebuilds a reloaded order through the private constructor.
OrderLine has no OrderId property: OrderId is a shadow foreign key.
```

**Say:**
"After step 1, the `Order` constructor is private, so `TalkApp` cannot call `new Order()`, and `Order.Place` is the only way to create an order. EF Core still calls the private constructor when it materializes an order from SQL Server. The same reload shows a shadow foreign key: `OrderLine` has no `OrderId` property, and `HasForeignKey("OrderId")` keeps the column in SQL Server only."

---

## Slide 9 — Creating Orders with Order.Place

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Order.cs`: the `Place` method at Step 1. The `Place` signature and the object initializer are wrapped to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 1

public static Order Place(string customerName,
    IEnumerable<(int productId, int quantity, Money unitPrice)> lines)
{
    var order = new Order
    {
        CustomerName = customerName,
        OrderDate = DateTime.UtcNow,
        Status = OrderStatus.Placed
    };
    foreach (var (productId, quantity, unitPrice) in lines)
    {
        order.Lines.Add(new OrderLine(productId, quantity, unitPrice));
    }
    order.RecalculateTotal();
    return order;
}
```

**Say:**
"The domain `Order` has a static factory method, `Order.Place`. One call sets the customer, the order date, the `Placed` status, and the lines, and then calculates `Total`. With the constructor private, every order starts here. The guard clauses on the next slide make `Order.Place` reject a bad order."

---

## Slide 10 — Guard Clauses in Order.Place

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Order.cs`, after Step 1: the guards from `Place`. Each guard is wrapped after the condition to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 1

// Order.Place
if (string.IsNullOrWhiteSpace(customerName))
    throw new ArgumentException("An order needs a customer name.", nameof(customerName));
if (!lines.Any())
    throw new ArgumentException("An order needs at least one line.", nameof(lines));
```

**Say:**
"After step 1, `Order.Place` rejects a blank customer name and an empty line list. `Money` gets its guards in step 4, and the lines get theirs in step 5, each with the code it protects."

---

## Slide 11 — Demo: Private Constructor and Order.Place

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Private Constructor and Order.Place
No public Order(), and Order.Place rejects a bad order
```

**Guidance:** Apply every `//TODO: Talk Step 1` (`OrderModels/Domain/Order.cs`, then `TalkApp/TalkDemo.cs` last). Run option 1, Private constructor and Order.Place.

**Say:**
"The anemic `Order` prints an empty `new Order()`, then reloads an order with an empty customer, a `Status` of Banana, a date of 1/1/0001, zero lines, and a `Total` of -99.99. After step 1, the domain `Order` has no public constructor. `Order.Place` creates the order for Contoso, and EF Core reloads it with a `Status` of `Placed`, today's date, 2 lines, and a `Total` of 44.98. A blank customer and an empty line list are rejected, and the line's shadow `OrderId` holds the order's `Id`."

---

## Segment: 13–17 min: Step 2, private setters

## Slide 12 — Private Setters: The Problem

**Layout:** Title Only layout. Code panel labeled `OrderModels/Anemic`: three properties and the three assignments from `TalkDemo.cs`, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic

public decimal Total { get; set; }
public DateTime OrderDate { get; set; }
public string CustomerName { get; set; } = string.Empty;

order.OrderDate = DateTime.Now.AddDays(-30);
order.CustomerName = "Someone Else";
order.Total = 0.01m;

Every setter is public, so any caller can change a placed order.
```

**Say:**
"Our code places an order for Contoso with a `Total` of 39.98. Our code then moves `OrderDate` back 30 days, changes `CustomerName` to Someone Else, and sets `Total` to 0.01. The anemic `Order` accepts all three changes, so an invoice for Contoso can belong to a different customer next week."

---

## Slide 13 — Private Setters

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Order.cs`, after Step 2: the five property lines.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 2

public int Id { get; private set; }
public OrderStatus Status { get; private set; }
public decimal Total { get; private set; }
public DateTime OrderDate { get; private set; }
public string CustomerName { get; private set; } = string.Empty;
```

**Say:**
"After step 2, each of the five properties has a private setter. Code inside the `Order` class, such as `Order.Place`, still sets `CustomerName`, `OrderDate`, and `Status`. Code outside the `Order` class can read the five properties and cannot assign them."

---

## Slide 14 — Demo: Private Setters

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Private Setters
Order.Place sets every property, and no outside caller can
```

**Guidance:** Apply every `//TODO: Talk Step 2` (`OrderModels/Domain/Order.cs`, then `TalkApp/TalkDemo.cs` last). Run option 2, Private setters.

**Say:**
"The anemic `Order` prints the moved `OrderDate`, the customer Someone Else, and a `Total` of 0.01. After step 2, the domain `Order` reports a private setter for `Id`, `Status`, `Total`, `OrderDate`, and `CustomerName`, and `Order.Place` still creates the order for Contoso."

---

## Segment: 17–22 min: Step 3, domain methods for Status

## Slide 15 — Domain Methods: The Problem

**Layout:** Title Only layout. Two code panels stacked: the anemic assignments on top, the domain assignment after Step 2 below, with one plain-text line below the panels.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic
order.Status = "Shipped";        // a Placed order, never confirmed
cancelled.Status = "Shipped";    // a Cancelled order

OrderModels/Domain, after Step 2
order.Status = OrderStatus.Shipped;   // no longer compiles

Nothing in the domain Order can move Status forward yet.
```

**Say:**
"The anemic `Order` ships a `Placed` order that was never confirmed, and ships a `Cancelled` order. After step 2, the private setter blocks the assignment on the domain `Order`. However, the domain `Order` has no method that moves `Status` from `Placed` to `Shipped` yet."

---

## Slide 16 — Domain Methods Guard Transitions

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Order.cs`, after Step 3: `Ship()` and `Cancel()`.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 3

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
```

**Say:**
"After step 3, `Ship()` checks for `Confirmed` before setting `Status` to `Shipped`, so a `Placed` order and a `Cancelled` order both throw. The `Cancel()` method refuses a shipped order. The `Process()` and `Confirm()` methods guard their own transitions the same way."

---

## Slide 17 — Demo: Domain Methods

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Domain Methods Control Status
Ship() rejects a Placed order and a Cancelled order
```

**Guidance:** Apply every `//TODO: Talk Step 3` (`OrderModels/Domain/Order.cs`, then `TalkApp/TalkDemo.cs` last). Run option 3, Domain methods control Status.

**Say:**
"The anemic `Order` prints `Shipped` for the `Placed` order and for the `Cancelled` order. After step 3, `Ship()` prints Cannot ship an order in Placed status and Cannot ship an order in Cancelled status. Our code then calls `Process()`, `Confirm()`, and `Ship()` in sequence, and `Status` reaches `Shipped`."

---

## Segment: 22–30 min: Step 4, Money value object and its guards

## Slide 18 — Value Object: The Problem

**Layout:** Title Only layout. Code panel labeled `OrderModels/Anemic`: the `Total` property, the sum from `TalkDemo.cs`, and the printed results as comments, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic

public decimal Total { get; set; }

// Line 1: 19.99 USD    Line 2: 25.00 EUR
order.Total = order.Lines.Sum(l => l.LineTotal.Amount);
// Total: 44.99   (dollars? euros?)

A decimal Total has no currency.
```

**Say:**
"The anemic `Total` is a `decimal`. Our code adds a 19.99 USD line and a 25.00 EUR line, and `Total` becomes 44.99 with no currency at all. The `Total` column in SQL Server holds the same bare number."

---

## Slide 19 — Money Guards: The Problem

**Layout:** Title Only layout. Code panel labeled `OrderModels/Anemic`: the two `Money` calls from `TalkDemo.cs`, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic

Money.Create(-42.50m, "USD");                                   // -42.50 USD
Money.Create(19.99m, "USD").Add(Money.Create(25.00m, "EUR"));   // 44.99 USD

Nothing checks the amount or the currency, so every value is accepted.
```

**Say:**
"`Money` has no checks either. `Money.Create` accepts -42.50, and adding 25.00 EUR to 19.99 USD returns 44.99 USD."

---

## Slide 20 — Using a Value Object for the Total

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Order.cs`, after Step 4: the `Total` property and `RecalculateTotal`.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 4

public Money Total { get; private set; } = null!;

private void RecalculateTotal()
{
    Total = Lines.Select(l => l.LineTotal).Aggregate((sum, next) => sum.Add(next));
}
```

**Say:**
"After step 4, `Total` has the type `Money`, so every total carries an amount and a currency. The `RecalculateTotal` method adds the line totals with `Money.Add` instead of summing bare decimals. Assigning `-42.50m` to `Total` no longer compiles."

---

## Slide 21 — Mapping Money with OwnsOne

**Layout:** Title Only layout. Code panel labeled `TalkApp/Data/TalkDbContext.cs`, after Step 4, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
TalkApp/Data/TalkDbContext.cs, after Step 4

order.OwnsOne(o => o.Total, money =>
{
    money.Property(m => m.Amount).HasColumnName("Total_Amount").HasPrecision(18, 2);
    money.Property(m => m.Currency).HasColumnName("Total_Currency").HasMaxLength(10);
});

Total_Amount and Total_Currency are two columns on the DomainOrders table.
```

**Say:**
"EF Core cannot build the model for a `Money` `Total` until `TalkDbContext` maps `Total` with `OwnsOne`. The `OwnsOne` call stores `Total` in two columns on the `DomainOrders` table, `Total_Amount` and `Total_Currency`."

---

## Slide 22 — Guard Clauses in Money

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Money.cs`, after Step 4. The `Money.Add` throw is wrapped to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Money.cs, after Step 4

// Money.Create
if (amount < 0)
    throw new ArgumentException("Amount cannot be negative.", nameof(amount));
if (string.IsNullOrWhiteSpace(currency))
    throw new ArgumentException("Currency cannot be empty.", nameof(currency));

// Money.Add
if (other.Currency != Currency)
    throw new InvalidOperationException(
        $"Cannot add {other.Currency} to {Currency}: currency mismatch.");
```

**Say:**
"The `Money` value object guards itself. `Money.Create` rejects a negative amount and a blank currency, and `Money.Add` throws an `InvalidOperationException` when the two currencies differ, so an order total can never mix dollars and euros."

---

## Slide 23 — Demo: Using a Value Object for the Total

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Total as a Money Value Object
59.97 USD saves and reloads, and SQL Server keeps the currency
```

**Guidance:** Apply every `//TODO: Talk Step 4` (`OrderModels/Domain/Money.cs`, `OrderModels/Domain/Order.cs`, `TalkApp/Data/TalkDbContext.cs`, then `TalkApp/TalkDemo.cs` last). Run option 4, Total as a Money value object.

**Say:**
"The anemic `Order` prints a `Total` of 44.99 for a dollar line and a euro line, and `Money` accepts -42.50 and USD plus EUR. After step 4, the domain `Total` is a `Money` of 59.97 USD, the order reloads as 59.97 USD, and SQL Server holds `Total_Amount` 59.97 and `Total_Currency` USD. A negative amount, USD plus EUR, and an order with a USD line and a EUR line are each rejected."

---

## Segment: 30–38 min: Step 5, backing field, read-only collection, and line guards

## Slide 24 — Backing Field: The Problem

**Layout:** Title Only layout. Code panel labeled `OrderModels/Anemic`: the `Lines` property, the two calls from `TalkDemo.cs`, and the printed results as comments, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic

public List<OrderLine> Lines { get; set; } = new();

order.Lines.Add(new OrderLine(2002, 2, Money.Create(5.00m, "USD")));
// Lines: 2   Sum of lines: 20.00   Total: 10.00

order.Lines.Clear();
// Lines: 0   Total: 10.00

Any caller can change Lines, and nothing recalculates Total.
```

**Say:**
"The anemic `Lines` property is a public `List<OrderLine>`. Our code adds a second line, and the lines sum to 20.00 while `Total` stays at 10.00. Our code then clears `Lines`, and an order with no lines still reports a `Total` of 10.00."

---

## Slide 25 — A Private Backing Field

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain`, after Step 5: `_lines`, `Lines`, `AddLine`, and `RecalculateTotal` from `Order.cs`, and the constructor line from `OrderLine.cs`.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain, after Step 5

private readonly List<OrderLine> _lines = new();
public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

public void AddLine(int productId, int quantity, Money unitPrice)
{
    _lines.Add(new OrderLine(productId, quantity, unitPrice));
    RecalculateTotal();
}

private void RecalculateTotal()
{
    Total = Lines.Select(l => l.LineTotal).Aggregate((sum, next) => sum.Add(next));
}

// OrderLine.cs
internal OrderLine(int productId, int quantity, Money unitPrice)
```

**Say:**
"After step 5, the private `_lines` field stores the lines, and `Lines` exposes `_lines` as a read-only collection. The `AddLine` method is the one path that adds a line, and `AddLine` calls `RecalculateTotal`, which sets `Total` to the sum of the lines. `Order.Place` now calls `AddLine` too. The `internal` constructor stops `TalkApp` from calling `new OrderLine(...)`."

---

## Slide 26 — Mapping the Backing Field

**Layout:** Title Only layout. Code panel labeled `TalkApp/Data/TalkDbContext.cs`, after Step 5, with two plain-text lines below the panel.

**Text on slide (verbatim, shown all at once):**
```
TalkApp/Data/TalkDbContext.cs, after Step 5

order.Navigation(o => o.Lines)
    .HasField("_lines")
    .UsePropertyAccessMode(PropertyAccessMode.Field);

EF Core finds _lines by convention when the name matches Lines.
Any other field name requires HasField.
```

**Say:**
"The read-only `Lines` property has no setter, so EF Core loads the lines into the `_lines` field. EF Core finds `_lines` by naming convention, because `_lines` matches `Lines`. A field named `_orderLines` breaks the convention, so the `HasField("_lines")` call states the mapping in `TalkDbContext` instead of relying on the name."

---

## Slide 27 — Line Guards: The Problem

**Layout:** Title Only layout. Code panel labeled `OrderModels/Anemic`: the three lines from `TalkDemo.cs`, with one plain-text line below the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic

order.Lines.Add(new OrderLine(2001, -3, Money.Create(19.99m, "USD")));
order.Lines.Add(new OrderLine(0, 1, Money.Create(5.00m, "USD")));
order.Lines.Add(new OrderLine(2001, 2, Money.Create(19.99m, "USD")));

Nothing checks the lines, so every line is accepted.
```

**Say:**
"Our code adds a line with a quantity of -3, a line with product 0, and product 2001 a second time. The anemic `Order` and `OrderLine` have no checks, so every line is accepted."

---

## Slide 28 — Guard Clauses on the Lines

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain`, after Step 5: the guards from `AddLine` and the `OrderLine` constructor. Each guard is wrapped after the condition to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain, after Step 5

// Order.AddLine
if (_lines.Any(l => l.ProductId == productId))
    throw new InvalidOperationException($"Product {productId} is already on this order.");

// OrderLine constructor
if (productId <= 0)
    throw new ArgumentException("A line needs a valid product id.", nameof(productId));
if (quantity <= 0)
    throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
```

**Say:**
"After step 5, the `AddLine` method rejects a product that is already on the order, and the `OrderLine` constructor rejects a product id or a quantity of zero or less."

---

## Slide 29 — Demo: Backing Field

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Backing Field and Read-Only Lines
AddLine keeps Total in step, and EF Core reloads both lines
```

**Guidance:** Apply every `//TODO: Talk Step 5` (`OrderModels/Domain/Order.cs`, `OrderModels/Domain/OrderLine.cs`, `TalkApp/Data/TalkDbContext.cs`, then `TalkApp/TalkDemo.cs` last). Run option 5, Backing field and read-only Lines.

**Say:**
"The anemic `Order` reports a `Total` of 10.00 with lines that sum to 20.00, and then with no lines at all, and it accepts all three bad lines. After step 5, `AddLine` moves `Total` from 10.00 USD to 20.00 USD, and EF Core saves the order and reloads 2 lines with a `Total` of 20.00 USD. Quantity -3, product 0, and product 2001 twice are each rejected."

---

## Segment: 38–45 min: Recap + Q&A

## Slide 30 — Seven Techniques, Seven Rules

**Layout:** Title Only layout. Seven-row table: step, technique, what the technique prevents, and the code that enforces it.

**Text on slide (verbatim, shown all at once):**
```
Step | Technique                  | What it prevents                               | Code that enforces it
1    | Private constructor        | An order that skips Order.Place                | private Order(), EF Core materialization
1    | Factory method             | Orders built with missing or invalid values    | Order.Place
2    | Private setters            | Callers rewriting a placed order               | private set on five properties
3    | Domain methods             | Shipping an unconfirmed or cancelled order     | Process, Confirm, Ship, Cancel
4    | Money value object         | A total with no currency                       | Money, RecalculateTotal, OwnsOne
5    | Read-only backing field    | A total that differs from the sum of the lines | _lines, AddLine, RecalculateTotal, HasField
1, 4, 5 | Guard clauses              | Bad input, duplicates, mixed currencies        | Guards in Place, AddLine, OrderLine, Money
```

**Say:**
"Each technique moves one rule from the caller into the `Order`. Most rules need more than one piece of code. The total stays correct because `_lines` blocks outside changes and because `AddLine` calls `RecalculateTotal` on every new line."

---

## Slide 31 — The Anemic and Domain Orders

**Layout:** Title Only layout. Two code panels stacked: `OrderModels/Anemic/Order.cs` on top, `OrderModels/Domain/Order.cs` after Step 5 below it (state only, methods omitted). Stacked rather than side by side so the code stays readable at 14pt or larger.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic/Order.cs
public int Id { get; set; }
public string Status { get; set; } = string.Empty;
public decimal Total { get; set; }
public DateTime OrderDate { get; set; }
public string CustomerName { get; set; } = string.Empty;
public List<OrderLine> Lines { get; set; } = new();

OrderModels/Domain/Order.cs, after Step 5
public int Id { get; private set; }
public OrderStatus Status { get; private set; }
public Money Total { get; private set; } = null!;
public DateTime OrderDate { get; private set; }
public string CustomerName { get; private set; } = string.Empty;
private readonly List<OrderLine> _lines = new();
public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
private Order() { }
```

**Say:**
"The anemic `Order` on top still accepts every invalid value from the seven problems. The domain `Order` below the anemic `Order` has private setters, a read-only `Lines` collection, and a private constructor, and `OrderModels/Domain` has no EF Core reference."

---

## Slide 32 — Questions?

**Layout:** Template questions slide, unchanged.

**Text on slide (verbatim, shown all at once):**
```
(Template questions slide, unchanged)
```

**Say:**
"Thank you for your time. I am happy to take questions about the seven techniques, the EF Core mapping, or the demo code."

---

## Slide 33 — Conclusion

**Layout:** Template conclusion slide, unchanged, with its existing speaker note.

**Text on slide (verbatim, shown all at once):**
```
(Template conclusion slide, unchanged)
```

**Say:**
"Thank you for attending my talk today. Please reach out and connect."
