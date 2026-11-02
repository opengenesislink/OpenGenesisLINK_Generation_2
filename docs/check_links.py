#!/usr/bin/env python3
"""Check local Markdown link targets in this documentation-only repository."""
from pathlib import Path
from urllib.parse import unquote, urlsplit
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
LINK = re.compile(r"(?<!!)\[[^\]]*\]\(([^)]+)\)")
FENCE = re.compile(r"^\s*(?:`{3,}|~{3,})")
errors = []

for markdown in sorted(ROOT.rglob("*.md")):
    if any(part.startswith(".") and part != ".github" for part in markdown.relative_to(ROOT).parts):
        continue
    in_fence = False
    for line_no, line in enumerate(markdown.read_text(encoding="utf-8").splitlines(), 1):
        if FENCE.match(line):
            in_fence = not in_fence
            continue
        if in_fence:
            continue
        for match in LINK.finditer(line):
            raw = match.group(1).strip().split(" ", 1)[0].strip("<>")
            if not raw or raw.startswith("#"):
                continue
            parsed = urlsplit(raw)
            if parsed.scheme or parsed.netloc or raw.startswith("//"):
                continue
            target = (markdown.parent / unquote(parsed.path)).resolve()
            if not target.is_relative_to(ROOT.resolve()) or not target.exists():
                errors.append(f"{markdown.relative_to(ROOT)}:{line_no}: broken link: {raw}")

if errors:
    print("\n".join(errors))
    sys.exit(1)
print("Local Markdown link check passed.")
