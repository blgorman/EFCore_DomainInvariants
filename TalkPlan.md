# Talk Plan: Creating Domain Invariants

- **Date:** 7 November 2026
- **Length:** 45 minutes
- **Repo:** `EFCore_DomainInvariants` (standalone; nothing on stage comes from `EFCore_DomainDrivenDesign`)

---

## 1. The Short Version

- **Two backing models.** The bad model (`OrderModels/Anemic`) is the anemic `Order` from Module 1 Clip 2 and never changes. The good model (`OrderModels/Domain`) starts as a copy of the bad model, except that `Status` is already the `OrderStatus` enum (as in the Module 2 starter), and is the code modified during the talk.
- **The steps follow the course.** Steps 1 to 7 follow Module 2 in order: Clip 1 (factory method), Clip 2 (backing field), Clip 3 (private setters, then the status domain methods), Clip 4 (`Money`), Clip 7 (private constructor and materialization), Clip 8 (guard clauses). Clips 5, 6, and 9 are not steps.
- **One demo file: `TalkApp/TalkDemo.cs`.** It holds the talk menu and every call. Each menu option calls the bad model to show one problem, then calls the good model to show the mapped solution.
- **Every menu option runs from step 0.** Before step N, option N shows the problem, then a "Not Yet Implemented" box.
- **On stage:** apply every `//TODO: Talk Step N`, run option N, show the problem on the bad model and the solution on the good model.

---

## 2. Solution Layout

| Project | Contents |
|---|---|
| `OrderModels/Anemic` | The bad model: `Order`, `OrderLine`, `Money`. No TODOs. Never edited. |
| `OrderModels/Domain` | The good model: `Order`, `OrderLine`, `Money`, `OrderStatus`. `OrderStatus` exists at step 0. Every change is pre-staged under a `//TODO: Talk Step N`. |
| `TalkApp/TalkDemo.cs` | The menu, seven problem methods (bad model), seven solution methods (good model). |
| `TalkApp/Data/TalkDbContext.cs` | Maps both models to separate tables (`AnemicOrders`, `DomainOrders`). `HasConversion<string>()` on `Status` is live at step 0. Step 2 adds `HasField("_lines")`; step 5 adds `OwnsOne` for `Total`. |
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
- **A line that changes twice** (`Total` at steps 3 and 5) carries the later step's TODO double-commented (`////TODO`) inside the earlier step's block. Uncommenting the earlier block turns the later TODO into a normal `//TODO`.
- **Members stubbed with their final signature at step 0:** `AddLine`, `Process`, `Confirm`, `Ship`, `Cancel`. Each body is `throw new NotImplementedException();` under its TODO, with the real body commented below.
- **No signature is typed live.** Every change is deleting a line and uncommenting the line(s) below it.

### 4.2 `Order.Place` is live from step 0

`Order.Place(string customerName, IEnumerable<(int productId, int quantity, Money unitPrice)> lines)` works at step 0, the same as the Module 2 starter. Step 1 is the course's Clip 1: no domain code changes, and the demo contrasts the anemic `new Order { ... }` with `Order.Place`. Every later solution creates its order through `Place`. The body of `Place` follows the model through steps 2 and 7 via its own TODOs.

### 4.3 The solution method in `TalkDemo`

Each solution method starts with `throw new NotImplementedException();` under `//TODO: Talk Step N — Delete the next line and uncomment the code below it`. The menu catches the `NotImplementedException` and shows the "Not Yet Implemented" box. The TalkDemo TODO is always the last one applied for a step.

---

## 5. Problem → Solution Map

One concept per step, in the course's order. Each step opens with its problem on the bad model, then applies the technique, then runs the option. Menu titles name the concept and its solution.

