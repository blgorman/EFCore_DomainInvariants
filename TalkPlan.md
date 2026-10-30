# Talk Plan: Creating Domain Invariants

- **Date:** 7 November 2026
- **Length:** 45 minutes
- **Repo:** `EFCore_DomainInvariants` (standalone; nothing on stage comes from `EFCore_DomainDrivenDesign`)

---

## 1. The Short Version

- **Two backing models.** The bad model (`OrderModels/Anemic`) is the anemic `Order` from Module 1 Clip 2 and never changes. The good model (`OrderModels/Domain`) starts as a copy of the bad model and is the code modified during the talk.
- **One demo file: `TalkApp/TalkDemo.cs`.** It holds the talk menu and every call. Each menu option calls the bad model to show one problem, then calls the good model to show the mapped solution.
- **Every menu option runs from step 0.** Before step N, option N shows the problem, then a "Not Yet Implemented" box.
- **On stage:** apply every `//TODO: Talk Step N`, run option N, show the problem on the bad model and the solution on the good model.

---

## 2. Solution Layout

| Project | Contents |
|---|---|
| `OrderModels/Anemic` | The bad model: `Order`, `OrderLine`, `Money`. No TODOs. Never edited. |
| `OrderModels/Domain` | The good model: `Order`, `OrderLine`, `Money`, `OrderStatus`. Every change is pre-staged under a `//TODO: Talk Step N`. |
| `TalkApp/TalkDemo.cs` | The menu, seven problem methods (bad model), seven solution methods (good model). |
| `TalkApp/Data/TalkDbContext.cs` | Maps both models to separate tables (`AnemicOrders`, `DomainOrders`). Steps 6 and 7 edit this file. |
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
- **A line that changes twice** (`Status` at steps 1 and 2, `Total` at steps 2 and 6) carries the later step's TODO double-commented (`////TODO`) inside the earlier step's block. Uncommenting the earlier block turns the later TODO into a normal `//TODO`.
- **Members stubbed with their final signature at step 0:** `AddLine`, `Process`, `Confirm`, `Ship`, `Cancel`. Each body is `throw new NotImplementedException();` under its TODO, with the real body commented below.
- **No signature is typed live.** Every change is deleting a line and uncommenting the line(s) below it.

### 4.2 `Order.Place` is live from step 0

`Order.Place(string customerName, IEnumerable<(int productId, int quantity, Money unitPrice)> lines)` works at step 0, the same as the Module 2 starter. Solutions 2 and 3 need an `Order` to call `Ship()` and `AddLine` on, and `TalkDemo` changes only by uncommenting, so `Place` has to work before step 4. The body of `Place` follows the model through steps 1, 3, and 5 via its own TODOs.

### 4.3 The solution method in `TalkDemo`

Each solution method starts with `throw new NotImplementedException();` under `//TODO: Talk Step N — Delete the next line and uncomment the code below it`. The menu catches the `NotImplementedException` and shows the "Not Yet Implemented" box. The TalkDemo TODO is always the last one applied for a step.

---

## 5. Problem → Solution Map

| Option | Problem (bad model) | `//TODO: Talk Step N` edits | Solution (good model) |
|---|---|---|---|
| 1 | `Status = "Shiped"` and `"Banana"` accepted | `Status` becomes `OrderStatus`; `Place` sets `OrderStatus.Placed` | `Status` property type is `OrderStatus`; `"Shiped"` and `"Banana"` are not values |
| 2 | `Status = "Shipped"` on a Placed order; `OrderDate` and `CustomerName` rewritten after shipping | Private setters on `Id`, `Status`, `Total`, `OrderDate`, `CustomerName`; bodies of `Process`, `Confirm`, `Ship`, `Cancel` | `Ship()` on a Placed order is rejected; `Process` → `Confirm` → `Ship` reaches Shipped |
| 3 | `Lines.Add` leaves `Total` stale; `Lines.Clear` empties the order | `_lines` + `IReadOnlyCollection<OrderLine> Lines`; `AddLine` body; `Place` calls `AddLine`; `OrderLine` constructor becomes `internal` | `AddLine` moves `Total` from 10.00 to 20.00 |
| 4 | `new Order()` with no customer, status, date, or lines | `Order()` becomes private | `Order.Place` is the only way to create an order |
| 5 | Quantity -3, product 0, product 2001 twice, all accepted | Guards in `Place` (blank customer, no lines), `AddLine` (duplicate product), `OrderLine` constructor (product id, quantity) | Each bad input is rejected with a readable message; a valid order succeeds |
| 6 | USD + EUR summed into one decimal; `Total = -42.50m` | `Total` becomes `Money`; `RecalculateTotal` sums `Money`; guards in `Money.Create` and `Money.Add`; `TalkDbContext` maps `Total` with `OwnsOne` | Negative amount and mixed currency are rejected; a valid total is `59.97 USD` |
| 7 | EF Core saves and reloads every invalid bad order | `TalkDbContext`: `HasField("_lines")` and `HasConversion<string>()` on `Status` | EF Core saves and reloads the good order through the private constructor; the `Status` column holds `'Confirmed'` |

Step 6 includes the `OwnsOne` mapping for `Total` because the app can't build its EF Core model once `Total` is a `Money` without it. Step 7 makes the backing field and the string status column explicit.

---

## 6. The Timeline

| Time | Segment |
|---|---|
| 0–4 min | Slides: the `Order` invariants (customer and at least one line; `Total` equals the sum of the lines; positive quantities, each product once; one currency; Placed → Processing → Confirmed → Shipped; a cancelled order can't be shipped) |
| 4–9 min | Walk the bad model, then run options 1–7 to show every problem and every "Not Yet Implemented" box |
| 9–13 min | Step 1: enum |
| 13–18 min | Step 2: private setters + domain methods |
| 18–23 min | Step 3: read-only collection + backing field |
| 23–27 min | Step 4: private constructor |
| 27–31 min | Step 5: guard clauses |
| 31–36 min | Step 6: value object |
| 36–41 min | Step 7: EF Core persists the good model |
| 41–45 min | Recap + Q&A: the bad model next to the good model |

---

## 7. Decisions

- **Two backing models, bad and good.** The bad model never changes.
- **One demo file, `TalkDemo.cs`, holds the menu and every call.** No caller classes.
- **Keep `CustomerName` as a string.** No `Customer` entity and no shadow `CustomerId`.
- **Tag each step** (`talk-step-0` through `talk-step-7`) so you can recover with `git checkout` if a step goes wrong on stage.
- **Run as x64** (`dotnet run --project TalkApp -a x64`) on this machine; LocalDB returns error 193 otherwise.

---

## 8. Verified

Built and run on 2026-10-30 by applying each step's TODOs in order to a scratch copy:
- Step 0 and each of steps 1–7: 0 compile errors, 0 compiler warnings.
- Every menu option runs at every step; at step 7 all seven solution boxes show the results listed in section 5.
- The only build warnings are NuGet advisories for the transitive `System.Security.Cryptography.Pkcs` 9.0.4 package pulled in by the SQL Server provider.

---

## 9. Next Action

- Commit step 0 and tag it `talk-step-0`.
- Write the slides from this repo.
