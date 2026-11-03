# Creating Domain Invariants — Slide Plan

> 27 slides. Demo segments are marked with transition slides that name the menu option.
> Built on `Template.pptx`. Code slides show the good model after that step's TODOs are applied, with unapplied later-step TODO comments left out. Runtime values quoted in `**Say:**` come from a `tools/verify_steps.sh` run on 2026-11-02.

---

## Segment: 0–4 min: Slides, the `Order` invariants

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
The Order invariants
The anemic Order and its seven problems
Seven techniques, applied live
The anemic Order next to the domain Order
```

**Say:**
"The talk opens with the rules an `Order` has to keep. [Bullet 1 appears] Our code then runs seven problems against the anemic `Order`. [Bullet 2 appears] Each of the seven techniques fixes one problem live, [Bullet 3 appears] and the recap puts the anemic `Order` next to the domain `Order`. [Bullet 4 appears]"

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

## Slide 5 — The Order Invariants

**Layout:** Title Only layout. Six cards in a 3 x 2 grid, each with a number and one invariant.

**Text on slide (verbatim, revealed one at a time):**
```
1  A customer and at least one line
2  Total equals the sum of the lines
3  Positive quantities, each product once
4  One currency per order
5  Placed > Processing > Confirmed > Shipped
6  A cancelled order cannot be shipped
```

**Say:**
"An invariant is a rule that holds for every `Order` at every moment. [Cards appear one at a time] The anemic `Order` keeps none of these six rules. The domain `Order` moves each rule into its own code, one technique at a time."

---

## Slide 6 — Two Models, One Demo

**Layout:** Title Only layout. Two cards side by side (bad model on the left, good model on the right), with one line across the bottom naming `TalkApp/TalkDemo.cs`.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic
The bad model
Order, OrderLine, Money
Never edited

OrderModels/Domain
The good model
Starts as a copy of the bad model
Changes on stage

TalkApp/TalkDemo.cs: option N runs the problem on the bad model, then the solution on the good model
```

**Say:**
"The solution holds two copies of the same model. `OrderModels/Anemic` never changes, and `OrderModels/Domain` starts as a copy and changes on stage. Each menu option in `TalkDemo.cs` runs a problem against the anemic `Order` and then runs the solution against the domain `Order`."

---

## Slide 7 — The Talk Step Pattern

**Layout:** Title Only layout. Code panel at the top with the TODO block copied from `OrderModels/Domain/Order.cs`, three short bullets below it.

**Text on slide (verbatim, shown all at once):**
```
//TODO: Talk Step 2 — Delete the next line and uncomment the line below it
public DateTime OrderDate { get; set; }
//public DateTime OrderDate { get; private set; }

Delete the live line, uncomment the line below it
Nothing is typed on stage
Before step N, option N shows "Not Yet Implemented"
```

**Say:**
"Every change in the talk is staged under a `//TODO: Talk Step` comment. The live line sits directly under the comment, and the replacement sits commented below the live line. On stage, I delete the live line and uncomment the replacement."

---

## Segment: 4–9 min: The bad model, options 1 to 7

## Slide 8 — Demo: The Anemic Order

**Layout:** Template demo slide (photo with dark banner). Title, one line of body text, menu option line.

**Text on slide (verbatim, shown all at once):**
```
Demo
Every problem runs, every solution waits
Options 1 to 7
```

**Say:**
"Our code now runs all seven menu options before any step is applied. Each option prints a problem box from the anemic `Order`, and then prints a Not Yet Implemented box where the solution will appear."

---

## Segment: 9–13 min: Step 1, enum

## Slide 9 — Status as an Enum

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain`, after Step 1: `OrderStatus.cs` and the `Status` property from `Order.cs`.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain, after Step 1

public enum OrderStatus
{
    Placed = 0,
    Processing = 1,
    Confirmed = 2,
    Shipped = 3,
    Cancelled = 4
}

public OrderStatus Status { get; set; }
```

