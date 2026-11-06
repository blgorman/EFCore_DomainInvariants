# Talk Plan: Creating Domain Invariants

- **Date:** 7 November 2026
- **Length:** 45 minutes
- **Repo:** `EFCore_DomainInvariants` (standalone; nothing on stage comes from `EFCore_DomainDrivenDesign`)

---

## 1. The Short Version

- **Two backing models.** The bad model (`OrderModels/Anemic`) is the anemic `Order` from Module 1 Clip 2 and never changes. The good model (`OrderModels/Domain`) starts as a copy of the bad model, except that `Status` is already the `OrderStatus` enum (as in the Module 2 starter), and is the code modified during the talk.
- **The steps follow the build order.** Five steps cover seven techniques: close the way in (private constructor, factory method, `Order.Place` guards), lock the properties (private setters), add the behavior that requires (status domain methods), fix `Money` (value object, `OwnsOne`, `Money` guards), then fix the lines that depend on `Money` (backing field, `AddLine`, line guards). Each guard clause goes in with the code it protects. Brian chose this order over the course's clip order on 2026-11-05 because each step solves a real problem and sets up the next.
- **One demo file: `TalkApp/TalkDemo.cs`.** It holds the talk menu and every call. Each menu option calls the bad model to show one problem, then calls the good model to show the mapped solution.
- **Every menu option runs from step 0.** Before step N, option N shows the problem, then a "Not Yet Implemented" box.
- **On stage:** apply every `//TODO: Talk Step N`, run option N, show the problem on the bad model and the solution on the good model.

---

## 2. Solution Layout

| Project | Contents |
|---|---|
| `OrderModels/Anemic` | The bad model: `Order`, `OrderLine`, `Money`. No TODOs. Never edited. |
| `OrderModels/Domain` | The good model: `Order`, `OrderLine`, `Money`, `OrderStatus`. `OrderStatus` exists at step 0. Every change is pre-staged under a `//TODO: Talk Step N`. |
| `TalkApp/TalkDemo.cs` | The menu, five problem methods (bad model), five solution methods (good model). |
| `TalkApp/Data/TalkDbContext.cs` | Maps both models to separate tables (`AnemicOrders`, `DomainOrders`). `HasConversion<string>()` on `Status` is live at step 0. Step 4 adds `OwnsOne` for `Total`; step 5 adds `HasField("_lines")`. |
| `TalkApp/Program.cs` | Drops and recreates the database on every run (`EnsureDeletedAsync` + `EnsureCreatedAsync`), then starts the menu. No migrations. |
| `ConsoleHelpers` | `OutputHelpers` and `InputHelpers`, copied from the course. |

The good model project has no EF Core reference.

---

## 3. The Bad Model (never changes)

`OrderModels/Anemic/Order.cs`:

```csharp
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

`Money` and `OrderLine` are pre-built the same way the Module 2 starter ships them: `OrderLine.UnitPrice` is already a `Money`, the `OrderLine` constructor is public, and neither class has guard clauses.

---

## 4. The Good Model (starts as a copy, modified live)

### 4.1 How every change is staged

- **Replace a line:** the bad code stays live. A `//TODO: Talk Step N — Delete the next line and uncomment the line below it` sits directly above the live line; the good code sits commented out directly below it. At step N: delete the live line, uncomment the good code.
- **Add a guard clause:** `//TODO: Talk Step N — Uncomment the ... guard clause(s) below`, followed by the commented guard.
- **A line that changes twice** (`Total` at steps 2 and 4) carries the later step's TODO double-commented (`////TODO`) inside the earlier step's block. Uncommenting the earlier block turns the later TODO into a normal `//TODO`.
- **Members stubbed with their final signature at step 0:** `AddLine`, `Process`, `Confirm`, `Ship`, `Cancel`. Each body is `throw new NotImplementedException();` under its TODO, with the real body commented below.
- **No signature is typed live.** Every change is deleting a line and uncommenting the line(s) below it.

### 4.2 `Order.Place` is live from step 0

`Order.Place(string customerName, IEnumerable<(int productId, int quantity, Money unitPrice)> lines)` works at step 0, the same as the Module 2 starter, and every solution creates its order through `Place`. Step 1 makes the constructor private and adds the guards for a blank customer and no lines, so `Place` is the only way in from the start. The body of `Place` changes again at step 5, when it calls `AddLine`.

### 4.3 The solution method in `TalkDemo`

Each solution method starts with `throw new NotImplementedException();` under `//TODO: Talk Step N — Delete the next line and uncomment the code below it`. The menu catches the `NotImplementedException` and shows the "Not Yet Implemented" box. The TalkDemo TODO is always the last one applied for a step.

---

## 5. Problem → Solution Map

Each step solves one problem and finishes the code it touches, guards included. Each step opens with its problem on the bad model, then applies the technique, then runs the option. Menu titles name the concept and its solution.

