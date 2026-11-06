import re, sys, pathlib
root_folder, step_number = pathlib.Path(sys.argv[1]), sys.argv[2]
line_counts_by_word = {"line": 1, "two": 2, "three": 3, "five": 5}
todo_pattern = re.compile(r'^(\s*)//TODO: Talk Step ' + step_number + r' — (.*)$')
def uncomment(source_line):
    comment_start = source_line.index("//")
    return source_line[:comment_start] + source_line[comment_start+2:]
for source_file in root_folder.rglob("*.cs"):
    if "bin" in source_file.parts or "obj" in source_file.parts: continue
    source_lines = source_file.read_text(encoding="utf-8").split("\n")
    output_lines, line_index, file_changed = [], 0, False
    while line_index < len(source_lines):
        todo_match = todo_pattern.match(source_lines[line_index])
        if not todo_match:
            output_lines.append(source_lines[line_index]); line_index += 1; continue
        file_changed = True
        todo_text = todo_match.group(2)
        line_index += 1
        if todo_text.startswith("Delete the next line"):
            print(f"{source_file.name}: delete -> {source_lines[line_index].strip()}")
            line_index += 1
        elif "property above" in todo_text:
            property_index = len(output_lines) - 1
            while not output_lines[property_index].strip(): property_index -= 1
            print(f"{source_file.name}: delete -> {output_lines[property_index].strip()}")
            del output_lines[property_index]
        count_word = re.search(r"uncomment the (\w+)", todo_text, re.I).group(1).lower()
        if count_word == "code":
            while line_index < len(source_lines) and source_lines[line_index].lstrip().startswith("//"):
                output_lines.append(uncomment(source_lines[line_index])); line_index += 1
        else:
            line_count = line_counts_by_word.get(count_word, 1)
            for _ in range(line_count):
                output_lines.append(uncomment(source_lines[line_index])); line_index += 1
    if file_changed:
        source_file.write_text("\n".join(output_lines), encoding="utf-8")