**Say:**
"The `OrderStatus` enum defines five values, from `Placed` to `Cancelled`. After step 1, the `Status` property has the type `OrderStatus`, so assigning `"Banana"` to `Status` no longer compiles."

---

## Slide 10 — Demo: Status Is a Free String

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Step 1: Status becomes OrderStatus
Option 1: Status is a free string
```

**Say:**
"Option 1 assigns `"Shiped"` and `"Banana"` to the anemic `Status`, and the compiler accepts both strings. After step 1, option 1 lists the five `OrderStatus` values and prints `False` for both `"Shiped"` and `"Banana"`."

---

## Segment: 13–18 min: Step 2, private setters + domain methods

## Slide 11 — Private Setters and Domain Methods

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Order.cs`, after Step 2: the five property lines and the `Ship()` method.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 2

public int Id { get; private set; }
public OrderStatus Status { get; private set; }
public decimal Total { get; private set; }
public DateTime OrderDate { get; private set; }
public string CustomerName { get; private set; } = string.Empty;

public void Ship()
{
    if (Status != OrderStatus.Confirmed)
        throw new InvalidOperationException($"Cannot ship an order in {Status} status.");
    Status = OrderStatus.Shipped;
}
```

**Say:**
"After step 2, every `Order` property has a private setter, so code outside the `Order` class cannot assign `Status`. The `Ship()` method checks for `Confirmed` before setting `Status` to `Shipped`, and `Process()`, `Confirm()`, and `Cancel()` guard their own transitions the same way."

---

## Slide 12 — Demo: Shipping Without Confirming

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Step 2: private setters, Process, Confirm, Ship, Cancel
Option 2: Shipping without confirming
```

**Say:**
"Option 2 sets the anemic `Status` to `"Shipped"` on a `Placed` order, and then changes `CustomerName` to Someone Else. After step 2, `Ship()` on the `Placed` order prints Cannot ship an order in Placed status, and `Process()`, `Confirm()`, and `Ship()` in sequence reach `Shipped`."

---

## Segment: 18–23 min: Step 3, read-only collection + backing field

## Slide 13 — A Read-Only Collection

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain`, after Step 3: `_lines`, `Lines`, `AddLine` from `Order.cs`, and the `OrderLine` constructor line from `OrderLine.cs`.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain, after Step 3

private readonly List<OrderLine> _lines = new();
public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

public void AddLine(int productId, int quantity, Money unitPrice)
{
    _lines.Add(new OrderLine(productId, quantity, unitPrice));
    RecalculateTotal();
}

// OrderLine.cs
internal OrderLine(int productId, int quantity, Money unitPrice)
```

**Say:**
"After step 3, the `_lines` field stores the lines, and `Lines` exposes `_lines` as an `IReadOnlyCollection`. The `AddLine` method adds the line and calls `RecalculateTotal()`. The `internal` constructor on `OrderLine` stops `TalkApp` from calling `new OrderLine(...)`."

---

## Slide 14 — Demo: Lines Changed Behind the Order's Back

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Step 3: _lines, AddLine, internal OrderLine constructor
Option 3: Lines changed behind the order's back
```

**Say:**
"Option 3 adds a line to the anemic `Lines` list, and `Total` stays at 10.00 while the lines sum to 20.00. Our code then clears `Lines`, and `Total` still reads 10.00. After step 3, `AddLine` moves `Total` from 10.00 to 20.00."

---

## Segment: 23–27 min: Step 4, private constructor

## Slide 15 — A Private Constructor

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain/Order.cs`, after Step 4: the constructor and `Place`. The `Place` signature is wrapped to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain/Order.cs, after Step 4

private Order() { }

public static Order Place(string customerName,
    IEnumerable<(int productId, int quantity, Money unitPrice)> lines)
{
    var order = new Order { CustomerName = customerName, OrderDate = DateTime.UtcNow };
    order.Status = OrderStatus.Placed;
    foreach (var (productId, quantity, unitPrice) in lines)
    {
        order.AddLine(productId, quantity, unitPrice);
    }
    order.RecalculateTotal();
    return order;
}
```

**Say:**
"After step 4, the `Order()` constructor is private, so `TalkApp` cannot call `new Order()`. The static `Place` method creates every `Order`, and `Place` sets `CustomerName`, `OrderDate`, `Status`, and the lines in one call."

---

## Slide 16 — Demo: An Empty Order

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Step 4: Order() becomes private
Option 4: An empty order
```

