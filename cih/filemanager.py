#!/usr/bin/env python3
"""CIH - Gerenciador de Arquivos Web"""

import http.server
import json
import os
import urllib.parse
import html
import mimetypes
from datetime import datetime
from string import Template

PORT = 17006
BASE_DIR = "/home/headless/workspace/cih"

HTML_TEMPLATE = Template("""<!DOCTYPE html>
<html lang="pt-BR">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>CIH - Gerenciador de Arquivos</title>
<style>
  * { margin: 0; padding: 0; box-sizing: border-box; }
  body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background: #f0f2f5;
    color: #1a1a2e;
    min-height: 100vh;
  }
  .header {
    background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
    color: #fff;
    padding: 20px 32px;
    box-shadow: 0 2px 12px rgba(0,0,0,0.15);
  }
  .header h1 {
    font-size: 1.5rem;
    font-weight: 600;
    letter-spacing: 0.5px;
  }
  .header .subtitle {
    font-size: 0.85rem;
    opacity: 0.7;
    margin-top: 4px;
  }
  .breadcrumb {
    background: #fff;
    padding: 12px 32px;
    border-bottom: 1px solid #e0e0e0;
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    gap: 4px;
    font-size: 0.9rem;
  }
  .breadcrumb a {
    color: #0f3460;
    text-decoration: none;
    padding: 4px 8px;
    border-radius: 4px;
    transition: background 0.2s;
  }
  .breadcrumb a:hover { background: #e8eaf6; }
  .breadcrumb .sep { color: #999; margin: 0 2px; }
  .breadcrumb .current { color: #555; font-weight: 600; padding: 4px 8px; }
  .toolbar {
    padding: 16px 32px;
    display: flex;
    align-items: center;
    gap: 12px;
  }
  .btn-up {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 8px 18px;
    background: #fff;
    border: 1px solid #d0d0d0;
    border-radius: 6px;
    color: #333;
    text-decoration: none;
    font-size: 0.88rem;
    font-weight: 500;
    transition: all 0.2s;
    cursor: pointer;
  }
  .btn-up:hover { background: #f5f5f5; border-color: #0f3460; color: #0f3460; }
  .btn-up.disabled { opacity: 0.4; pointer-events: none; }
  .info-bar {
    padding: 0 32px 8px;
    font-size: 0.82rem;
    color: #888;
  }
  .container { padding: 0 32px 32px; }
  table {
    width: 100%;
    border-collapse: collapse;
    background: #fff;
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 1px 6px rgba(0,0,0,0.07);
  }
  thead th {
    background: #f8f9fa;
    padding: 14px 16px;
    text-align: left;
    font-weight: 600;
    font-size: 0.82rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    color: #666;
    border-bottom: 2px solid #e8e8e8;
  }
  thead th:first-child { padding-left: 24px; }
  tbody tr { transition: background 0.15s; }
  tbody tr:hover { background: #f5f7ff; }
  tbody td {
    padding: 12px 16px;
    border-bottom: 1px solid #f0f0f0;
    font-size: 0.9rem;
  }
  tbody td:first-child { padding-left: 24px; }
  tbody tr:last-child td { border-bottom: none; }
  .name-cell {
    display: flex;
    align-items: center;
    gap: 10px;
  }
  .name-cell a {
    color: #1a1a2e;
    text-decoration: none;
    font-weight: 500;
    transition: color 0.2s;
  }
  .name-cell a:hover { color: #0f3460; }
  .icon {
    width: 28px;
    height: 28px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 6px;
    font-size: 1rem;
    flex-shrink: 0;
  }
  .icon-folder { background: #fff3e0; color: #f57c00; }
  .icon-file { background: #e3f2fd; color: #1976d2; }
  .icon-image { background: #fce4ec; color: #e91e63; }
  .icon-pdf { background: #ffebee; color: #d32f2f; }
  .icon-code { background: #e8f5e9; color: #388e3c; }
  .icon-doc { background: #e8eaf6; color: #3f51b5; }
  .icon-archive { background: #fff8e1; color: #f9a825; }
  .icon-video { background: #f3e5f5; color: #7b1fa2; }
  .icon-audio { background: #e0f7fa; color: #00838f; }
  .size-cell { color: #888; white-space: nowrap; }
  .date-cell { color: #888; white-space: nowrap; }
  .empty-msg {
    text-align: center;
    padding: 48px 16px;
    color: #aaa;
    font-size: 1rem;
  }
  @media (max-width: 768px) {
    .header, .breadcrumb, .toolbar, .info-bar, .container { padding-left: 16px; padding-right: 16px; }
    .date-cell { display: none; }
    thead th:last-child { display: none; }
  }
</style>
</head>
<body>
  <div class="header">
    <h1>CIH - Gerenciador de Arquivos</h1>
    <div class="subtitle">Navegador de arquivos do workspace</div>
  </div>
  $breadcrumb
  <div class="toolbar">
    $up_button
  </div>
  <div class="info-bar">$info</div>
  <div class="container">
    <table>
      <thead>
        <tr>
          <th>Nome</th>
          <th>Tamanho</th>
          <th>Data de Modifica\u00e7\u00e3o</th>
        </tr>
      </thead>
      <tbody>
        $rows
      </tbody>
    </table>
  </div>
</body>
</html>""")


