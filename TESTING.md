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

Written 2026-09-30, `Tests/Pickle/`. Full detail in `Tests/Pickle/README.md`; summary:

- `01-loading.feature` (3 scenarios): the mod loads, the presses/recipe/research def database is
  real, the copy patch reached `Novel` and `MarketValueBase`.
- `02-copying.feature` (1 scenario): a real bill leaves the original on the shelf, marks the
  printed copy's `CompCopiedBook`, and the copy's computed market value lands at the documented
  tenth. Does not use Pickle's built-in `I wait for bill ... to finish` (it polls the recipe's
  *declared* product, `NeverOutOfPrint_CopiedBookPlaceholder`, which this mod's own Harmony patch
  never actually spawns — the same reason the built-in step already refuses a `specialProducts`
  recipe); waits a fixed tick budget and counts `Novel` directly instead.
- `03-ideoligion.feature` (2 scenarios, `@requires:Ludeon.RimWorld.Ideology`): a believer's
  own-faith read raises certainty, a rival-faith read lowers it. Calls
  `BookOutcomeDoer_Ideoligion.OnReadingTick` directly (the method `JobDriver_Reading` itself
  ticks) rather than orchestrating a full reading job through Pickle, which has no built-in step
  for it.
- Custom steps: `Tests/Pickle/Source/Steps.cs`, compiles clean offline (`dotnet build
  Tests/Pickle/Source/NeverOutOfPrint.PickleSteps.csproj`, 0 errors) via `Krafs.Rimworld.Ref` +
  `RimWorks.Pickle.Ref`, same as every other mod's Pickle companion — no game install needed to
  build it, only to run it.
- **Not converted**: the awful-quality-book scenario ("a badly argued copy pushes the opposite
  direction"). No built-in Pickle step to force a specific `QualityCategory` on a freshly
  generated book was found; needs either a custom step writing the comp directly or confirmation
  a vanilla quality debug action applies to books. Left `unverified` in STATUS.md rather than
  guessed at.

**Nothing here has been played** — writing it is this criterion; playing it is `done -> tested`,
filed through the dispatcher, never run by this session directly.

## Passes

Per AUDIT.md, at minimum: one pass without optional mods (Core + Ideology DLC + Harmony +
RimLogging + Pickle + this mod — Ideology is required for `03-ideoligion.feature`), one pass with
`loadAfter` mods present (Vanilla Books Expanded, this mod's only optional compat). Neither has
run. Since none of the mods found in the duplicate-coverage search (`BACKLOG.md`) declare an
incompatibility with this one, and nothing here declares one with them either, no incompatibility
pass is owed. No `wsl-deps.map`/config seed needed: no hard `modDependencies`, no settings.

## Evidence kept

Per AGENTS.md, "Test evidence": latest report per scenario on disk, out of git
(`Tests/Pickle/Evidence/`, gitignored); a short dated summary in `docs/runs/` is what is
committed.
