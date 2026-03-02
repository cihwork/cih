#!/usr/bin/env python3
"""
Gera arquivo Markdown com todas as informações da planilha PROSPECAO_RH_COMPLETA_CIH.xlsx
356 empresas (39 originais + 317 concorrentes) organizadas por setor.
"""
import csv
from collections import defaultdict

BASE = "/home/headless/workspace/cih/clientes/pesquisacliente"

# ============================================================
# 1. READ ALL DATA
# ============================================================
rows = []
with open(f"{BASE}/db_consolidado_completo.csv", "r", encoding="utf-8") as f:
    reader = csv.DictReader(f)
    for row in reader:
        rows.append(row)

print(f"Total de registros lidos: {len(rows)}")

# ============================================================
# 2. CLASSIFY SECTOR
# ============================================================
def classify_setor(grupo, segmento):
    grupo_lower = grupo.lower()
    seg_lower = segmento.lower() if segmento else ""

    if "petroquimica" in grupo_lower or "petroquimica" in seg_lower or "quimic" in seg_lower:
        return "Petroquimica e Quimica"
    elif "celulose" in grupo_lower or "papel" in grupo_lower or "celulose" in seg_lower or "papel" in seg_lower:
        return "Celulose e Papel"
    elif "fertiliz" in grupo_lower or "fertiliz" in seg_lower:
        return "Fertilizantes"
    elif "refrat" in grupo_lower or "refrat" in seg_lower:
        return "Refratarios"
    elif "mineracao" in grupo_lower or "metalurg" in grupo_lower or "ferro" in seg_lower or "sider" in seg_lower or "miner" in seg_lower or "niob" in seg_lower or "silic" in seg_lower:
        return "Mineracao e Metalurgia"
    elif "energia" in grupo_lower or "energia" in seg_lower or "eletric" in seg_lower:
        return "Energia"
    elif "saneamento" in grupo_lower or "saneamento" in seg_lower:
        return "Saneamento"
    elif "saude" in grupo_lower or "hospital" in seg_lower or "diagnostic" in seg_lower or "plano" in seg_lower or "medic" in seg_lower or "seguro saude" in seg_lower or "operadora" in seg_lower or "autogestao" in seg_lower or "oncologia" in seg_lower or "cardiologia" in seg_lower or "filantropico" in seg_lower or "pediatrico" in seg_lower:
        return "Saude"
    elif "educacao" in grupo_lower or "universidade" in seg_lower or "educacao" in seg_lower:
        return "Educacao"
    elif "varejo" in grupo_lower or "atacado" in grupo_lower or "supermerc" in seg_lower or "atacad" in seg_lower or "hipermercado" in seg_lower:
        return "Varejo e Atacado"
    elif "moveis" in grupo_lower or "movel" in seg_lower or "moveis" in seg_lower or "eletro" in seg_lower or "departamento" in seg_lower or "decoracao" in seg_lower:
        return "Varejo Moveis"
    elif "moda" in grupo_lower or "fitness" in seg_lower or "vestuar" in seg_lower or "moda" in seg_lower or "activewear" in seg_lower or "sportswear" in seg_lower:
        return "Moda e Vestuario"
    elif "alimento" in grupo_lower or "suplemento" in grupo_lower or "nutri" in seg_lower or "suplemento" in seg_lower or "cafe" in seg_lower or "barras" in seg_lower:
        return "Alimentos e Suplementos"
    elif "franquia" in grupo_lower or "acai" in seg_lower or "franquia" in seg_lower or "acaiteria" in seg_lower:
        return "Franquias Alimentacao"
    elif "transporte" in grupo_lower or "transporte" in seg_lower or "onibus" in seg_lower or "rodoviario" in seg_lower:
        return "Transporte"
    elif "textil" in grupo_lower or "textil" in seg_lower or "linhas" in seg_lower or "fio" in seg_lower or "tecido" in seg_lower or "fiacao" in seg_lower or "tecelagem" in seg_lower or "denim" in seg_lower or "tissue" in seg_lower:
        return "Industria Textil"
    elif "lawtech" in grupo_lower or "juridic" in seg_lower or "lawtech" in seg_lower:
        return "Tecnologia - Lawtech"
    elif "inovacao" in grupo_lower or "aceleradora" in seg_lower or "inovacao" in seg_lower or "hub" in seg_lower:
        return "Tecnologia - Inovacao"
    elif "tech" in grupo_lower or "tecnologia" in grupo_lower or "consultoria" in seg_lower or "digital" in seg_lower:
        return "Tecnologia"
    elif "agricola" in grupo_lower or "agricol" in seg_lower or "insumos" in seg_lower:
        return "Distribuicao Agricola"
    elif grupo in ["INDUSTRIA"]:
        if "refinaria" in seg_lower or "petroleo" in seg_lower:
            return "Petroquimica e Quimica"
        elif "celulose" in seg_lower:
            return "Celulose e Papel"
        return "Industria (Geral)"
    elif grupo in ["COMERCIO"]:
        if "farmacia" in seg_lower or "drogaria" in seg_lower:
            return "Varejo e Atacado"
        elif "cooperativa" in seg_lower:
            return "Distribuicao Agricola"
        elif "combustiv" in seg_lower:
            return "Energia"
        return "Varejo e Atacado"
    elif grupo in ["SERVICOS"]:
        if "saude" in seg_lower or "hospital" in seg_lower or "diagnostic" in seg_lower:
            return "Saude"
        elif "educacao" in seg_lower or "universidade" in seg_lower:
            return "Educacao"
        elif "transporte" in seg_lower:
            return "Transporte"
        return "Servicos"
    elif grupo in ["TECNOLOGIA"]:
        return "Tecnologia"
    else:
        return "Outros"

