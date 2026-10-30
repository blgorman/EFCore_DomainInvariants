#!/usr/bin/env bash
# Copies the solution to a scratch folder, applies every //TODO: Talk Step in order,
# and builds after each step. Then runs all seven menu options at the final state.
# Usage: tools/verify_steps.sh <scratch-folder>
set -u
REPO="$(cd "$(dirname "$0")/.." && pwd)"
SIM="${1:?scratch folder required}/sim"

rm -rf "$SIM" && mkdir -p "$SIM"
cp -r "$REPO/ConsoleHelpers" "$REPO/OrderModels" "$REPO/TalkApp" "$REPO/Directory.Build.props" "$REPO/EFCore_DomainInvariants.slnx" "$SIM/"
rm -rf "$SIM"/*/bin "$SIM"/*/obj
cd "$SIM"

echo "== STEP 0: $(dotnet build TalkApp -a x64 -v q 2>&1 | grep -E 'error CS|warning CS|rror\(s\)' | sort -u | tr '\n' ' ')"
for n in 1 2 3 4 5 6 7; do
  python "$REPO/tools/apply_step.py" . "$n" > /dev/null
  echo "== STEP $n: $(dotnet build TalkApp -a x64 -v q 2>&1 | grep -E 'error CS|warning CS|rror\(s\)' | sort -u | tr '\n' ' ')"
done

echo "== LEFTOVER TODOS (expect only the two NTI box strings in TalkDemo.cs)"
grep -rn "TODO: Talk" --include=*.cs . | grep -v "/bin/\|/obj/"

echo "== FINAL STATE, BOX OVERFLOW CHECK"
printf '1\n2\n3\n4\n5\n6\n7\n8\n' | dotnet run --project TalkApp -a x64 --no-build 2>&1 | grep -E "^\* " | awk 'length($0) != 80 { print "OVERFLOW: " $0 }'

echo "== FINAL STATE, SOLUTION BOXES"
printf '1\n2\n3\n4\n5\n6\n7\n8\n' | dotnet run --project TalkApp -a x64 --no-build 2>&1 | grep -E "^\* " | sed -n '/Good:/,/Bad:/p' | grep -v "Bad:"
