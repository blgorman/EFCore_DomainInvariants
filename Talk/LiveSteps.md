# Live Steps: TL;DR

**Rules for every step**
- Delete the `//TODO` line along with the line it names. The line numbers below assume you do.
- Each line number is where the TODO sits **when that step starts**. Within one file, work **bottom to top**, so the numbers above stay valid. The rows are already in that order.
- `TalkDemo.cs` always goes last, then run the option. Each option shows **Bad** first. Press a key to show **Good**.
- Lost? Use the Visual Studio Task List and filter it by `Talk Step N`.

| Slide | File | Line | Change | Option |
|---|---|---|---|---|
| 1–5 | | | Intro, no code | |
| 6–10 | | | Talk: `new Order()` and `Banana` problems, private constructor, `Order.Place`, `Place` guards | |
| **11** | `OrderModels/Domain/Order.cs` | 38 | `Place`: uncomment 2 guards | |
| | `OrderModels/Domain/Order.cs` | 32 | `public Order()` → `private Order()` | |
| | `TalkApp/TalkDemo.cs` | 129 | `Solution1`: delete throw, uncomment code | **1** |
| 12–13 | | | Talk: private setters problem, the five setters | |
| **14** | `OrderModels/Domain/Order.cs` | 23 | `CustomerName` → `private set` | |
| | `OrderModels/Domain/Order.cs` | 19 | `OrderDate` → `private set` | |
| | `OrderModels/Domain/Order.cs` | 13 | `Total` → `private set` (uncomment 3 lines; leaves a Step 4 TODO) | |
| | `OrderModels/Domain/Order.cs` | 9 | `Status` → `private set` | |
| | `OrderModels/Domain/Order.cs` | 5 | `Id` → `private set` | |
| | `TalkApp/TalkDemo.cs` | 196 | `Solution2`: delete throw, uncomment code | **2** |
| 15–16 | | | Talk: status jumps problem, `Ship`/`Cancel` | |
| **17** | `OrderModels/Domain/Order.cs` | 82 | `Cancel`: delete throw, uncomment 3 lines | |
| | `OrderModels/Domain/Order.cs` | 73 | `Ship`: delete throw, uncomment 3 lines | |
| | `OrderModels/Domain/Order.cs` | 64 | `Confirm`: delete throw, uncomment 3 lines | |
| | `OrderModels/Domain/Order.cs` | 55 | `Process`: delete throw, uncomment 3 lines | |
| | `TalkApp/TalkDemo.cs` | 234 | `Solution3`: delete throw, uncomment code | **3** |
| 18–22 | | | Talk: decimal total and `Money` problems, `Money` total, `OwnsOne`, `Money` guards | |
| **23** | `OrderModels/Domain/Money.cs` | 25 | `Add`: uncomment currency guard | |
| | `OrderModels/Domain/Money.cs` | 16 | `Create`: uncomment 2 guards | |
| | `OrderModels/Domain/Order.cs` | 83 | `RecalculateTotal`: `Sum` → `Aggregate(... Add)` | |
| | `OrderModels/Domain/Order.cs` | 9 | `decimal Total` → `Money Total` | |
| | `TalkApp/Data/TalkDbContext.cs` | 42 | `HasPrecision` → `OwnsOne` (uncomment 5 lines) | |
| | `TalkApp/TalkDemo.cs` | 288 | `Solution4`: delete throw, uncomment code | **4** |
| 24–28 | | | Talk: backing field and bad line problems, `_lines`, `HasField`, line guards | |
| **29** | `OrderModels/Domain/Order.cs` | 45 | `AddLine`: delete throw, uncomment 2 lines | |
| | `OrderModels/Domain/Order.cs` | 42 | `AddLine`: uncomment duplicate-product guard | |
| | `OrderModels/Domain/Order.cs` | 31 | `Place`: `Lines.Add` → `AddLine` | |
| | `OrderModels/Domain/Order.cs` | 15 | `List<OrderLine> Lines` → `_lines` + `IReadOnlyCollection` (2 lines) | |
| | `OrderModels/Domain/OrderLine.cs` | 20 | Constructor: uncomment 2 guards | |
| | `OrderModels/Domain/OrderLine.cs` | 16 | `public` → `internal` constructor | |
| | `TalkApp/Data/TalkDbContext.cs` | 50 | Uncomment `HasField("_lines")` | |
| | `TalkApp/TalkDemo.cs` | 357 | `Solution5`: delete throw, uncomment code | **5** |
| 30–33 | | | Recap table, side-by-side models, questions, close | |
