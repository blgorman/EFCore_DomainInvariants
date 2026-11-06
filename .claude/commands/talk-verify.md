# Talk Verify

Verify that the talk demo still works at every step after any change to `OrderModels/`, `TalkApp/`, or the `//TODO: Talk Step N` blocks.

## What To Run

```bash
bash tools/verify_steps.sh "<scratchpad directory>"
```

The script copies the solution to `<scratchpad>/sim`, builds step 0, applies steps 1 to 5 in order with `tools/apply_step.py` (building after each), then runs every menu option at the final state. It never touches the real repo files.

## What Passing Looks Like

- `== STEP 0` through `== STEP 7` each show `0 Error(s)` and no `warning CS` lines.
- `LEFTOVER TODOS` lists only the two "Not Yet Implemented" box strings in `TalkApp/TalkDemo.cs`.
- `BOX OVERFLOW CHECK` prints nothing.
- `SOLUTION BOXES` shows all seven "Good:" boxes with the results listed in `TalkPlan.md` section 5.

Also run step 0 for real (`printf '1\n2\n3\n4\n5\n6\n' | dotnet run --project TalkApp -a x64`) and confirm each option prints its "Bad:" box followed by the "Not Yet Implemented" box.

## Reporting

- Report the result in one plain sentence: which steps passed, and the first failure if any, with the full file path and line number.
- On any failure, stop. Don't fix it unless Brian says to.
- If the script couldn't run (LocalDB down, build lock from a running instance), say the result is unverified. Never report a pass you didn't see.
