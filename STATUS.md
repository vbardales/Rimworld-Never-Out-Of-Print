---
localization:   complete
translation_en: complete
translation_fr: complete
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
  - resolved 2026-09-29: ModIcon.png delivered by the owner (1254x1254, unrelated to the earlier unexplained stray already at Art/ModIcon-stray-1254.png). Full-resolution delivered file kept untouched at Art/Icons/ModIcon-source.png; resize to 128x128/20.6 KB done by this session with the owner's explicit per-file accord ("oui, vas-y") — a format geste, not a generation, per STYLE_RIMWORLD.md. Art/ModIcon.ico built (16/32/48/256, RGBA PNG-encoded, ffmpeg via per-size streams mapped into one mux — recipe added to STYLE_RIMWORLD.md 2026-09-29, resolving the earlier single-resolution limitation). Root desktop.ini wired (IconResource=Art\ModIcon.ico,0), root folder ReadOnly, its desktop.ini Hidden+System.
  - note: at 32px the delivered icon reads clearly (head + wink legible, single motif) — the STYLE_RIMWORLD.md legibility test passes. It is a generic winking-sun emoji, not a mod-specific mascot holding an accompanying object (book/press) the way the guide's "tête et objet qui l'accompagne" phrasing describes. Not a legibility failure, so not blocking per the guide's own rule (only a failed 32px test forces an owner query) — flagged here rather than silently accepted or replaced.
  - resolved 2026-09-29: Preview.png text overlay composed per STYLE_RIMWORLD.md's surcouche rules and RECOMPOSER_PREVIEW.md, via Art/compose-preview.cjs (Playwright + Chrome headless, Segoe UI, 896x504). Palette in Art/preview-palette.json: veil sampled from the cool stone floor at the reserved top-left zone; inkPrimary ivory (veil luminance dark); inkSecondary the warm amber/brass family of the lamp pool and wood, brightened for contrast; accent a deep terracotta, split from inkSecondary by saturation/lightness since the source illustration is close to monochromatic (guide's monochromatic-image clause). Title "Never Out of Print" with "of" reduced to 65% as a connecting word (ink stays primary — it is not a prefix/suffix); no tag line (public MIT mod, not (prohibited)/(unofficial)); résumé is About.xml's first sentence; badge "1.6" from supportedVersions. Script-verified: Segoe UI available, minimum contrast 11.4:1 (title) / 7.0:1 (résumé) / 4.7:1 (badge), all ≥4.5:1; file 490 KB, under the 900 KB soft limit. Raw check output at Art/qa/visual-checks.json (gitignored, not evidence retained per AGENTS.md's rule for git). Art/Preview.png stays the no-overlay source; Art/Preview.ico rebuilt from the final overlaid file.
  - dependency: Art/compose-preview.cjs needs playwright + sharp; package.json added at repo root (matches the pattern in AnimalsNaturally/MintchocoConfectionery), node_modules/ gitignored. Not yet run against a live in-game screenshot for camera/lighting fidelity (AUDIT.md treats this as a review method, not a blocking requirement, when no camera defect is visible on inspection — none was).
  - resolved 2026-09-29: applied the owner's new corner-stamp rule (STYLE_RIMWORLD.md, "Le ModIcon détouré sur la vitrine") — Preview.png now also carries ModIcon.png, cutout from its near-black background by a colour-distance alpha threshold (no ImageMagick), tilted -15° and composited bottom-right (the corner the text block does not occupy), wired into Art/compose-preview.cjs so it reproduces on regeneration. Fixed a bug in the same edit: the script was re-reading the already-overlaid Mod/About/Preview.png as its background source instead of the clean Art/Preview.png, which would have compounded text on every re-run — now reads Art/Preview.png. Art/Preview.ico rebuilt from the final stamped file. Also applied the owner's screenshot-0 rule (PUBLISHING.md, already documented there 2026-09-29): Art/Gallery/00-Preview.png is a byte-copy of the final Preview.png.
  - note: RECOMPOSER_PREVIEW.md (the copy-paste prompt mirroring STYLE_RIMWORLD.md's surcouche section) was read once this session, then found deleted from disk later the same session — untracked in this repo (protocols left the monorepo, per commit 90d51374), no git trail here. Not recreated blind; the corner-stamp rule was written directly into STYLE_RIMWORLD.md instead, which stays authoritative regardless.
  - resolved 2026-09-29: fixed two defects the owner caught by eye in the first corner-stamp render. (1) The colour-distance cutout was global, so the icon's own near-black facial linework (the winking eye, the smile) fell inside the same distance band as the background and was cut transparent too — invisible on the dark left-side veil, but a real hole once composited over the lighter floor on the right. Replaced with a border flood-fill: only background pixels reachable from the image edge are cut, so interior dark linework stays opaque regardless of its colour distance from the background. (2) The stamp sat too far from the corner (24px margin) with too much bare floor beside it; margin reduced to 10px. Both fixes are in Art/compose-preview.cjs, reproducible on regeneration.
  - resolved 2026-09-29: workflow_stage advanced horsMonoRepo -> ModIcon générée -> Preview générée -> preOptions -> options -> l10n in one step. Each of preOptions/options/l10n is asserted from source inspection, not a running game (no settings class, no MainButtonDef, all Keyed/DefInjected coverage checked by Check-DefInjected.ps1, 33 keys, 0 errors) — AUDIT.md's "stage jamais bloqué par le jeu" treats that as sufficient to advance, since no defect was found; the in-game exercise itself stays open below, under l10n -> preTest.
  - unverified: l10n -> preTest: modDependencies is empty by design (VBE is an optional compat, not a hard dependency — loadAfter only), which needs to be re-confirmed against MOD_SETTINGS.md/PUBLISHING.md wording once the mod is actually loaded in a game with and without VBE present. Options/l10n themselves also not yet exercised in a running game.
  - unverified: preTest -> done: no automated XML/offline test harness written yet (no equivalent of ManureComposting's test_resources.py); no Tests/Pickle/ suite written yet, so its absence is not yet justified in TESTING.md as AUDIT.md requires — TESTING.md exists as a skeleton only.
  - unverified: All in-game verification (done -> tested): nothing has been loaded in RimWorld. No Player.log read, no Pickle run, no @review captures.
  - feature: PUBLISHING.md's systematic pull-request-to-origin rule is blocked, not fulfilled: github.com/emipa606/VanillaBooksExpandedExpanded is archived and GitHub refuses PRs against archived repositories. Recorded in BACKLOG.md with the reason; recheck if the repository is ever unarchived.
  - unverified: GitHub social-preview image not set (PUBLISHING.md, "Topics et image de partage GitHub") — no API/gh path exists for it, only the repo Settings page through a browser; topics (rimworld, rimworld-mod, mod) were set by gh. Set the image once ready, via Claude in Chrome or by hand.
session:      local_6cf3cb3c-08c2-4179-84cc-d5e727e5fe98
updated:      2026-09-29, reconstructed from scratch after the 2026-09-06 build was deleted (duplicate coverage by living mods, see BACKLOG.md history in the mod's memory file); namespace/assembly/defName prefix changed from the deleted build's `PrintingPress` to `NeverOutOfPrint` specifically to avoid colliding with Zaljerem's living `zal.printingpress` mod at def-load (two assemblies exporting the same full type name). Session title corrected 2026-09-29: first set to `neveroutofprint / port`, wrong per AUDIT.md — the title uses `workflow_stage`, not `stage` (`stage` has only six codes and would have dropped the finer `horsMonoRepo` state); renamed to `neveroutofprint / horsMonoRepo`.
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
