# Live Steps: TL;DR

**Rules for every step**
- Work **top to bottom**. Delete the `//TODO` line along with the line it names. Each line number is where the TODO sits when you reach that row, with the rows above it already done.
- `TalkDemo.cs` always goes last, then run the option. Each option shows **Bad** first. Press a key to show **Good**.
- **Finding the code:** every change sits under a comment that starts `//TODO: Talk Step N`. Press `Ctrl+Shift+F`, set the scope to Entire Solution, and search for the text in the Search row of that step.
- Lost? Use the Visual Studio Task List and filter it by `Talk Step N`.

| Slide | File | Line | Change | Problem this fixes (slide) | Option |
|---|---|---|---|---|---|
| 1–5 | Intro, no code | | | | |
| 6–7 | **Step 1 problem:** any caller can build any order. `new Order()` is empty, and `Status = "Banana"`, `Total = -99.99` saves and reloads through EF Core. | | | | |
| 8–10 | Fix: private constructor, `Order.Place`, `Place` guards | | | | |
| **11** | **Search:** `TODO: Talk Step 1` (3 TODOs in 2 files) | | | | |
| | `OrderModels/Domain/Order.cs` | 36 | `public Order()` → `private Order()` | Any caller can write `new Order { Status = "Banana", Total = -99.99m }`. (6, 7) |  |
| | `OrderModels/Domain/Order.cs` | 40 | `Place`: uncomment 2 guards | An order can have a blank customer and zero lines. (6, 7) |  |
| | `TalkApp/TalkDemo.cs` | 138 | `Solution1`: delete throw, uncomment code | Bad: the Banana order saves and reloads. Good: no public constructor, and `Place` rejects a blank customer and no lines. | **1** |
| 12 | **Step 2 problem:** every setter is public, so any caller can rewrite a placed order. | | | | |
| 13 | Fix: private setters. Slide 13 shows five properties; `Lines` gets one too. | | | | |
| **14** | **Search:** `TODO: Talk Step 2` (7 TODOs in 2 files) | | | | |
| | `OrderModels/Domain/Order.cs` | 5 | `Id` → `private set` | Any caller can change `Id`. (12) |  |
| | `OrderModels/Domain/Order.cs` | 7 | `Status` → `private set` | Any caller can set `Status` to anything. (12, 15) |  |
| | `OrderModels/Domain/Order.cs` | 9 | `Total` → `private set` | `order.Total = 0.01m` works on a placed order. (12) |  |
| | `OrderModels/Domain/Order.cs` | 14 | `OrderDate` → `private set` | `order.OrderDate = DateTime.Now.AddDays(-30)` moves the date back. (12) |  |
| | `OrderModels/Domain/Order.cs` | 16 | `CustomerName` → `private set` | `order.CustomerName = "Someone Else"` gives the order to another customer. (12) |  |
| | `OrderModels/Domain/Order.cs` | 18 | `Lines` → `private set` | `order.Lines = new()` replaces every line. Every setter is public. `Lines.Add` still works until Step 5. (12) | |
| | `TalkApp/TalkDemo.cs` | 215 | `Solution2`: delete throw, uncomment code | Bad: moved date, Someone Else, total 0.01. Good: all five setters are private, and `Place` still creates the order. | **2** |
| 15 | **Step 3 problem:** the anemic `Order` ships a Placed order and a Cancelled order. The domain `Order` now blocks the assignment, but nothing can move `Status` forward. | | | | |
| 16 | Fix: `Process`, `Confirm`, `Ship`, `Cancel` guard each transition | | | | |
| **17** | **Search:** `TODO: Talk Step 3` (5 TODOs in 2 files) | | | | |
| | `OrderModels/Domain/Order.cs` | 57 | `Process`: delete throw, uncomment 3 lines | With a private setter, nothing can move Placed to Processing. (15) |  |
| | `OrderModels/Domain/Order.cs` | 64 | `Confirm`: delete throw, uncomment 3 lines | With a private setter, nothing can move Processing to Confirmed. (15) |  |
| | `OrderModels/Domain/Order.cs` | 71 | `Ship`: delete throw, uncomment 3 lines | A Placed order that was never confirmed can ship, and so can a Cancelled order. (15) |  |
| | `OrderModels/Domain/Order.cs` | 78 | `Cancel`: delete throw, uncomment 3 lines | Nothing stops a shipped order from being cancelled. (16) |  |
| | `TalkApp/TalkDemo.cs` | 259 | `Solution3`: delete throw, uncomment code | Bad: Placed and Cancelled orders both print Shipped. Good: `Ship` throws for both, and `Process`, `Confirm`, `Ship` reaches Shipped. | **3** |
| 18 | **Step 4 problem:** a decimal `Total` has no currency. 19.99 USD plus 25.00 EUR gives 44.99. Dollars? Euros? | | | | |
| 19 | **Step 4 problem:** `Money` checks nothing. `Money.Create` accepts -42.50, and USD plus EUR returns 44.99 USD. | | | | |
| 20–22 | Fix: `Money` total, `OwnsOne`, `Money` guards | | | | |
| **23** | **Search:** `TODO: Talk Step 4` (6 TODOs in 4 files) | | | | |
| | `OrderModels/Domain/Money.cs` | 16 | `Create`: uncomment 2 guards | `Money.Create(-42.50m, "USD")` is accepted, and so is a blank currency. (19, 22) |  |
| | `OrderModels/Domain/Money.cs` | 24 | `Add`: uncomment currency guard | 19.99 USD plus 25.00 EUR returns 44.99 USD. (19) |  |
| | `OrderModels/Domain/Order.cs` | 11 | Delete `decimal Total` above the TODO, uncomment `Money Total` | A decimal `Total` has no currency. (18) |  |
| | `OrderModels/Domain/Order.cs` | 83 | `RecalculateTotal`: `Sum` → `Aggregate(... Add)` (uncomment 3 lines) | `Sum` adds bare decimals, so USD and EUR mix without an error. (18) |  |
| | `TalkApp/Data/TalkDbContext.cs` | 42 | `HasPrecision` → `OwnsOne` (uncomment 5 lines) | The `Total` column in SQL Server holds a bare number with no currency. (18, 21) |  |
| | `TalkApp/TalkDemo.cs` | 321 | `Solution4`: delete throw, uncomment code | Bad: 44.99 with no currency, -42.50 accepted, USD plus EUR accepted. Good: 59.97 USD reloads, and the bad cases are rejected. | **4** |
| 24 | **Step 5 problem:** any caller can change `Lines`, and nothing recalculates `Total`. Lines sum to 20.00 with a total of 10.00. Zero lines still shows 10.00. | | | | |
| 25–26 | Fix: `_lines` backing field, read-only `Lines`, `AddLine`, internal constructor, `HasField` | | | | |
| 27 | **Step 5 problem:** nothing checks the lines. Quantity -3, product 0, and product 2001 twice are all accepted. | | | | |
| 28 | Fix: guards in `AddLine` and the `OrderLine` constructor | | | | |
| **29** | **Search:** `TODO: Talk Step 5` (8 TODOs in 4 files) | | | | |
| | `OrderModels/Domain/Order.cs` | 18 | Delete `List<OrderLine> Lines` above the TODO, uncomment `_lines` + `IReadOnlyCollection` (2 lines) | Any caller can call `Lines.Add` or `Lines.Clear`. (24) |  |
| | `OrderModels/Domain/Order.cs` | 31 | `Place`: `Lines.Add` → `AddLine` | `Place` adds lines directly, so its lines would skip the rules in `AddLine`. (25) |  |
| | `OrderModels/Domain/Order.cs` | 40 | `AddLine`: uncomment duplicate-product guard | Product 2001 can go on the same order twice. (27) |  |
| | `OrderModels/Domain/Order.cs` | 42 | `AddLine`: delete throw, uncomment 2 lines | Adding a line leaves `Total` stale: lines sum to 20.00, total says 10.00. (24) |  |
| | `OrderModels/Domain/OrderLine.cs` | 16 | `public` → `internal` constructor | Any caller can create a line with `new OrderLine(...)`. (24) |  |
| | `OrderModels/Domain/OrderLine.cs` | 18 | Constructor: uncomment 2 guards | A quantity of -3 and a product id of 0 are accepted. (27) |  |
| | `TalkApp/Data/TalkDbContext.cs` | 50 | Uncomment `HasField("_lines")` | `Lines` has no setter, and finding `_lines` by name breaks if the field is renamed. (26) |  |
| | `TalkApp/TalkDemo.cs` | 401 | `Solution5`: delete throw, uncomment code | Bad: total 10.00 with lines summing to 20.00, then zero lines, and all three bad lines accepted. Good: 10.00 USD becomes 20.00 USD, 2 lines reload, and the bad lines are rejected. | **5** |
| 30–33 | Recap table, side-by-side models, questions, close | | | | |
