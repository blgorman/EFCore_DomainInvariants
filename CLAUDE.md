# CLAUDE.md — Creating Domain Invariants (Talk)

## What This Repo Is

The demo app, plan, slides, and narration for Brian Gorman's 45-minute talk **Creating Domain Invariants**, delivered 7 November 2026. The talk takes the anemic `Order` from the EF Core + DDD Pluralsight course (Module 1 Clip 2) and hardens a copy of it one technique at a time, live, while the original anemic copy stays broken beside it.

`TalkPlan.md` is the source of truth for the talk's structure. Read it before changing anything.

---

## The Design (settled, do not re-open)

- **Two backing models.** `OrderModels/Anemic` is the bad model and **never changes**. `OrderModels/Domain` is the good model; it starts as a copy of the bad model and is what changes on stage.
- **Each model has its own `Order`, `OrderLine`, and `Money`.** The good model's `OrderLine` and `Money` change on stage (internal constructor at step 2, guards at step 7), so the bad model cannot share them. Brian questioned this on 2026-10-30 and kept the separate copies.
- **One demo file: `TalkApp/TalkDemo.cs`.** It holds the menu and every call. No caller classes, no second demo file.
- **Every menu option works from step 0.** Option N shows the problem on the bad model, then the solution on the good model. Before step N, the solution shows a "Not Yet Implemented" box.
- **Nothing is typed on stage.** Every change is "delete one live line, uncomment the line(s) below it" or "uncomment the guard below". No signature is typed live; every member the demo calls exists at step 0 with its final signature.

### The `//TODO: Talk Step N` pattern

```csharp
//TODO: Talk Step 3 — Delete the next line and uncomment the line below it
public DateTime OrderDate { get; set; }
//public DateTime OrderDate { get; private set; }
```

- The live (bad) line is always directly after the TODO.
- The TODO states the exact count: "the line", "the two lines", "the three lines", "the five lines", or "the code" (everything commented until the block ends).
- A line that changes twice carries the later TODO double-commented (`////TODO`) inside the earlier block, so uncommenting the earlier block produces a normal `//TODO`.
- Each `TalkDemo` solution method starts with `throw new NotImplementedException();` under its TODO. The `TalkDemo` TODO is the last one applied for each step.
- `Order.Place` works at step 0. Step 1 (course Clip 1) changes only `TalkDemo`; every later solution creates its order through `Place`.
- `OrderStatus` and `HasConversion<string>()` on `Status` exist at step 0, as in the course starter. There is no enum step.
- `TalkApp/Data/TalkDbContext.cs` changes in the step that creates the member it maps: step 2 (`HasField("_lines")`), step 5 (`OwnsOne` for `Total`). Solutions 2, 5, and 6 save and reload through EF Core so each mapping is exercised.
- One concept per step, in the course's Module 2 order, and every step opens with its problem before the solution (slides and demo).
- Menu titles and slide titles name the concept and its solution, never the problem.

### The seven steps (course Module 2 order)

| Step | Course | Technique | Menu option |
|---|---|---|---|
| 1 | Clip 1 | Factory method `Order.Place` (no domain edit) | Factory method: Order.Place |
| 2 | Clip 2 | `_lines` backing field + `IReadOnlyCollection`, `AddLine`, internal `OrderLine` constructor + `HasField("_lines")` | Backing field and read-only Lines |
| 3 | Clip 3 | Private setters | Private setters |
| 4 | Clip 3 | `Process`/`Confirm`/`Ship`/`Cancel` | Domain methods control Status |
| 5 | Clip 4 | `Total` becomes `Money` + `OwnsOne` | Total as a Money value object |
| 6 | Clip 7 | Private `Order()` constructor, EF Core materialization, shadow FK shown | Private constructor and EF materialization |
| 7 | Clip 8 | Guard clauses (`Place`, `AddLine`, `OrderLine`, `Money.Create`, `Money.Add`) | Guard clauses |