def is_original(grupo):
    g = grupo.upper()
    return g in ["INDUSTRIA", "COMERCIO", "SERVICOS", "TECNOLOGIA"]

def has_gestor(gestor):
    if not gestor:
        return False
    g = gestor.strip().lower()
    if not g or g.startswith("nao identificado") or g.startswith("ver bloco") or g == "contato direto" or g.startswith("gerente de gente"):
        return False
    return True

def clean_gestor(gestor):
    if not gestor:
        return ""
    g = gestor.strip()
    if g.lower().startswith("nao identificado"):
        # Extract parenthetical info
        if "(" in g and ")" in g:
            info = g[g.index("(")+1:g.rindex(")")]
            return f"(Possivel contato: {info})"
        return "Nao identificado"
    if g.lower().startswith("ver bloco"):
        return g
    return g

# ============================================================
# 3. ORGANIZE DATA
# ============================================================
originais = []
concorrentes = []
by_setor = defaultdict(list)
com_gestor = []

for r in rows:
    grupo = r.get("GRUPO", "")
    segmento = r.get("SEGMENTO", "")
    setor = classify_setor(grupo, segmento)
    r["_SETOR"] = setor
    r["_ORIGINAL"] = is_original(grupo)
    r["_TEM_GESTOR"] = has_gestor(r.get("GESTOR_RH", ""))

    if r["_ORIGINAL"]:
        originais.append(r)
    else:
        concorrentes.append(r)

    by_setor[setor].append(r)

    if r["_TEM_GESTOR"]:
        com_gestor.append(r)

# Sort sectors by count
setores_sorted = sorted(by_setor.keys(), key=lambda s: -len(by_setor[s]))

# ============================================================
# 4. GENERATE MARKDOWN
# ============================================================
md = []

# Title
md.append("# PROSPECAO RH COMPLETA - CIH")
md.append("")
md.append("**Base de dados para prospecao de clientes - Equipe de Atendimento**")
md.append("")
md.append(f"**Total de empresas:** {len(rows)}")
md.append(f"**Empresas originais (mapeamento):** {len(originais)}")
md.append(f"**Empresas concorrentes:** {len(concorrentes)}")
md.append(f"**Com gestor de RH identificado:** {len(com_gestor)}")
md.append(f"**Setores mapeados:** {len(setores_sorted)}")
md.append("")
md.append("---")
md.append("")

# Summary table
md.append("## RESUMO POR SETOR")
md.append("")
md.append("| Setor | Total | Originais | Concorrentes | Com Gestor RH |")
md.append("|-------|-------|-----------|-------------|---------------|")
for setor in setores_sorted:
    empresas = by_setor[setor]
    n_orig = sum(1 for e in empresas if e["_ORIGINAL"])
    n_conc = sum(1 for e in empresas if not e["_ORIGINAL"])
    n_gest = sum(1 for e in empresas if e["_TEM_GESTOR"])
    md.append(f"| {setor} | {len(empresas)} | {n_orig} | {n_conc} | {n_gest} |")
