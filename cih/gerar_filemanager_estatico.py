#!/usr/bin/env python3
"""Gera uma versão estática (HTML) do gerenciador de arquivos CIH."""

import os
import html
import urllib.parse
from datetime import datetime

BASE_DIR = "/home/headless/workspace/cih"
OUTPUT_DIR = "/home/headless/workspace/cih/_filemanager"

CSS = """
* { margin: 0; padding: 0; box-sizing: border-box; }
body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background: #f0f2f5; color: #1a1a2e; min-height: 100vh;
}
.header {
  background: linear-gradient(135deg, #0097A7 0%, #00796B 100%);
  color: #fff; padding: 20px 32px;
  box-shadow: 0 2px 12px rgba(0,0,0,0.15);
}
.header h1 { font-size: 1.5rem; font-weight: 600; }
.header .subtitle { font-size: 0.85rem; opacity: 0.7; margin-top: 4px; }
.breadcrumb {
  background: #fff; padding: 12px 32px; border-bottom: 1px solid #e0e0e0;
  display: flex; align-items: center; flex-wrap: wrap; gap: 4px; font-size: 0.9rem;
}
.breadcrumb a { color: #00796B; text-decoration: none; padding: 4px 8px; border-radius: 4px; transition: background 0.2s; }
.breadcrumb a:hover { background: #E0F2F1; }
.breadcrumb .sep { color: #999; margin: 0 2px; }
.toolbar { padding: 16px 32px; display: flex; align-items: center; gap: 12px; }
.btn-up {
  display: inline-flex; align-items: center; gap: 6px; padding: 8px 18px;
  background: #fff; border: 1px solid #d0d0d0; border-radius: 6px;
  color: #333; text-decoration: none; font-size: 0.88rem; font-weight: 500;
  transition: all 0.2s; cursor: pointer;
}
.btn-up:hover { background: #E0F2F1; border-color: #0097A7; color: #0097A7; }
.btn-up.disabled { opacity: 0.4; pointer-events: none; }
.info-bar { padding: 0 32px 8px; font-size: 0.82rem; color: #888; }
.container { padding: 0 32px 32px; }
table {
  width: 100%; border-collapse: collapse; background: #fff;
  border-radius: 10px; overflow: hidden; box-shadow: 0 1px 6px rgba(0,0,0,0.07);
}
thead th {
  background: #f8f9fa; padding: 14px 16px; text-align: left;
  font-weight: 600; font-size: 0.82rem; text-transform: uppercase;
  letter-spacing: 0.5px; color: #666; border-bottom: 2px solid #e8e8e8;
}
thead th:first-child { padding-left: 24px; }
tbody tr { transition: background 0.15s; }
tbody tr:hover { background: #E0F2F1; }
tbody td { padding: 12px 16px; border-bottom: 1px solid #f0f0f0; font-size: 0.9rem; }
tbody td:first-child { padding-left: 24px; }
tbody tr:last-child td { border-bottom: none; }
.name-cell { display: flex; align-items: center; gap: 10px; }
.name-cell a { color: #1a1a2e; text-decoration: none; font-weight: 500; transition: color 0.2s; }
.name-cell a:hover { color: #0097A7; }
.icon {
  width: 32px; height: 32px; display: flex; align-items: center; justify-content: center;
  border-radius: 6px; font-size: 1.1rem; flex-shrink: 0;
}
.icon-folder { background: #FFF3E0; color: #F57C00; }
.icon-file { background: #E3F2FD; color: #1976D2; }
.icon-image { background: #FCE4EC; color: #E91E63; }
.icon-pdf { background: #FFEBEE; color: #D32F2F; }
.icon-code { background: #E8F5E9; color: #388E3C; }
.icon-doc { background: #E8EAF6; color: #3F51B5; }
.icon-archive { background: #FFF8E1; color: #F9A825; }
.icon-xlsx { background: #E8F5E9; color: #2E7D32; }
.size-cell { color: #888; white-space: nowrap; }
.date-cell { color: #888; white-space: nowrap; }
.empty-msg { text-align: center; padding: 48px 16px; color: #aaa; font-size: 1rem; }
@media (max-width: 768px) {
  .header, .breadcrumb, .toolbar, .info-bar, .container { padding-left: 16px; padding-right: 16px; }
  .date-cell, thead th:last-child { display: none; }
}
"""


def format_size(size_bytes):
    if size_bytes < 1024:
        return f"{size_bytes} B"
    elif size_bytes < 1024 * 1024:
        return f"{size_bytes / 1024:.1f} KB"
    elif size_bytes < 1024 * 1024 * 1024:
        return f"{size_bytes / (1024 * 1024):.1f} MB"
    return f"{size_bytes / (1024 * 1024 * 1024):.2f} GB"


def get_icon(name, is_dir):
    if is_dir:
        return "icon-folder", "\U0001f4c1"
    ext = os.path.splitext(name)[1].lower()
    icons = {
        ".pdf": ("icon-pdf", "\U0001f4d1"),
        ".png": ("icon-image", "\U0001f5bc"),
        ".jpg": ("icon-image", "\U0001f5bc"),
        ".jpeg": ("icon-image", "\U0001f5bc"),
        ".gif": ("icon-image", "\U0001f5bc"),
        ".svg": ("icon-image", "\U0001f5bc"),
        ".py": ("icon-code", "\U0001f40d"),
        ".js": ("icon-code", "\U0001f4dd"),
        ".html": ("icon-code", "\U0001f310"),
        ".css": ("icon-code", "\U0001f3a8"),
        ".json": ("icon-code", "{ }"),
        ".md": ("icon-code", "\U0001f4dd"),
        ".sh": ("icon-code", ">_"),
        ".yml": ("icon-code", "\u2699"),
        ".yaml": ("icon-code", "\u2699"),
        ".xlsx": ("icon-xlsx", "\U0001f4ca"),
        ".xls": ("icon-xlsx", "\U0001f4ca"),
        ".csv": ("icon-xlsx", "\U0001f4ca"),
        ".doc": ("icon-doc", "\U0001f4c3"),
        ".docx": ("icon-doc", "\U0001f4c3"),
        ".txt": ("icon-doc", "\U0001f4c4"),
        ".zip": ("icon-archive", "\U0001f4e6"),
        ".tar": ("icon-archive", "\U0001f4e6"),
        ".gz": ("icon-archive", "\U0001f4e6"),
    }
    return icons.get(ext, ("icon-file", "\U0001f4c4"))