---|---|---|
| 1 | `Status` becomes the `OrderStatus` enum + `HasConversion<string>()` | Status is a free string |
| 2 | Private setters | Changing an order after it is placed |
| 3 | `Process`/`Confirm`/`Ship`/`Cancel` | Shipping without confirming |
| 4 | `_lines` backing field + `IReadOnlyCollection`, `AddLine`, internal `OrderLine` constructor + `HasField("_lines")` | Lines changed behind the order's back |
| 5 | Private `Order()` constructor | An empty order |
| 6 | Guard clauses (`Place`, `AddLine`, `OrderLine`) | Bad quantities and duplicate products |
| 7 | `Total` becomes `Money` + `Money` guards + `OwnsOne` | Negative and mixed-currency totals |

---

## Solution Structure

| Path | Role |
|---|---|
| `OrderModels/Anemic` | Bad model. Never edited. |
| `OrderModels/Domain` | Good model. All talk TODOs except `TalkDemo` and `TalkDbContext`. No EF Core reference. |
| `TalkApp/TalkDemo.cs` | Menu, problem methods, solution methods. |
| `TalkApp/Data/TalkDbContext.cs` | Maps both models (`AnemicOrders`, `DomainOrders`). |
| `TalkApp/Program.cs` | `EnsureDeletedAsync` + `EnsureCreatedAsync` on every run. No migrations, ever. |
| `ConsoleHelpers` | `OutputHelpers` / `InputHelpers` copied from the course. |
| `tools/apply_step.py` | Applies one step's TODOs mechanically to a folder. Scratch copies only. |
| `tools/verify_steps.sh` | Copies the solution to scratch, applies steps 1–7 in order, builds after each, runs every option at the end. |
| `Talk/` | Slide plan and demo narration (see "Talk Documents"). |

---

## Running

- **Database:** `TalkApp/Program.cs` loads user secrets over `appsettings.json`. On the Surface (ARM64) the secret points at the `sqledge` Docker container; LocalDB is x64 only and fails with error 193 there.
- `System.Security.Cryptography.Pkcs` is pinned to 10.0.14 in `TalkApp/TalkApp.csproj` to clear the advisories on the version `Microsoft.Data.SqlClient` pulls in.
- Never apply talk steps to the real repo files to test them. Use `tools/verify_steps.sh` against a scratch folder.

---

## Talk Documents

| File | Contents | Skill |
|---|---|---|
| `TalkPlan.md` | Structure, timeline, problem → solution map, decisions | — |
| `Talk/SlidePlan.md` | Every slide, in order: layout, verbatim text, `**Say:**` narration | `/talk-slide-plan` |
| `Talk/DemoSteps.md` | Per step: what to delete/uncomment, menu option to run, narration | `/talk-demo-script` |

Use `/talk-script-cleanup` to fix a flagged line in either file. Use `/talk-verify` after any code change.

---

## Working With Brian (hard rules)

- **A question is not an edit request.** Answer it and change nothing. Only an explicit instruction authorizes an edit.
- **Talk it through before changing a settled design.** If a change has consequences (a lost beat, a renamed menu option, a database shape change), state them and ask first. Never edit first and explain after.
- **Plan wording vs. build wording.** When changing a document, say "I'll write X into the plan", never "I'll build X".
- **Never commit or tag without showing the staged work and getting approval.** One approval covers one batch.
- **Never claim code fails or works without running it.** "Unverified" is the only allowed wording otherwise.
- **Name the noun every time.** No "it", "this", "that" for a known class, method, or file, in chat or in docs. Give full repo-relative paths with line numbers.
- **One finding, one sentence.** Report a defect plainly; no fix menus.
- **Yes/no means yes/no.** No "you're right". Don't append unrequested findings.
- **No em dashes in narration or slide text.** (The `//TODO` comments use them; that's code, not narration.)