total_orig = len(originais)
total_conc = len(concorrentes)
total_gest = len(com_gestor)
md.append(f"| **TOTAL** | **{len(rows)}** | **{total_orig}** | **{total_conc}** | **{total_gest}** |")
md.append("")
md.append("---")
md.append("")

# Original companies section
md.append("## EMPRESAS ORIGINAIS (MAPEAMENTO)")
md.append("")
md.append(f"*{len(originais)} empresas da lista original de prospecao na Bahia*")
md.append("")

for i, r in enumerate(originais, 1):
    empresa = r.get("EMPRESA", "")
    segmento = r.get("SEGMENTO", "")
    cidade = r.get("CIDADE_UF", "")
    gptw = r.get("GPTW", "")
    status = r.get("STATUS_MAPEAMENTO", "")
    site = r.get("SITE_OFICIAL", "")
    linkedin = r.get("LINKEDIN_EMPRESA", "")
    gestor = r.get("GESTOR_RH", "")
    cargo = r.get("CARGO_RH", "")
    linkedin_g = r.get("LINKEDIN_GESTOR_RH", "")
    email = r.get("EMAIL_CONTATO", "")
    telefone = r.get("TELEFONE", "")
    portal = r.get("PORTAL_VAGAS", "")
    obs = r.get("OBSERVACOES", "")
    setor = r.get("_SETOR", "")

    md.append(f"### {i}. {empresa}")
    md.append("")
    md.append(f"- **Setor:** {setor}")
    md.append(f"- **Segmento:** {segmento}")
    md.append(f"- **Cidade/UF:** {cidade}")
    if gptw:
        md.append(f"- **GPTW:** {gptw}")
    if status:
        md.append(f"- **Status:** {status}")
    if site and site != "Nao encontrado":
        md.append(f"- **Site:** {site}")
    if linkedin and linkedin != "Nao encontrado":
        md.append(f"- **LinkedIn:** {linkedin}")

    gestor_clean = clean_gestor(gestor)
    if gestor_clean:
        md.append(f"- **Gestor RH:** {gestor_clean}")
    if cargo:
        md.append(f"- **Cargo:** {cargo}")
    if linkedin_g:
        md.append(f"- **LinkedIn Gestor:** {linkedin_g}")
    if email:
        md.append(f"- **Email:** {email}")
    if telefone:
        md.append(f"- **Telefone:** {telefone}")
    if portal:
        md.append(f"- **Portal Vagas:** {portal}")
    if obs:
        md.append(f"- **Obs:** {obs}")
    md.append("")

md.append("---")
md.append("")

# Companies with identified HR manager
md.append("## EMPRESAS COM GESTOR DE RH IDENTIFICADO")
md.append("")
md.append(f"*{len(com_gestor)} empresas com nome do gestor de RH/Pessoas identificado*")
md.append("")
md.append("| # | Empresa | Setor | Gestor RH | Cargo | Tipo |")
md.append("|---|---------|-------|-----------|-------|------|")
for i, r in enumerate(com_gestor, 1):
    empresa = r.get("EMPRESA", "")
    setor = r.get("_SETOR", "")
    gestor = r.get("GESTOR_RH", "").replace("|", "/")
    cargo = r.get("CARGO_RH", "").replace("|", "/")
    tipo = "Original" if r["_ORIGINAL"] else "Concorrente"
    md.append(f"| {i} | {empresa} | {setor} | {gestor} | {cargo} | {tipo} |")
md.append("")
md.append("---")
md.append("")

# Sections by sector
md.append("## EMPRESAS POR SETOR")
md.append("")