| Option (menu title) | Course | Problem (bad model) | `//TODO: Talk Step N` edits | Solution (good model) |
|---|---|---|---|---|
| 1. Factory method: `Order.Place` | M2 Clip 1 | `new Order { Status = "Banana", Total = -99.99m }` with no customer, date, or lines; EF Core saves and reloads it | `TalkDemo` only | `Order.Place("Contoso", two lines)` sets customer, `Placed`, date, 2 lines, total 44.98 |
| 2. Backing field and read-only `Lines` | M2 Clip 2 | `Lines.Add` leaves `Total` stale; `Lines.Clear` empties the order | `_lines` + `IReadOnlyCollection<OrderLine> Lines`; `AddLine` body; `Place` calls `AddLine`; `OrderLine` constructor becomes `internal`; `TalkDbContext`: `HasField("_lines")` | `AddLine` moves `Total` from 10.00 to 20.00; EF Core saves and reloads 2 lines into `_lines` |
| 3. Private setters | M2 Clip 3 | Caller rewrites `OrderDate`, `CustomerName`, and `Total` on a placed order | Private setters on `Id`, `Status`, `Total`, `OrderDate`, `CustomerName` | Every setter reports private; `Order.Place` still sets them inside `Order` |
| 4. Domain methods control `Status` | M2 Clip 3 | `Status = "Shipped"` on a Placed order and on a Cancelled order | Bodies of `Process`, `Confirm`, `Ship`, `Cancel` | `Ship()` while Placed and while Cancelled is rejected; `Process` → `Confirm` → `Ship` reaches Shipped |
| 5. `Total` as a `Money` value object | M2 Clip 4 | 19.99 USD + 25.00 EUR summed into a decimal `Total` of 44.99 | `Total` becomes `Money`; `RecalculateTotal` sums `Money`; `TalkDbContext`: `OwnsOne` for `Total` | `Total` is `59.97 USD`, reloads as `59.97 USD`; SQL Server holds `Total_Amount` and `Total_Currency` |
| 6. Private constructor and EF materialization | M2 Clip 7 | `new Order()` with no customer, status, date, or lines | The `Order` constructor becomes private | No public `Order()`; EF Core reloads the order through the private constructor; the line's `OrderId` is a shadow foreign key |
| 7. Guard clauses | M2 Clip 8 | Quantity -3, product 0, product 2001 twice, blank customer, `Money.Create(-42.50m)`, USD + EUR: all accepted | Guards in `Place` (blank customer, no lines), `AddLine` (duplicate product), `OrderLine` constructor (product id, quantity), `Money.Create` (negative, blank currency), `Money.Add` (currency mismatch) | Each bad input is rejected with a readable message; a valid order succeeds |

`HasField("_lines")` is not required when the field name follows the `_lines` convention (verified 2026-11-04: EF Core reloads the lines without it). The talk keeps it because the mapping must be explicit whenever the field name diverges from the convention.

Step 5 includes the `OwnsOne` mapping for `Total` because the app can't build its EF Core model once `Total` is a `Money` without it. Each line gets its own `Money` instance: EF Core tracks an owned instance on one owner only, and a shared `Money` saves as NULL on the second line.

The shadow foreign key (course Clip 5) is not a step. Solution 6 shows the one the talk already has: `OrderLine` has no `OrderId` property, and `TalkDbContext` maps it with `HasForeignKey("OrderId")`.

---

## 6. The Timeline

| Time | Segment |
|---|---|
| 0–5 min | Slides: the anemic `Order`, the `Order` invariants, the two models |
| 5–9 min | Step 1: factory method |
| 9–14 min | Step 2: backing field + read-only collection |
| 14–18 min | Step 3: private setters |
| 18–23 min | Step 4: domain methods for `Status` |
| 23–28 min | Step 5: `Money` value object |
| 28–33 min | Step 6: private constructor + EF materialization |
| 33–38 min | Step 7: guard clauses |
| 38–45 min | Recap + Q&A: the bad model next to the good model |

---

## 7. Decisions

- **Two backing models, bad and good.** The bad model never changes.
- **One demo file, `TalkDemo.cs`, holds the menu and every call.** No caller classes.
- **Keep `CustomerName` as a string.** No `Customer` entity and no shadow `CustomerId`.
- **Tag each step** (`talk-step-0` through `talk-step-7`) so you can recover with `git checkout` if a step goes wrong on stage.
- **Follow the course's Module 2 order.** No enum step: `OrderStatus` exists at step 0, as in the course starter.
- **Database:** `TalkApp/Program.cs` loads user secrets over `appsettings.json`; on the Surface (ARM64) the secret points at the `sqledge` Docker container. LocalDB is x64 only.

---

## 8. Verified

Built and run on 2026-11-05 with `tools/verify_steps.sh`, plus all seven options run after each step:
- Step 0 and each of steps 1–7: 0 compile errors.
- At step 0 every option prints its "Bad:" box and then the "Not Yet Implemented" box.
- After step N, options 1 to N print their "Good:" boxes and options N+1 to 7 print "Not Yet Implemented"; no crashes, no box overflow.
- `System.Security.Cryptography.Pkcs` is pinned to 10.0.14 in `TalkApp/TalkApp.csproj`; no vulnerable packages.

---

## 9. Next Action

- Commit step 0 and tag it `talk-step-0`.
- Write the slides from this repo.
