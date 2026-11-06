import re, sys, pathlib
root, step = pathlib.Path(sys.argv[1]), sys.argv[2]
words = {"line": 1, "two": 2, "three": 3, "five": 5}
todo = re.compile(r'^(\s*)//TODO: Talk Step ' + step + r' — (.*)$')
def uncomment(s):
    i = s.index("//")
    return s[:i] + s[i+2:]
for f in root.rglob("*.cs"):
    if "bin" in f.parts or "obj" in f.parts: continue
    lines = f.read_text(encoding="utf-8").split("\n")
    out, i, changed = [], 0, False
    while i < len(lines):
        m = todo.match(lines[i])
        if not m:
            out.append(lines[i]); i += 1; continue
        changed = True
        text = m.group(2)
        i += 1
        if text.startswith("Delete the next line"):
            print(f"{f.name}: delete -> {lines[i].strip()}")
            i += 1
        elif "property above" in text:
            k = len(out) - 1
            while not out[k].strip(): k -= 1
            print(f"{f.name}: delete -> {out[k].strip()}")
            del out[k]
        what = re.search(r"uncomment the (\w+)", text, re.I).group(1).lower()
        if what == "code":
            while i < len(lines) and lines[i].lstrip().startswith("//"):
                out.append(uncomment(lines[i])); i += 1
        else:
            n = words.get(what, 1)
            for _ in range(n):
                out.append(uncomment(lines[i])); i += 1
    if changed:
        f.write_text("\n".join(out), encoding="utf-8")
