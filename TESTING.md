# Testing

## Offline (l10n -> preTest)

Written 2026-09-29, `Tests/test_resources.py`, model ManureComposting's own `Tests/test_resources.py`.
Run: `uv run --with pillow python Tests/test_resources.py` (pillow unused here, kept for a
uniform invocation across mods). 93 checks, all green:

- All distributed XML parses; EN/FR Keyed coverage identical, `{0}`/`{1}` parameters match;
  every `"NeverOutOfPrint_*".Translate()` call in Source/ resolves in both languages.
- The three `IDEO_*` grammar symbols `BookOutcomeDoer_Ideoligion.GetTopicRuleStrings` actually
  supplies match, set for set, the ones `RulePacks.xml` uses — a mismatch here fails silently
  in-game (an unresolved symbol in a title/blurb), so this is the one check that would not be
  caught by `Check-XmlClasses.ps1`/`Check-DefRefs.ps1`.
- Recipe/def wiring: the copy recipe's `recipeUsers` are exactly the two presses, both declared
  ThingDefs, both requiring `NeverOutOfPrint_Printing`; the ideoligion book's `nameMaker`/
  `descriptionMaker` resolve to `RulePackDef`s this mod actually declares.
- Stockpile/reading-policy filter `workerClass`es are defined in this mod's own `Source/` (not
  just anywhere on the load order, which is all `Check-XmlClasses.ps1` proves).
- Packaging: `About.xml` description/url, `ModIcon.png`/`Preview.png` PNG signature+dimensions+
  weight, `LICENSE`/`ATTRIBUTION.md` byte-identical at repo root and inside `Mod/` (this
  surfaced a real gap — neither file existed in `Mod/` before this pass, fixed in the same
  commit per `PUBLISHING.md`'s "Fichier LICENSE ... et dans le dossier publié"), no dependency
  DLL redistributed, no `MainButtonDef`/settings class (`settings_audit: not_applicable`).

Does not prove: anything requiring a running game (bill execution, book generation, reading
outcomes) — that is the Pickle suite below.

## Pickle suites (done -> tested)

Not yet written, and AUDIT.md requires either a suite or a written justification for its
absence — neither exists yet, which is itself one of the reasons `stage` has not reached
`preTest`.

Scope to write, once started: at minimum a bill-lifecycle scenario (build a press, queue a copy
bill, confirm the original survives and a marked copy appears with the same skill/research
grants and a tenth of the market value), and an ideoligion-book scenario (certainty moves the
documented direction for a believer and a rival, an awful-quality book pushes the opposite way).

## Passes

Not yet decided. Per AUDIT.md, at minimum: one pass without optional mods (Core + DLC + Harmony +
RimLogging + Pickle + this mod), one pass with `loadAfter` mods present (notably Vanilla Books
Expanded), and — since none of the mods found in the duplicate-coverage search
(`BACKLOG.md`) declare an incompatibility with this one, and nothing here declares one with them
either — no incompatibility pass is owed yet. Revisit if that changes.

## Evidence kept

Per AGENTS.md, "Test evidence": latest report per scenario on disk, out of git
(`Tests/Pickle/Evidence/`, gitignored); a short dated summary in `docs/runs/` is what is
committed.
