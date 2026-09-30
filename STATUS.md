---
localization:   complete
translation_en: complete
translation_fr: partial
settings_audit: not_applicable
mod:          Never Out of Print
packageId:    nelim.neveroutofprint
repo:         Rimworld-Never-Out-Of-Print
visibility:   public
detached:     yes
stage:        showcase
workflow_stage: l10n
licence:      open
licence_at:   MIT (Vanilla Books Expanded Expanded, Ben Lubar and Efi, continued by Mlie); own C#, rule packs and translations also MIT
upstream_mod_remotes:
  - https://github.com/emipa606/VanillaBooksExpandedExpanded (archived, MIT)
dependencies: declared
showcase:     blocked
tested_on:
workshop:
remaining:
  - resolved 2026-09-29: ModIcon.png delivered by the owner (1254x1254; unrelated to the unexplained stray already at Art/ModIcon-stray-1254.png). Full-resolution delivered file kept untouched at Art/Icons/ModIcon-source.png; resize to 128x128/20.6 KB done by this session with the owner's explicit per-file accord ("oui, vas-y"). Art/ModIcon.ico is 16/32/48/256, RGBA PNG-encoded (ffmpeg recipe in STYLE_RIMWORLD.md). Root desktop.ini wired (IconResource=Art\ModIcon.ico,0), root folder ReadOnly, its desktop.ini Hidden+System.
  - note: the delivered icon is a generic winking-sun emoji, not a mod-specific mascot holding an accompanying object (book/press). Legible at 32px, so not a blocking defect per STYLE_RIMWORLD.md's own rule — flagged for the owner rather than silently accepted or replaced.
  - resolved 2026-09-29: Mod/About/Preview.png (896x504) is rendered by ../scripts/Render-Preview.cjs (the monorepo-shared script, migrated onto 2026-09-29 from this mod's own now-deleted Art/render-preview.cjs — see history for the compose-preview.cjs → render-preview.cjs → shared-script progression) from Art/preview-copy.json (title "Never Out of Print", no suffix/tag, résumé from About.xml's first sentence) and Art/preview-palette.json (veil from the cool stone floor in the text's calm zone; inkPrimary ivory; inkSecondary the warm amber/brass family of the lamp pool and wood; accent a deep terracotta, split from inkSecondary by saturation/lightness since the source illustration is close to monochromatic). Icon badge: Art/ModIcon-badge.png (flood-filled + alpha-trimmed by ../scripts/Make-PreviewBadge.ps1 -SaveTrimmedIconTo from ModIcon.png), corner bottom-left (same side as the text, per the owner's rule), width 220. Verified by Art/verify-preview.py (patched locally to skip zero-area suffix/tag boxes — the shared script's own bug on an empty suffix/tag, not fixed upstream yet): contrast 10.9:1 (title) / 7.7:1 (résumé) / 4.7:1 (badge), all ≥4.5:1; 518 KB, under 900 KB. Art/Preview.png is the no-overlay source; Art/Preview.ico rebuilt from the final rendered file; Art/Gallery/00-Preview.png is a byte-copy of it (PUBLISHING.md's gallery-starts-with-Preview rule).
  - note: verify-preview.py's zero-area-box bug (KeyError on the final summary line, ValueError on an empty suffix/tag's min() over zero pixels) is only patched in this mod's own Art/verify-preview.py copy, not in the shared tooling — worth fixing at the source once a second no-suffix/no-tag mod hits it, so every future migration doesn't re-discover it.
  - note: RECOMPOSER_PREVIEW.md and this mod's own former Art/compose-preview.cjs were both superseded by the shared ../scripts/Render-Preview.cjs + Make-PreviewBadge.ps1 pattern (generalized 2026-09-29 out of AlphaMythologyRenew, ~50 mods had grown near-identical copies before this); package.json/package-lock.json/node_modules (the earlier per-mod npm-install approach) are gone, playwright now comes from the shared Codex runtime cache.
  - resolved 2026-09-29: workflow_stage advanced horsMonoRepo -> ModIcon générée -> Preview générée -> preOptions -> options -> l10n in one step. Each of preOptions/options/l10n is asserted from source inspection, not a running game (no settings class, no MainButtonDef, all Keyed/DefInjected coverage checked by Check-DefInjected.ps1, 33 keys, 0 errors) — AUDIT.md's "stage jamais bloqué par le jeu" treats that as sufficient to advance, since no defect was found; the in-game exercise itself stays open below, under l10n -> preTest.
  - unverified: l10n -> preTest: modDependencies is empty by design (VBE is an optional compat, not a hard dependency — loadAfter only), which needs to be re-confirmed against MOD_SETTINGS.md/PUBLISHING.md wording once the mod is actually loaded in a game with and without VBE present. Options/l10n themselves also not yet exercised in a running game.
  - resolved 2026-09-29: Tests/test_resources.py written (model: ManureComposting's own), 93 offline checks green — XML parses; EN/FR Keyed coverage and parameters; every `.Translate()` key in Source/ resolves both languages; the three `IDEO_*` grammar symbols the doer supplies match what RulePacks.xml uses; recipe/def wiring (recipeUsers, research prerequisites, nameMaker/descriptionMaker resolve); filter workerClasses defined in this mod's own Source/; About.xml, ModIcon/Preview packaging, LICENSE/ATTRIBUTION.md. That last check surfaced a real gap — neither file existed inside `Mod/` yet, despite PUBLISHING.md requiring both root and published copies — fixed in the same pass. Detail in TESTING.md. No Tests/Pickle/ suite written yet (done -> tested gate, separate from this).
  - unverified: preTest -> done: no Tests/Pickle/ suite written yet, so its absence is not yet justified in TESTING.md as AUDIT.md requires.
  - resolved 2026-09-30: TRANSLATIONS.md's new gender-agreement rule (every French text agreeing with a pawn needs a `{PAWN_gender ? m : f : n}` switch) reset `translation_fr` to `unchecked` mod-wide; this session reread section 3 and ran the required review pass — full-text audit of every Keyed/DefInjected/grammar French string. `NeverOutOfPrint_BookUnsettles`'s "convertit le lecteur qui n'en a plus" was a real instance: `reader` is in scope in `BookOutcomeDoer_Ideoligion.GetBenefitsString` at that call, so `.Translate()` now also passes `reader.Named("PAWN")` (English unaffected, ignores the extra named argument) and the French string reads `convertit {PAWN_gender ? le lecteur : la lectrice : la personne qui lit} qui n'en a plus`. `Tests/test_resources.py`'s EN/FR parameter-parity check updated to strip a `{\w+_gender ? ...}` switch before comparing `{0}`/`{1}` positional params, or a legitimate switch reads as a param mismatch. `NeverOutOfPrint_NamelessBeliever`/`NamelessBelievers` (fallback IDEO_memberName/Plural symbols, `GetTopicRuleStrings`, no Pawn parameter at that call site at all — book-generation time, not reading time) switched to static inclusive spelling instead, `croyant·e`/`croyant·e·s`, since no Pawn exists there to switch on. The remaining two candidates — the ideoligion book's static item description ("un croyant... assuré") and the grammar rule pack's "le lecteur"/"un sceptique" flavor text — likewise have no Pawn argument available (static Def field; generation-time grammar with no PAWN-tagged symbol); still flagged in `FRENCH_REVIEW.md` for the owner's own read rather than silently rewritten. `translation_fr` moves to `partial` (audited, no unresolved defect) — TRANSLATIONS.md is explicit that only the owner's own reading of `FRENCH_REVIEW.md` can mark it `complete`, never a session.
  - unverified: French review by Virginie (TRANSLATIONS.md, "Systematic French review", 2026-09-30). `FRENCH_REVIEW.md` at the mod root is current as of this pass; regenerate it (`uv run python Tests/generate_french_review.py`) before her next read if any French or English text changes first.
  - unverified: All in-game verification (done -> tested): nothing has been loaded in RimWorld. No Player.log read, no Pickle run, no @review captures.
  - feature: PUBLISHING.md's systematic pull-request-to-origin rule is blocked, not fulfilled: github.com/emipa606/VanillaBooksExpandedExpanded is archived and GitHub refuses PRs against archived repositories. Recorded in BACKLOG.md with the reason; recheck if the repository is ever unarchived.
  - unverified: GitHub social-preview image not set (PUBLISHING.md, "Topics et image de partage GitHub") — no API/gh path exists for it, only the repo Settings page through a browser; topics (rimworld, rimworld-mod, mod) were set by gh. Set the image once ready, via Claude in Chrome or by hand.
session:      local_6cf3cb3c-08c2-4179-84cc-d5e727e5fe98
updated:      2026-09-30, offline test harness written (93 checks) and the 2026-09-30 French gender-agreement review pass completed (see `remaining`); 2026-09-29 reconstructed from scratch after the 2026-09-06 build was deleted (duplicate coverage by living mods, see BACKLOG.md history in the mod's memory file); namespace/assembly/defName prefix changed from the deleted build's `PrintingPress` to `NeverOutOfPrint` specifically to avoid colliding with Zaljerem's living `zal.printingpress` mod at def-load (two assemblies exporting the same full type name). Session title corrected 2026-09-29: first set to `neveroutofprint / port`, wrong per AUDIT.md — the title uses `workflow_stage`, not `stage` (`stage` has only six codes and would have dropped the finer `horsMonoRepo` state); renamed to `neveroutofprint / horsMonoRepo`, then to `neveroutofprint / l10n` as workflow_stage advanced.
---

# Never Out of Print - status

Kept at the root, never inside `Mod/`, so Steam never receives it.

## Where it stands

`stage` is **port**, `workflow_stage` is **horsMonoRepo**: the repository is not yet detached
into its own standalone git repo and pushed (see "Right after this file" below — that step was
still pending when this file was written). Every offline check that does not require a running
game or the mod owner's own act has been run and is green: `dotnet build` (0 errors, 0 warnings),
`Check-XmlFields.ps1` (12 files, no unknown fields), `Check-XmlClasses.ps1` (29 types, all
resolved), `Check-DefRefs.ps1` (against Core/DLC and Vanilla Books Expanded's own defs via
`-AlsoScan`: no unresolved reference), `Check-DefInjected.ps1` (33 keys, 0 errors; the five
`MayRequire` advisories are expected and answered by `Mod/Ideology/`'s `LoadFolders.xml` gate).

Duplicate-coverage search re-run on 2026-09-29 against the full ~9,000-folder Workshop corpus with
`scripts/Search-Workshop.sh`, both for the exact name (`Never Out of Print` / `NeverOutOfPrint`:
zero hits) and for the mechanism (every modded `BookOutcomeProperties`/`ReadingOutcomeProperties`
class, and every `RecipeWorker_CopyBook`/`CompCopiedBook` reference): unchanged from the
2026-09-28 finding — one living copy-book mod (Zaljerem's *Printing Press*, `zal.printingpress`)
and three living ideoligion-book mods (Alpha Books, Enhanced Beliefs, PropagandaAndManifestos),
plus an unused engine in Hauts' Framework. Building this mod under a name and namespace that
collide with none of them was the decision made on 2026-09-28 (see the mod's memory file,
`rimworld-mod-printingpress.md`, and `ATTRIBUTION.md` in this repository).

`stage` codes, as `AUDIT.md` step 12 defines them: `port` before `horsMonoRepo`; `showcase` from
`Preview générée` to `l10n`; then `preTest`, `done`, `tested`, `published`.

## ModIcon and Preview: both resolved 2026-09-29

Both assets are now owner-generated (this session never generated artwork for either, per
STYLE_RIMWORLD.md's "ModIcon : contrôle, pas génération" and the same standing preference the
owner set for Preview): `Mod/About/ModIcon.png` (128x128, 20.6 KB, resized by this session from
the owner's 1254x1254 delivery with her explicit per-file accord; full-res original kept at
`Art/Icons/ModIcon-source.png`) and `Mod/About/Preview.png` (896x504, text overlay composed by
this session per RECOMPOSER_PREVIEW.md; no-overlay source kept at `Art/Preview.png`). See
"Documentation read" and the `remaining:` entries above for the detail and the one open note
(ModIcon content is a generic emoji, not a mod-specific mascot — legibility passes, so not
blocking, flagged for the owner to decide).

The earlier unexplained stray 1254x1254 PNG found at `Mod/About/ModIcon.png` when reconstruction
began — no git history, no reference anywhere, origin unknown — remains untouched at
`Art/ModIcon-stray-1254.png`, unrelated to the owner's actual delivery above.

Both `desktop.ini`s are wired: root (`IconResource=Art\ModIcon.ico,0`) and `Mod/`
(`IconResource=..\Art\Preview.ico,0`); both folders `ReadOnly`, both `desktop.ini`s
`Hidden,System`.

## `.dds` files

None exist in this mod (textures are `.png` only), so "sors les .dds de git" is `not_applicable`.

## Documentation read for this reconstruction

See `docs/PROTOCOLS-READ.md`.
