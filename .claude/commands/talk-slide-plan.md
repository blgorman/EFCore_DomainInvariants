# Talk Slide Plan Writer

You are helping Brian Gorman write or update the slide plan for his 45-minute talk **Creating Domain Invariants**. Brian is an experienced technical trainer and speaker. No hand-holding, motivation, or meta-commentary.

## Where Things Live

- Talk structure, timeline, and problem-to-solution map: `TalkPlan.md`
- Design rules: `CLAUDE.md`
- Slide plan (create or update this): `Talk/SlidePlan.md`
- Demo narration (if written): `Talk/DemoSteps.md`
- Code shown on slides: `OrderModels/Anemic/`, `OrderModels/Domain/`, `TalkApp/TalkDemo.cs`, `TalkApp/Data/TalkDbContext.cs`

## What To Read First

1. `TalkPlan.md` for the timeline segment being planned and its time budget.
2. `CLAUDE.md` for the settled design. Never plan a slide that contradicts it (one demo file, two backing models, nothing typed on stage).
3. `Talk/DemoSteps.md`, if it exists, so transition slides match the demo.
4. The existing `Talk/SlidePlan.md`, if updating, so you keep what's already correct.
5. The actual source files for any code shown on a slide. Copy code from the source. For good-model code, show the state **after** that step's TODOs are applied, and say so in the layout line. Never show code from memory.

## Format (match this exactly)

```
# Creating Domain Invariants — Slide Plan

> {N} slides. Demo segments are marked with transition slides that name the menu option.

---

## Segment: {timeline segment from TalkPlan.md, e.g. "13–17 min: Step 2, private setters"}

## Slide N — {Slide Title}

**Layout:** {one or two sentences describing the visual layout}

**Text on slide (verbatim, shown all at once / revealed one at a time):**
```
{exact text/code that appears on the slide}
```

**Say:**
"{narration, with inline reveal cues in brackets where timing matters}"

---
```

- Group slides under `## Segment:` headers that match the `TalkPlan.md` timeline rows, so each segment's slide count can be checked against its minutes.
- Reveal timing goes inline inside `**Say:**` (`[Bullet 1 appears]`), never as standalone tokens.
- The last slide before each demo is a transition slide: title, one line of body text, and the menu option as a code-style line (e.g. `Option 2 — Shipping without confirming`).
- Slide titles name the concept, never an action phrase like "Let's Make the Change".
- ASCII punctuation. No em dashes on a slide or in `**Say:**`.

## Voice for the `**Say:**` Prose

`**Say:**` is spoken narration and follows every rule in the `talk-demo-script` skill's "Voice and Tone" section. Read the rules there rather than duplicating them.

Slide text can be terse (short bullets, headers). `**Say:**` must be full spoken sentences. A `**Say:**` block names only identifiers visible on that slide, and opens with what the code on the slide produces and why.

## How To Use This Skill

When invoked, ask Brian which segment(s) to plan, whether this is new or an update, and anything to emphasize or cut. Read the sources above, then write directly into `Talk/SlidePlan.md`. Don't paste a draft into chat.

At the end, state the file updated and a one-line summary: slide count per segment and whether each segment fits its minutes.
