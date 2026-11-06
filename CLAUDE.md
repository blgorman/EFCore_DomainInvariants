# CLAUDE.md — Creating Domain Invariants (Talk)

## What This Repo Is

The demo app, plan, slides, and narration for Brian Gorman's 45-minute talk **Creating Domain Invariants**, delivered 7 November 2026. The talk takes the anemic `Order` from the EF Core + DDD Pluralsight course (Module 1 Clip 2) and hardens a copy of it one technique at a time, live, while the original anemic copy stays broken beside it.

`TalkPlan.md` is the source of truth for the talk's structure. Read it before changing anything.

---

## The Design (settled, do not re-open)

- **Two backing models.** `OrderModels/Anemic` is the bad model and **never changes**. `OrderModels/Domain` is the good model; it starts as a copy of the bad model and is what changes on stage.
- **Each model has its own `Order`, `OrderLine`, and `Money`.** The good model's `OrderLine` and `Money` change on stage (`Money` at step 4, `OrderLine` at step 5), so the bad model cannot share them. Brian questioned this on 2026-10-30 and kept the separate copies.
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
- The TODO states the exact count: "the line", "the two lines", "the three lines", "the four lines", "the five lines", or "the code" (everything commented until the block ends).
- A line that changes twice carries the later TODO double-commented (`////TODO`) inside the earlier block, so uncommenting the earlier block produces a normal `//TODO`.
- Each `TalkDemo` solution method starts with `throw new NotImplementedException();` under its TODO. The `TalkDemo` TODO is the last one applied for each step.
- `Order.Place` works at step 0, as in the course starter, and every solution creates its order through `Place`. Step 1 makes the constructor private and adds the `Place` guards.
- `OrderStatus` and `HasConversion<string>()` on `Status` exist at step 0, as in the course starter. There is no enum step.
- `TalkApp/Data/TalkDbContext.cs` changes in the step that creates the member it maps: step 4 (`OwnsOne` for `Total`), step 5 (`HasField("_lines")`). Solutions 1, 4, and 5 save and reload through EF Core so each mapping is exercised.
- Each step solves one problem and finishes the code it touches, guards included, in the build order below. This is Brian's chosen order (2026-11-05), not the course's clip order. Every step opens with its problem before the solution (slides and demo).
- Menu titles and slide titles name the concept and its solution, never the problem.

### The five steps (seven techniques, build order)

| Step | Techniques | Menu option |
|---|---|---|
| 1 | Private `Order()` constructor, factory method `Order.Place`, `Place` guards, EF Core materialization | Private constructor and Order.Place |
| 2 | Private setters | Private setters |
| 3 | `Process`/`Confirm`/`Ship`/`Cancel` | Domain methods control Status |
| 4 | `Total` becomes `Money` + `OwnsOne`, `Money.Create` and `Money.Add` guards | Total as a Money value object |
| 5 | `_lines` backing field + `IReadOnlyCollection`, `AddLine`, internal `OrderLine` constructor + `HasField("_lines")`, `AddLine` and `OrderLine` guards | Backing field and read-only Lines |

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
| `tools/verify_steps.sh` | Copies the solution to scratch, applies steps 1–5 in order, builds after each, runs every option at the end. |
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

## Slide Rules (Brian's decisions, do not regress)

- **The talk is about the technology, never about how to run the demos.** No slide explains the `//TODO: Talk Step N` pattern, the repo layout, the two-model setup, or how the demo works. "Two Models, One Demo" and "The Talk Step Pattern" were removed on purpose and stay out.
- **A demo slide has three lines:** `Demo`, the concept name, and one line stating what the technique achieves. No step number, option number, or menu text on the slide. All of that goes in the speaker notes.
- **Every line on a slide must say something about the concept.** "Every problem runs, every solution waits" is the example of what never goes on a slide.
- **Boxes appear on click.** A slide with several cards or code panels reveals them one click at a time. The invariant cards (slide 5) and the stacked code panels (slides 15 and 31) keep their click animations. The agenda (slide 3) is a quick list and is not animated.
- **The live deck is `Talk/CreatingDomainInvariants.pptx`.** Every deck change starts from that file, never from `Template.pptx` or a `Talk/CreatingDomainInvariants.backup-*.pptx` file, and keeps the existing animations.
- **After any deck change,** compare each slide's click count with the slide's plan in `Talk/SlidePlan.md`, and search the slide text for `Option`, `Talk Step`, and `Not Yet Implemented`.
- **Never write to the deck while PowerPoint has `Talk/CreatingDomainInvariants.pptx` open.** Check for a running `POWERPNT` process and an exclusive file lock. `Talk/~$CreatingDomainInvariants.pptx` alone proves nothing, because that lock file can be left behind after PowerPoint closes.

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