**Say:**
"Option 4 calls `new Order()` on the anemic model, and the printed order has an empty customer name, an empty status, and zero lines. After step 4, option 4 creates the order for Contoso through `Order.Place`, with `Status` set to `Placed`."

---

## Segment: 27–31 min: Step 5, guard clauses

## Slide 17 — Guard Clauses

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain`, after Step 5: the guards from `Place`, `AddLine`, and the `OrderLine` constructor. Each guard is wrapped after the condition to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain, after Step 5

// Order.Place
if (string.IsNullOrWhiteSpace(customerName))
    throw new ArgumentException("An order needs a customer name.", nameof(customerName));
if (!lines.Any())
    throw new ArgumentException("An order needs at least one line.", nameof(lines));

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
"After step 5, `Place` rejects a blank customer name and an empty line list. The `AddLine` method rejects a product that is already on the order, and the `OrderLine` constructor rejects a product id or a quantity of zero or less."

---

## Slide 18 — Demo: Bad Quantities and Duplicate Products

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Step 5: guards in Place, AddLine, and OrderLine
Option 5: Bad quantities and duplicate products
```

**Say:**
"Option 5 adds quantity -3, product 0, and product 2001 twice to the anemic `Order`, and every line is accepted. After step 5, option 5 prints a rejection message for each bad input, and the valid order for Contoso succeeds."

---

## Segment: 31–36 min: Step 6, value object

## Slide 19 — Money as a Value Object

**Layout:** Title Only layout. Code panel labeled `OrderModels/Domain`, after Step 6: the `Total` property and `RecalculateTotal` line from `Order.cs`, and the guards from `Money.cs`. Long lines are wrapped to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Domain, after Step 6

public Money Total { get; private set; } = null!;

Total = Lines.Select(l => l.LineTotal).Aggregate((sum, next) => sum.Add(next));

// Money.Create
if (amount < 0)
    throw new ArgumentException("Amount cannot be negative.", nameof(amount));

// Money.Add
if (other.Currency != Currency)
    throw new InvalidOperationException(
        $"Cannot add {other.Currency} to {Currency}: currency mismatch.");
```

**Say:**
"After step 6, `Total` has the type `Money`, and `RecalculateTotal` adds the line totals with `Money.Add`. The `Money.Create` method rejects a negative amount, and `Money.Add` throws an `InvalidOperationException` when the two currencies differ."

---

## Slide 20 — Mapping Money with OwnsOne

**Layout:** Title Only layout. Code panel labeled `TalkApp/Data/TalkDbContext.cs`, after Step 6.

**Text on slide (verbatim, shown all at once):**
```
TalkApp/Data/TalkDbContext.cs, after Step 6

order.OwnsOne(o => o.Total, money =>
{
    money.Property(m => m.Amount).HasColumnName("Total_Amount").HasPrecision(18, 2);
    money.Property(m => m.Currency).HasColumnName("Total_Currency").HasMaxLength(10);
});
```

**Say:**
"EF Core cannot build the model for a `Money` `Total` until `TalkDbContext` maps `Total` with `OwnsOne`. The `OwnsOne` call stores `Total` in two columns on the `DomainOrders` table, `Total_Amount` and `Total_Currency`."

---

## Slide 21 — Demo: Negative and Mixed-Currency Totals

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Step 6: Total becomes Money
Option 6: Negative and mixed-currency totals
```

**Say:**
"Option 6 sums 19.99 USD and 25.00 EUR into 44.99, and then sets the anemic `Total` to -42.50. After step 6, `Money.Create` rejects -42.50, the USD and EUR order is rejected, and the valid order total prints as 59.97 USD."

---

