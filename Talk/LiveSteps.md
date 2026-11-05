# Live Steps: TL;DR

**Rules for every step**
- Delete the `//TODO` line along with the line it names. The line numbers below assume you do.
- Each line number is where the TODO sits **when that step starts**. Within one file, work **bottom to top**, so the numbers above stay valid. The rows are already in that order.
- `TalkDemo.cs` always goes last, then run the option. Each option shows **Bad** first. Press a key to show **Good**.
- Lost? Use the Visual Studio Task List and filter it by `Talk Step N`.

| Slide | File | Line | Change | Option |
|---|---|---|---|---|
| 1–6 | | | Intro, no code | |
| 7–8 | | | Talk: factory method problem, `Order.Place` | |
| **9** | `TalkApp/TalkDemo.cs` | 132 | `Solution1`: delete throw, uncomment code | **1** |
| 10–12 | | | Talk: backing field problem, `_lines`, `HasField` | |
| **13** | `OrderModels/Domain/Order.cs` | 60 | `AddLine`: delete throw, uncomment 2 lines | |
| | `OrderModels/Domain/Order.cs` | 46 | `Place`: `Lines.Add` → `AddLine` | |
| | `OrderModels/Domain/Order.cs` | 27 | `List<OrderLine> Lines` → `_lines` + `IReadOnlyCollection` (2 lines) | |
| | `OrderModels/Domain/OrderLine.cs` | 16 | `public` → `internal` constructor | |
| | `TalkApp/Data/TalkDbContext.cs` | 52 | Uncomment `HasField("_lines")` | |
| | `TalkApp/TalkDemo.cs` | 184 | `Solution2`: delete throw, uncomment code | **2** |
| 14–15 | | | Talk: private setters problem, the five setters | |
| **16** | `OrderModels/Domain/Order.cs` | 23 | `CustomerName` → `private set` | |
| | `OrderModels/Domain/Order.cs` | 19 | `OrderDate` → `private set` | |
| | `OrderModels/Domain/Order.cs` | 13 | `Total` → `private set` (uncomment 3 lines; leaves a Step 5 TODO) | |
| | `OrderModels/Domain/Order.cs` | 9 | `Status` → `private set` | |
| | `OrderModels/Domain/Order.cs` | 5 | `Id` → `private set` | |
| | `TalkApp/TalkDemo.cs` | 241 | `Solution3`: delete throw, uncomment code | **3** |
| 17–18 | | | Talk: status jumps problem, `Ship`/`Cancel` | |
| **19** | `OrderModels/Domain/Order.cs` | 79 | `Cancel`: delete throw, uncomment 3 lines | |
| | `OrderModels/Domain/Order.cs` | 70 | `Ship`: delete throw, uncomment 3 lines | |
| | `OrderModels/Domain/Order.cs` | 61 | `Confirm`: delete throw, uncomment 3 lines | |
| | `OrderModels/Domain/Order.cs` | 52 | `Process`: delete throw, uncomment 3 lines | |
| | `TalkApp/TalkDemo.cs` | 279 | `Solution4`: delete throw, uncomment code | **4** |
| 20–22 | | | Talk: decimal total problem, `Money` total, `OwnsOne` | |
| **23** | `OrderModels/Domain/Order.cs` | 80 | `RecalculateTotal`: `Sum` → `Aggregate(... Add)` | |
| | `OrderModels/Domain/Order.cs` | 9 | `decimal Total` → `Money Total` | |
| | `TalkApp/Data/TalkDbContext.cs` | 42 | `HasPrecision` → `OwnsOne` (uncomment 5 lines) | |
| | `TalkApp/TalkDemo.cs` | 327 | `Solution5`: delete throw, uncomment code | **5** |
| 24–25 | | | Talk: `new Order()` problem, private constructor + shadow FK | |
| **26** | `OrderModels/Domain/Order.cs` | 18 | `public Order()` → `private Order()` | |
| | `TalkApp/TalkDemo.cs` | 381 | `Solution6`: delete throw, uncomment code | **6** |
| 27–29 | | | Talk: bad input problem, `Order`/`OrderLine` guards, `Money` guards | |
| **30** | `OrderModels/Domain/Money.cs` | 25 | `Add`: uncomment currency guard | |
| | `OrderModels/Domain/Money.cs` | 16 | `Create`: uncomment 2 guards | |
| | `OrderModels/Domain/Order.cs` | 39 | `AddLine`: uncomment duplicate-product guard | |
| | `OrderModels/Domain/Order.cs` | 22 | `Place`: uncomment 2 guards | |
| | `OrderModels/Domain/OrderLine.cs` | 18 | Constructor: uncomment 2 guards | |
| | `TalkApp/TalkDemo.cs` | 439 | `Solution7`: delete throw, uncomment code | **7** |
| 31–34 | | | Recap table, side-by-side models, questions, close | |