| Option (menu title) | Problem (bad model) | `//TODO: Talk Step N` edits | Solution (good model) |
|---|---|---|---|
| 1. Private constructor and `Order.Place` | `new Order()` is empty; `new Order { Status = "Banana", Total = -99.99m }` has no customer, date, or lines, and EF Core saves and reloads it | The `Order` constructor becomes private; guards in `Place` (blank customer, no lines) | No public `Order()`; `Order.Place("Contoso", two lines)` reloads through the private constructor with total 44.98; a blank customer and no lines are rejected; the line's `OrderId` is a shadow foreign key |
| 2. Private setters | Caller rewrites `OrderDate`, `CustomerName`, and `Total` on a placed order | Private setters on `Id`, `Status`, `Total`, `OrderDate`, `CustomerName` | Every setter reports private; `Order.Place` still sets them inside `Order` |
| 3. Domain methods control `Status` | `Status = "Shipped"` on a Placed order and on a Cancelled order | Bodies of `Process`, `Confirm`, `Ship`, `Cancel` | `Ship()` while Placed and while Cancelled is rejected; `Process` → `Confirm` → `Ship` reaches Shipped |
| 4. `Total` as a `Money` value object | 19.99 USD + 25.00 EUR summed into a decimal `Total` of 44.99; `Money.Create(-42.50m)` and USD + EUR accepted | `Total` becomes `Money`; `RecalculateTotal` sums `Money`; `TalkDbContext`: `OwnsOne` for `Total`; guards in `Money.Create` (negative, blank currency) and `Money.Add` (currency mismatch) | `Total` is `59.97 USD`, reloads as `59.97 USD`; SQL Server holds `Total_Amount` and `Total_Currency`; a negative amount, USD + EUR, and USD and EUR lines are rejected |
| 5. Backing field and read-only `Lines` | `Lines.Add` leaves `Total` stale; `Lines.Clear` empties the order; quantity -3, product 0, and product 2001 twice are accepted | `_lines` + `IReadOnlyCollection<OrderLine> Lines`; `AddLine` body; `Place` calls `AddLine`; `OrderLine` constructor becomes `internal`; `TalkDbContext`: `HasField("_lines")`; guards in `AddLine` (duplicate product) and the `OrderLine` constructor (product id, quantity) | `AddLine` moves `Total` from 10.00 USD to 20.00 USD; EF Core saves and reloads 2 lines into `_lines`; each bad line is rejected |

`HasField("_lines")` is not required when the field name follows the `_lines` convention (verified 2026-11-04: EF Core reloads the lines without it). The talk keeps it because the mapping must be explicit whenever the field name diverges from the convention.

Step 4 includes the `OwnsOne` mapping for `Total` because the app can't build its EF Core model once `Total` is a `Money` without it. Each line gets its own `Money` instance: EF Core tracks an owned instance on one owner only, and a shared `Money` saves as NULL on the second line.

The shadow foreign key is not a step. Solution 1 shows the one the talk already has: `OrderLine` has no `OrderId` property, and `TalkDbContext` maps it with `HasForeignKey("OrderId")`.

---

## 6. The Timeline

| Time | Segment |
|---|---|
| 0–5 min | Slides: the anemic `Order`, the desired `Order` invariants |
| 5–13 min | Step 1: private constructor, factory method, `Order.Place` guards |
| 13–17 min | Step 2: private setters |
| 17–22 min | Step 3: domain methods for `Status` |
| 22–30 min | Step 4: `Money` value object and its guards |
| 30–38 min | Step 5: backing field, read-only collection, line guards |
| 38–45 min | Recap + Q&A: the bad model next to the good model |

---

## 7. Decisions

- **Two backing models, bad and good.** The bad model never changes.
- **One demo file, `TalkDemo.cs`, holds the menu and every call.** No caller classes.
- **Keep `CustomerName` as a string.** No `Customer` entity and no shadow `CustomerId`.
- **Tag each step** (`v1.0` through `v1.5`) so you can recover with `git checkout` if a step goes wrong on stage.
- **Follow the build order, not the clip order.** No enum step: `OrderStatus` exists at step 0, as in the course starter.
- **Database:** `TalkApp/Program.cs` loads user secrets over `appsettings.json`; on the Surface (ARM64) the secret points at the `sqledge` Docker container. LocalDB is x64 only.

---

## 8. Verified

Built and run on 2026-11-06 with `tools/verify_steps.sh`, plus all five options run after each step:
- Step 0 and each of steps 1–5: 0 compile errors.
- At step 0 every option prints its "Bad:" box and then the "Not Yet Implemented" box.
- After step N, options 1 to N print their "Good:" boxes and options N+1 to 5 print "Not Yet Implemented"; no crashes, no box overflow.
- `System.Security.Cryptography.Pkcs` is pinned to 10.0.14 in `TalkApp/TalkApp.csproj`; no vulnerable packages.

---

## 9. Next Action

- Commit step 0 and rebuild tags `v1.0` through `v1.5` on the new order, after Brian approves the staged work.
