"""Offline packaging, wiring and localization checks. Reads only; touches nothing.
Model: ManureComposting/Tests/test_resources.py. Run: uv run --with pillow python Tests/test_resources.py
(pillow is unused here but keeps the invocation identical across mods that do need it)."""
from pathlib import Path
import re, struct, xml.etree.ElementTree as ET
ROOT = Path(__file__).resolve().parents[1]
checks = 0


def check(ok, message):
    global checks
    assert ok, message
    checks += 1
    print("PASS", message)


def resources(folder):
    result = {}
    for f in folder.rglob("*.xml"):
        for e in ET.parse(f).getroot():
            if not isinstance(e.tag, str):
                continue
            check(e.tag not in result, f"unique resource {e.tag}")
            check(bool(e.text and e.text.strip()), f"nonempty {e.tag}")
            result[e.tag] = e.text
    return result


def defs_by_tag(folder, tag):
    result = {}
    for f in folder.rglob("*.xml"):
        for d in ET.parse(f).getroot():
            if d.tag == tag and d.findtext("defName"):
                result[d.findtext("defName")] = d
    return result


for f in (ROOT / "Mod").rglob("*.xml"):
    ET.parse(f)
print("PASS all distributed XML parses")

# --- Localization: EN/FR Keyed coverage and parameter parity, every source Translate() key
# resolves in both languages. This mod carries no DefInjected translations (no def field is
# translated; About.xml states EN/FR coverage as Keyed strings and IDEO_ grammar symbols only).
en = resources(ROOT / "Mod/Languages/English/Keyed")
fr = resources(ROOT / "Mod/Languages/French/Keyed")
check(set(en) == set(fr), "Keyed languages have identical coverage")
# A {PAWN_gender ? m : f : n} switch (TRANSLATIONS.md, "French gender agreement", 2026-09-30) is
# a French-only addition with no English counterpart by design - strip it before the {0}/{1}
# positional-parameter parity check, or a legitimate gender switch reads as a missing/extra param.
gender_switch = re.compile(r"\{\w+_gender\s*\?[^{}]*\}")
for key in en:
    check(re.findall(r"\{[^{}]+\}", en[key]) == re.findall(r"\{[^{}]+\}", gender_switch.sub("", fr[key])), f"parameters match {key}")
for f in (ROOT / "Source").glob("*.cs"):
    for key in re.findall(r'"(NeverOutOfPrint_[^"]+)"\.Translate', f.read_text(encoding="utf-8-sig")):
        check(key in en and key in fr, f"source key resolves {key}")

# --- IDEO_ grammar symbols: BookOutcomeDoer_Ideoligion.GetTopicRuleStrings supplies these at
# generation time (IDEO_name, IDEO_memberName, IDEO_memberNamePlural); the RulePackDefs must
# reference exactly the symbols the doer actually yields, or a book title/blurb fails to resolve
# silently in-game. Cross-check by static text search rather than running the grammar resolver.
doer_source = (ROOT / "Source/BookOutcomeDoer_Ideoligion.cs").read_text(encoding="utf-8-sig")
supplied_symbols = set(re.findall(r'new Rule_String\("([A-Za-z_]+)"', doer_source))
check(supplied_symbols == {"IDEO_name", "IDEO_memberName", "IDEO_memberNamePlural"}, "doer supplies exactly the three IDEO_ symbols")
rule_packs_xml = (ROOT / "Mod/Defs/RulePacks.xml").read_text(encoding="utf-8-sig")
used_symbols = set(re.findall(r"\[(IDEO_[A-Za-z]+)\]", rule_packs_xml))
check(used_symbols <= supplied_symbols, f"every [IDEO_*] used in RulePacks.xml is supplied by the doer (used: {used_symbols})")
check(len(used_symbols) == 3, "RulePacks.xml actually uses all three IDEO_ symbols, not a subset")

# --- Recipe/comp/def wiring: the recipe's declared users are real ThingDefs that exist and
# carry a printing research prerequisite; the ideoligion book's nameMaker/descriptionMaker
# RulePackDef references resolve to defs this mod actually declares.
things = defs_by_tag(ROOT / "Mod/Defs", "ThingDef")
recipes = defs_by_tag(ROOT / "Mod/Defs", "RecipeDef")
rule_packs = defs_by_tag(ROOT / "Mod/Defs", "RulePackDef")
# NeverOutOfPrint_CopyBookBase is Abstract="True", so it carries a Name attribute (for
# inheritance) rather than a <defName> child - defs_by_tag only indexes by defName.
copy_base = next(d for f in (ROOT / "Mod/Defs").rglob("*.xml") for d in ET.parse(f).getroot() if d.tag == "RecipeDef" and d.get("Name") == "NeverOutOfPrint_CopyBookBase")
check(copy_base is not None, "NeverOutOfPrint_CopyBookBase recipe exists")
users = [li.text for li in copy_base.findall("recipeUsers/li")]
check(set(users) == {"NeverOutOfPrint_Manual", "NeverOutOfPrint_Electric"}, "copy recipe's users are exactly the two presses")
for press in users:
    check(press in things, f"recipe user {press} is a declared ThingDef")
    prereqs = [li.text for li in things[press].findall("researchPrerequisites/li")]
    check("NeverOutOfPrint_Printing" in prereqs, f"{press} requires the printing research")