SKIP_DIRS = {".git", ".claude", "__pycache__", "node_modules", "_filemanager", ".session"}


def generate_page(rel_path, full_path):
    entries_raw = []
    try:
        items = os.listdir(full_path)
    except PermissionError:
        items = []

    dirs = []
    files = []
    for name in sorted(items, key=str.lower):
        if name.startswith(".") or name in SKIP_DIRS:
            continue
        entry_full = os.path.join(full_path, name)
        try:
            stat = os.stat(entry_full)
        except OSError:
            continue
        is_dir = os.path.isdir(entry_full)
        mod_time = datetime.fromtimestamp(stat.st_mtime).strftime("%d/%m/%Y %H:%M")
        entry = {"name": name, "is_dir": is_dir, "size": stat.st_size if not is_dir else 0, "mod_time": mod_time}
        if is_dir:
            dirs.append(entry)
        else:
            files.append(entry)

    all_entries = dirs + files

    # Breadcrumb
    parts = [p for p in rel_path.split("/") if p]
    bc = ['<a href="/index.html">\U0001f3e0 cih</a>']
    acc = ""
    for p in parts:
        acc += "/" + p
        bc.append(f'<span class="sep">/</span><a href="{acc}/index.html">{html.escape(p)}</a>')
    breadcrumb = '<div class="breadcrumb">' + "".join(bc) + '</div>'

    # Up button
    if rel_path:
        parent_parts = parts[:-1]
        parent_href = "/" + "/".join(parent_parts) + "/index.html" if parent_parts else "/index.html"
        up_btn = f'<a class="btn-up" href="{parent_href}">\u2B06 Voltar</a>'
    else:
        up_btn = '<span class="btn-up disabled">\u2B06 Voltar</span>'

    # Info
    n_dirs = len(dirs)
    n_files = len(files)
    info_parts = []
    if n_dirs:
        info_parts.append(f"{n_dirs} pasta{'s' if n_dirs != 1 else ''}")
    if n_files:
        info_parts.append(f"{n_files} arquivo{'s' if n_files != 1 else ''}")
    info_text = " &bull; ".join(info_parts) if info_parts else "Pasta vazia"

    # Rows
    rows = []
    for e in all_entries:
        icon_cls, icon_char = get_icon(e["name"], e["is_dir"])
        ename = html.escape(e["name"])
        if e["is_dir"]:
            href = ("/" + rel_path + "/" + e["name"] + "/index.html").replace("//", "/")
            size_str = "&mdash;"
        else:
            href = ("/" + rel_path + "/" + urllib.parse.quote(e["name"])).replace("//", "/")
            size_str = format_size(e["size"])

        rows.append(
            f'<tr>'
            f'<td><div class="name-cell"><span class="icon {icon_cls}">{icon_char}</span>'
            f'<a href="{href}">{ename}</a></div></td>'
            f'<td class="size-cell">{size_str}</td>'
            f'<td class="date-cell">{e["mod_time"]}</td>'
            f'</tr>'
        )

    if not rows:
        rows_html = '<tr><td colspan="3" class="empty-msg">Esta pasta est\u00e1 vazia</td></tr>'
    else:
        rows_html = "\n".join(rows)

    page_html = f"""<!DOCTYPE html>
<html lang="pt-BR">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>CIH - Gerenciador de Arquivos</title>
<style>{CSS}</style>
</head>
<body>
<div class="header">
  <h1>\U0001f4c2 CIH - Gerenciador de Arquivos</h1>
  <div class="subtitle">Navegador de arquivos do workspace</div>
</div>
{breadcrumb}
<div class="toolbar">{up_btn}</div>
<div class="info-bar">{info_text}</div>
<div class="container">
<table>
  <thead><tr><th>Nome</th><th>Tamanho</th><th>Data de Modifica\u00e7\u00e3o</th></tr></thead>
  <tbody>
{rows_html}
  </tbody>
</table>
</div>
</body>
</html>"""

    # Write
    out_dir = os.path.join(OUTPUT_DIR, rel_path) if rel_path else OUTPUT_DIR
    os.makedirs(out_dir, exist_ok=True)
    out_file = os.path.join(out_dir, "index.html")
    with open(out_file, "w", encoding="utf-8") as f:
        f.write(page_html)

    # Recurse into subdirs
    for d in dirs:
        sub_rel = (rel_path + "/" + d["name"]).strip("/")
        sub_full = os.path.join(full_path, d["name"])
        if d["name"] not in SKIP_DIRS:
            generate_page(sub_rel, sub_full)


if __name__ == "__main__":
    # Clean output
    import shutil
    if os.path.exists(OUTPUT_DIR):
        shutil.rmtree(OUTPUT_DIR)
    os.makedirs(OUTPUT_DIR)

    generate_page("", BASE_DIR)

    # Count pages
    count = 0
    for root, dirs, files in os.walk(OUTPUT_DIR):
        count += sum(1 for f in files if f == "index.html")
    print(f"Gerado {count} p\u00e1ginas em {OUTPUT_DIR}")
