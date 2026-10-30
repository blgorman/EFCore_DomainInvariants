# CLAUDE.md — Creating Domain Invariants (Talk)

## What This Repo Is

The demo app, plan, slides, and narration for Brian Gorman's 45-minute talk **Creating Domain Invariants**, delivered 7 November 2026. The talk takes the anemic `Order` from the EF Core + DDD Pluralsight course (Module 1 Clip 2) and hardens a copy of it one technique at a time, live, while the original anemic copy stays broken beside it.

`TalkPlan.md` is the source of truth for the talk's structure. Read it before changing anything.

---

## The Design (settled, do not re-open)

- **Two backing models.** `OrderModels/Anemic` is the bad model and **never changes**. `OrderModels/Domain` is the good model; it starts as a copy of the bad model and is what changes on stage.
- **Each model has its own `Order`, `OrderLine`, and `Money`.** The good model's `OrderLine` and `Money` change on stage (internal constructor at step 3, guards at steps 5 and 6), so the bad model cannot share them. Brian questioned this on 2026-10-30 and kept the separate copies.
- **One demo file: `TalkApp/TalkDemo.cs`.** It holds the menu and every call. No caller classes, no second demo file.
- **Every menu option works from step 0.** Option N shows the problem on the bad model, then the solution on the good model. Before step N, the solution shows a "Not Yet Implemented" box.
- **Nothing is typed on stage.** Every change is "delete one live line, uncomment the line(s) below it" or "uncomment the guard below". No signature is typed live; every member the demo calls exists at step 0 with its final signature.

### The `//TODO: Talk Step N` pattern

```csharp
//TODO: Talk Step 2 — Delete the next line and uncomment the line below it
public DateTime OrderDate { get; set; }
//public DateTime OrderDate { get; private set; }
```

- The live (bad) line is always directly after the TODO.
- The TODO states the exact count: "the line", "the two lines", "the three lines", "the five lines", or "the code" (everything commented until the block ends).
- A line that changes twice carries the later TODO double-commented (`////TODO`) inside the earlier block, so uncommenting the earlier block produces a normal `//TODO`.
- Each `TalkDemo` solution method starts with `throw new NotImplementedException();` under its TODO. The `TalkDemo` TODO is the last one applied for each step.
- `Order.Place` works at step 0 because solutions 2 and 3 need an `Order` instance.
- Step 6 also edits `TalkApp/Data/TalkDbContext.cs` (`OwnsOne` for `Total`), because EF Core cannot build the model once `Total` is `Money` without it.

### The seven steps

| Step | Technique | Menu option |
|---|---|---|
| 1 | `Status` becomes the `OrderStatus` enum | Status is a free string |
| 2 | Private setters + `Process`/`Confirm`/`Ship`/`Cancel` | Shipping without confirming |
| 3 | `_lines` backing field + `IReadOnlyCollection`, `AddLine`, internal `OrderLine` constructor | Lines changed behind the order's back |
| 4 | Private `Order()` constructor | An empty order |
| 5 | Guard clauses (`Place`, `AddLine`, `OrderLine`) | Bad quantities and duplicate products |
| 6 | `Total` becomes `Money` + `Money` guards | Negative and mixed-currency totals |
| 7 | EF Core config: `HasField("_lines")`, `HasConversion<string>()` | Saving and reloading through EF Core |

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

- **Always run as x64:** `dotnet run --project TalkApp -a x64`. LocalDB fails with error 193 otherwise.
- The NuGet warnings for `System.Security.Cryptography.Pkcs` 9.0.4 come in transitively through the SQL Server provider. They are not compiler warnings.
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