## Segment: 36–41 min: Step 7, EF Core persists the good model

## Slide 22 — Mapping the Good Model

**Layout:** Title Only layout. Code panel labeled `TalkApp/Data/TalkDbContext.cs`, after Step 7. The two fluent calls are wrapped one call per line to fit the panel.

**Text on slide (verbatim, shown all at once):**
```
TalkApp/Data/TalkDbContext.cs, after Step 7

order.Navigation(o => o.Lines)
    .HasField("_lines")
    .UsePropertyAccessMode(PropertyAccessMode.Field);

order.Property(o => o.Status)
    .HasConversion<string>()
    .HasMaxLength(20);
```

**Say:**
"After step 7, `HasField("_lines")` maps the `Lines` navigation to the `_lines` field, so EF Core reads and writes `_lines` directly. The `HasConversion<string>()` call stores `Status` as text, so the `Status` column holds `Confirmed` instead of the number 2."

---

## Slide 23 — Demo: Saving and Reloading Through EF Core

**Layout:** Template demo slide.

**Text on slide (verbatim, shown all at once):**
```
Demo
Step 7: HasField and HasConversion
Option 7: Saving and reloading through EF Core
```

**Say:**
"Option 7 saves the `"Banana"` order, the empty order, and the -42.50 order, and SQL Server stores all three anemic orders. After step 7, EF Core saves the confirmed Contoso order and reloads that order with two lines and a `Status` column of `Confirmed`."

---

## Segment: 41–45 min: Recap + Q&A

## Slide 24 — Seven Techniques, Seven Rules

**Layout:** Title Only layout. Seven-row table: step, technique, rule the technique protects.

**Text on slide (verbatim, revealed one row at a time):**
```
Step | Technique                          | Rule protected
1    | OrderStatus enum                   | Status is one of five values
2    | Private setters + domain methods   | Placed > Processing > Confirmed > Shipped
3    | _lines + IReadOnlyCollection       | Total equals the sum of the lines
4    | Private constructor                | Every order starts in Order.Place
5    | Guard clauses                      | A customer, lines, positive quantities, each product once
6    | Money value object                 | A non-negative total in one currency
7    | HasField + HasConversion           | The good model saves and reloads intact
```

**Say:**
"Each technique moves one rule from the caller into the `Order`. [Rows appear one at a time] Steps 1 to 6 each close one problem from the first demo, and step 7 maps the good model so EF Core saves and reloads the good model without an EF Core reference in `OrderModels/Domain`."

---

## Slide 25 — The Anemic and Domain Orders

**Layout:** Title Only layout. Two code panels stacked: `OrderModels/Anemic/Order.cs` on top, `OrderModels/Domain/Order.cs` after Step 7 below it (state only, methods omitted). Stacked rather than side by side so the code stays readable at 14pt or larger.

**Text on slide (verbatim, shown all at once):**
```
OrderModels/Anemic/Order.cs
public int Id { get; set; }
public string Status { get; set; } = string.Empty;
public decimal Total { get; set; }
public DateTime OrderDate { get; set; }
public string CustomerName { get; set; } = string.Empty;
public List<OrderLine> Lines { get; set; } = new();

OrderModels/Domain/Order.cs, after Step 7
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
"The anemic `Order` on top still accepts every invalid value from the first demo. The domain `Order` below the anemic `Order` has private setters, a read-only `Lines` collection, and a private constructor, and `OrderModels/Domain` has no EF Core reference."

---

## Slide 26 — Questions?

**Layout:** Template questions slide, unchanged.

**Text on slide (verbatim, shown all at once):**
```
(Template questions slide, unchanged)
```

**Say:**
"Thank you for your time. I am happy to take questions about the seven techniques, the EF Core mapping, or the demo code."

---

## Slide 27 — Conclusion

**Layout:** Template conclusion slide, unchanged, with its existing speaker note.

**Text on slide (verbatim, shown all at once):**
```
(Template conclusion slide, unchanged)
```

**Say:**
"Thank you for attending my talk today. Please reach out and connect."