def format_size(size_bytes):
    if size_bytes < 1024:
        return f"{size_bytes} B"
    elif size_bytes < 1024 * 1024:
        return f"{size_bytes / 1024:.1f} KB"
    elif size_bytes < 1024 * 1024 * 1024:
        return f"{size_bytes / (1024 * 1024):.1f} MB"
    else:
        return f"{size_bytes / (1024 * 1024 * 1024):.2f} GB"


def get_icon_class(name, is_dir):
    if is_dir:
        return "icon-folder", "\U0001f4c1"
    ext = os.path.splitext(name)[1].lower()
    if ext in (".png", ".jpg", ".jpeg", ".gif", ".bmp", ".svg", ".webp", ".ico"):
        return "icon-image", "\U0001f5bc"
    if ext in (".pdf",):
        return "icon-pdf", "\U0001f4c4"
    if ext in (".py", ".js", ".html", ".css", ".json", ".xml", ".yaml", ".yml", ".md", ".sh", ".bat", ".toml", ".cfg", ".ini", ".sql", ".tsx", ".ts", ".jsx"):
        return "icon-code", "\U0001f4dd"
    if ext in (".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".ods", ".odp", ".txt", ".csv"):
        return "icon-doc", "\U0001f4c3"
    if ext in (".zip", ".tar", ".gz", ".bz2", ".rar", ".7z", ".xz"):
        return "icon-archive", "\U0001f4e6"
    if ext in (".mp4", ".avi", ".mkv", ".mov", ".wmv", ".webm"):
        return "icon-video", "\U0001f3ac"
    if ext in (".mp3", ".wav", ".ogg", ".flac", ".aac", ".m4a"):
        return "icon-audio", "\U0001f3b5"
    return "icon-file", "\U0001f4c4"


def safe_path(requested_path):
    """Resolve and validate the path is within BASE_DIR."""
    requested_path = requested_path.strip("/")
    full = os.path.realpath(os.path.join(BASE_DIR, requested_path))
    if not full.startswith(os.path.realpath(BASE_DIR)):
        return None
    return full


class FileManagerHandler(http.server.BaseHTTPRequestHandler):
    def log_message(self, format, *args):
        pass  # suppress logs

    def do_GET(self):
        parsed = urllib.parse.urlparse(self.path)
        req_path = urllib.parse.unquote(parsed.path).strip("/")

        full_path = safe_path(req_path)
        if full_path is None:
            self.send_error(403, "Acesso negado")
            return

        if os.path.isfile(full_path):
            self.serve_file(full_path)
            return

        if not os.path.isdir(full_path):
            self.send_error(404, "N\u00e3o encontrado")
            return

        self.serve_directory(req_path, full_path)

    def serve_file(self, full_path):
        try:
            mime, _ = mimetypes.guess_type(full_path)
            if mime is None:
                mime = "application/octet-stream"
            stat = os.stat(full_path)
            fname = os.path.basename(full_path)
            self.send_response(200)
            self.send_header("Content-Type", mime)
            self.send_header("Content-Length", str(stat.st_size))
            self.send_header("Content-Disposition",
                             'attachment; filename="{}"'.format(urllib.parse.quote(fname)))
            self.end_headers()
            with open(full_path, "rb") as f:
                while True:
                    chunk = f.read(65536)
                    if not chunk:
                        break
                    self.wfile.write(chunk)
        except Exception:
            self.send_error(500, "Erro ao ler arquivo")

    def serve_directory(self, req_path, full_path):
        try:
            entries = os.listdir(full_path)
        except PermissionError:
            self.send_error(403, "Permiss\u00e3o negada")
            return

        # Separate dirs and files, sort each
        dirs = []
        files = []
        for name in entries:
            if name.startswith("."):
                continue
            entry_path = os.path.join(full_path, name)
            try:
                stat = os.stat(entry_path)
            except OSError:
                continue
            is_dir = os.path.isdir(entry_path)
            mod_time = datetime.fromtimestamp(stat.st_mtime).strftime("%d/%m/%Y %H:%M")
            size = stat.st_size if not is_dir else 0
            entry = {
                "name": name,
                "is_dir": is_dir,
                "size": size,
                "mod_time": mod_time,
            }
            if is_dir:
                dirs.append(entry)
            else:
                files.append(entry)

        dirs.sort(key=lambda x: x["name"].lower())
        files.sort(key=lambda x: x["name"].lower())
        all_entries = dirs + files

        # Build breadcrumb
        parts = [p for p in req_path.split("/") if p]
        bc_items = ['<a href="/">cih</a>']
        accumulated = ""
        for p in parts:
            accumulated += "/" + p
            bc_items.append('<span class="sep">/</span><a href="{}">{}</a>'.format(
                html.escape(accumulated), html.escape(p)))
        breadcrumb_html = '<div class="breadcrumb">' + "".join(bc_items) + "</div>"

        # Up button
        if req_path and req_path != "":
            parent = "/".join(parts[:-1]) if len(parts) > 1 else ""
            up_html = '<a class="btn-up" href="/{}">&#x2B06; Voltar</a>'.format(parent)
        else:
            up_html = '<span class="btn-up disabled">&#x2B06; Voltar</span>'

        # Info
        n_dirs = len(dirs)
        n_files = len(files)
        info_parts = []
        if n_dirs:
            info_parts.append("{} pasta{}".format(n_dirs, "s" if n_dirs != 1 else ""))
        if n_files:
            info_parts.append("{} arquivo{}".format(n_files, "s" if n_files != 1 else ""))
        info_text = " &bull; ".join(info_parts) if info_parts else "Pasta vazia"

        # Table rows
        if not all_entries:
            rows_html = '<tr><td colspan="3" class="empty-msg">Esta pasta est\u00e1 vazia</td></tr>'
        else:
            rows = []
            for entry in all_entries:
                name = entry["name"]
                is_dir = entry["is_dir"]
                icon_cls, icon_char = get_icon_class(name, is_dir)

                if is_dir:
                    link_path = ("/" + req_path + "/" + name).replace("//", "/")
                    link = '<a href="{}">{}</a>'.format(
                        html.escape(link_path), html.escape(name))
                    size_str = "&mdash;"
                else:
                    link_path = ("/" + req_path + "/" + name).replace("//", "/")
                    link = '<a href="{}">{}</a>'.format(
                        html.escape(link_path), html.escape(name))
                    size_str = format_size(entry["size"])

                row = ('<tr>'
                       '<td><div class="name-cell">'
                       '<span class="icon {}">{}</span>{}</div></td>'
                       '<td class="size-cell">{}</td>'
                       '<td class="date-cell">{}</td>'
                       '</tr>').format(icon_cls, icon_char, link, size_str, entry["mod_time"])
                rows.append(row)
            rows_html = "\n".join(rows)

        page = HTML_TEMPLATE.substitute(
            breadcrumb=breadcrumb_html,
            up_button=up_html,
            info=info_text,
            rows=rows_html,
        )
        data = page.encode("utf-8")
        self.send_response(200)
        self.send_header("Content-Type", "text/html; charset=utf-8")
        self.send_header("Content-Length", str(len(data)))
        self.end_headers()
        self.wfile.write(data)


if __name__ == "__main__":
    server = http.server.HTTPServer(("0.0.0.0", PORT), FileManagerHandler)
    print("Servidor iniciado em http://0.0.0.0:{}/".format(PORT))
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        print("\nServidor encerrado.")
        server.server_close()
