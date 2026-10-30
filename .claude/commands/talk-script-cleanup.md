# Talk Script Cleanup

You are cleaning up an existing excerpt of narration for the **Creating Domain Invariants** talk: a `NARRATION:` line in `Talk/DemoSteps.md`, or `**Say:**` prose in `Talk/SlidePlan.md`. This is a targeted fix to text that already exists. Use `/talk-demo-script` or `/talk-slide-plan` for new content.

## Scope Discipline

- Fix only what's flagged. If Brian highlights a phrase and says what's wrong with it, fix that phrase, not the rest of the sentence and not the surrounding lines.
- Apply the fix directly to the file, then show the result.
- If only one sentence in a block is bad, rewrite only that sentence.
- Preserve everything structural: headers, `[ACTION: ...]` lines, code fences, backticks, inline reveal cues, `---` separators.
- Keep the same approximate length unless a shorter version is clearly better.

## Voice Rules

Every rule in the `talk-demo-script` skill's "Voice and Tone" section applies here without exception. Read that skill's rules rather than re-deriving them.

## Before Rewriting a Technical Claim

If the excerpt claims what code does (what the box prints, what no longer compiles, what throws an exception, what EF Core stores), open the source file in `OrderModels/` or `TalkApp/` first, or run `/talk-verify`. A rewrite that sounds better but is factually wrong is worse than the original.

## Output

- Edit the file directly. Show the final line, not a floating draft.
- No commentary unless Brian asks.
- If the same problem exists elsewhere in the file, say so in one sentence before moving on.