book = things.get("NeverOutOfPrint_IdeoligionBook")
check(book is not None, "NeverOutOfPrint_IdeoligionBook ThingDef exists")
name_maker = book.findtext("comps/li/nameMaker")
desc_maker = book.findtext("comps/li/descriptionMaker")
check(name_maker in rule_packs, f"ideoligion book's nameMaker {name_maker!r} is a declared RulePackDef")
check(desc_maker in rule_packs, f"ideoligion book's descriptionMaker {desc_maker!r} is a declared RulePackDef")
check(things["NeverOutOfPrint_Manual"].findtext("thingClass") == "Building_WorkTable", "manual press uses the vanilla work-table class")
check(book.findtext("thingClass") == "NeverOutOfPrint.IdeoligionBook", "ideoligion book ThingDef points at the mod's own Book subclass")

# --- Research: the printing project the two presses gate on actually exists (Patches/
# VanillaBooksExpanded.xml removes it when VBE is loaded, so this only proves the base-game
# case; the compat patch's own XPath is validated by Check-DefRefs.ps1 with -AlsoScan).
research = defs_by_tag(ROOT / "Mod/Defs", "ResearchProjectDef")
check("NeverOutOfPrint_Printing" in research, "NeverOutOfPrint_Printing research project is declared")

# --- Stockpile filters and the reading-effects category the mod adds resolve to the C# workers
# this repository ships (not just any string - Check-XmlClasses.ps1 already proves the class
# exists somewhere on the load order; this proves it exists in *this* mod's own Source/).
filters_xml = (ROOT / "Mod/Defs/SpecialThingFilters.xml").read_text(encoding="utf-8-sig")
source_text = "\n".join(f.read_text(encoding="utf-8-sig") for f in (ROOT / "Source").glob("*.cs"))
for worker_class in re.findall(r"<workerClass>NeverOutOfPrint\.(\w+)</workerClass>", filters_xml):
    check(re.search(rf"\bclass {worker_class}\b", source_text) is not None, f"filter worker class {worker_class} is defined in Source/")

# --- Packaging: About.xml, ModIcon/Preview dimensions and weight, LICENSE/ATTRIBUTION shipped
# both at repo root and inside Mod/ (PUBLISHING.md, "Fichier LICENSE ... et dans le dossier
# publié"), no dependency DLL redistributed, no settings class or MainButtonDef (settings_audit:
# not_applicable).
about = ET.parse(ROOT / "Mod/About/About.xml").getroot()
url = "https://github.com/vbardales/Rimworld-Never-Out-Of-Print"
check(about.findtext("description").rstrip().endswith(f"[url={url}]Source code on GitHub[/url]"), "description ends with exact repository link")
check(about.findtext("url") == url, "About url matches description")
for filename, size in [("ModIcon.png", (128, 128)), ("Preview.png", (896, 504))]:
    data = (ROOT / "Mod/About" / filename).read_bytes()
    check(data[:8] == b"\x89PNG\r\n\x1a\n", f"{filename} PNG signature")
    check(struct.unpack(">II", data[16:24]) == size, f"{filename} dimensions")
    check(len(data) < 1000000, f"{filename} below 1 MB")
for name in ["LICENSE", "ATTRIBUTION.md"]:
    check((ROOT / name).read_bytes() == (ROOT / "Mod" / name).read_bytes(), f"distributed {name} matches")
check([p.name for p in (ROOT / "Mod/Assemblies").glob("*.dll")] == ["NeverOutOfPrint.dll"], "no dependency DLL redistribution")
distributed_xml = "\n".join(f.read_text(encoding="utf-8-sig") for f in (ROOT / "Mod").rglob("*.xml"))
check("MainButtonDef" not in distributed_xml, "no MainButtonDef in the distributed XML")
check(not re.search(r"\bModSettings\b|:\s*Mod\b|DoSettingsWindowContents|SettingsCategory", source_text), "no settings class or settings window in the source")
check(about.find("incompatibleWith") is None, "no incompatibility is declared, so no incompatibility pass is owed")

print(f"{checks} offline packaging, wiring and localization checks passed; no in-game display claim.")