for setor in setores_sorted:
    empresas = by_setor[setor]
    n_orig = sum(1 for e in empresas if e["_ORIGINAL"])
    n_conc = sum(1 for e in empresas if not e["_ORIGINAL"])
    n_gest = sum(1 for e in empresas if e["_TEM_GESTOR"])

    md.append(f"### {setor}")
    md.append("")
    md.append(f"*{len(empresas)} empresas ({n_orig} originais, {n_conc} concorrentes) | {n_gest} com gestor RH identificado*")
    md.append("")

    # Separate originals and competitors
    orig_setor = [e for e in empresas if e["_ORIGINAL"]]
    conc_setor = [e for e in empresas if not e["_ORIGINAL"]]

    if orig_setor:
        md.append("**Empresas Originais:**")
        md.append("")
        for r in orig_setor:
            empresa = r.get("EMPRESA", "")
            cidade = r.get("CIDADE_UF", "")
            gestor = clean_gestor(r.get("GESTOR_RH", ""))
            cargo = r.get("CARGO_RH", "")
            site = r.get("SITE_OFICIAL", "")
            linkedin = r.get("LINKEDIN_EMPRESA", "")
            email = r.get("EMAIL_CONTATO", "")
            telefone = r.get("TELEFONE", "")
            portal = r.get("PORTAL_VAGAS", "")
            obs = r.get("OBSERVACOES", "")
            gptw = r.get("GPTW", "")

            gestor_str = f" | **{gestor}** - {cargo}" if gestor and gestor != "Nao identificado" else ""
            gptw_str = f" | GPTW: {gptw}" if gptw and gptw.lower() != "nao" else ""

            md.append(f"- **{empresa}** ({cidade}){gestor_str}{gptw_str}")

            details = []
            if site and site != "Nao encontrado":
                details.append(f"Site: {site}")
            if linkedin and linkedin != "Nao encontrado":
                details.append(f"LinkedIn: {linkedin}")
            if email:
                details.append(f"Email: {email}")
            if telefone:
                details.append(f"Tel: {telefone}")
            if portal:
                details.append(f"Vagas: {portal}")
            if obs:
                details.append(f"Obs: {obs}")

            if details:
                for d in details:
                    md.append(f"  - {d}")
        md.append("")

    if conc_setor:
        md.append("**Concorrentes:**")
        md.append("")

        # Deduplicate by company name for display
        seen = set()
        for r in conc_setor:
            empresa = r.get("EMPRESA", "")
            key = empresa.lower().strip()
            if key in seen:
                continue
            seen.add(key)

            cidade = r.get("CIDADE_UF", "")
            segmento = r.get("SEGMENTO", "")
            gestor = clean_gestor(r.get("GESTOR_RH", ""))
            cargo = r.get("CARGO_RH", "")
            site = r.get("SITE_OFICIAL", "")
            linkedin = r.get("LINKEDIN_EMPRESA", "")
            email = r.get("EMAIL_CONTATO", "")
            telefone = r.get("TELEFONE", "")
            portal = r.get("PORTAL_VAGAS", "")
            obs = r.get("OBSERVACOES", "")

            gestor_str = f" | **{gestor}** - {cargo}" if gestor and gestor != "Nao identificado" and not gestor.startswith("Ver BLOCO") else ""

            md.append(f"- **{empresa}** ({cidade}){gestor_str}")

            details = []
            if segmento:
                details.append(f"Segmento: {segmento}")
            if site and site not in ["Nao encontrado", "Não localizado", "Nao localizado", ""]:
                details.append(f"Site: {site}")
            if linkedin and linkedin not in ["Nao encontrado", "Não identificado", "Nao identificado", ""]:
                details.append(f"LinkedIn: {linkedin}")
            if email:
                details.append(f"Email: {email}")
            if telefone:
                details.append(f"Tel: {telefone}")
            if portal and portal not in ["Verificar site", ""]:
                details.append(f"Vagas: {portal}")
            if obs:
                # Trim long obs
                obs_clean = obs[:200] + "..." if len(obs) > 200 else obs
                details.append(f"Obs: {obs_clean}")

            if details:
                for d in details:
                    md.append(f"  - {d}")
        md.append("")

    md.append("---")
    md.append("")

# Footer
md.append("## NOTAS")
md.append("")
md.append("- **Empresas Originais**: 39 empresas do mapeamento inicial na Bahia (grupos INDUSTRIA, COMERCIO, SERVICOS, TECNOLOGIA)")
md.append("- **Concorrentes**: Empresas do mesmo setor em todo o Brasil, pesquisadas como potenciais clientes")
md.append("- **GPTW**: Great Place to Work - certificacao de qualidade do ambiente de trabalho")
md.append("- **Gestor RH**: Nome do responsavel pela area de Recursos Humanos / Gente e Gestao identificado")
md.append("- Dados coletados em fevereiro/2026 - verificar atualizacoes periodicamente")
md.append("")
md.append("---")
md.append("")
md.append("*Arquivo gerado automaticamente a partir de db_consolidado_completo.csv (356 registros)*")

# Write file
output = "\n".join(md)
output_path = f"{BASE}/PROSPECAO_RH_COMPLETA_CIH.md"
with open(output_path, "w", encoding="utf-8") as f:
    f.write(output)

print(f"\nArquivo gerado: {output_path}")
print(f"Tamanho: {len(output):,} caracteres")
print(f"Linhas: {len(md):,}")
