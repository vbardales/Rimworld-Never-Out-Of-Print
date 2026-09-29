# Protocol docs read for this reconstruction

Logged so a later session does not re-read what changed nothing, and re-reads what has since
moved. Monorepo-root commit hash is the doc's state as read, not this mod's own commit.

| Doc | Commit read at | What it decided here |
|---|---|---|
| `AUDIT.md` | `90d51374` | The whole gate chain; in particular the ModIcon-is-owner-only rule (step 2) and the fail-fast publication policy (not yet relevant, no version shipped). |
| `PUBLISHING.md` | `90d51374` | Systematic PR to an origin repo that exists (blocked here — archived, see BACKLOG.md); packageId has no `renew`/version suffix; description ends with the `[url=]Source code on GitHub[/url]` line; all documentation in English regardless of repo visibility; `LICENSE` + `ATTRIBUTION.md` split, both required. |
| `MOD_SETTINGS.md` | `90d51374` | `settings_audit: not_applicable` — no settings class, no MainButtonDef, nothing to expose; verified by source inspection that no empty page/shortcut exists (there is no C# UI code at all). |
| `TRANSLATIONS.md` | `90d51374` | Full EN/FR key inventory; no counted noun phrases anywhere in this mod's text (percentages and single amounts only), so the `.One`/`.Many` plural rule does not apply. |
| `STYLE_RIMWORLD.md` | `90d51374` | Read in full for the ModIcon section specifically ("ModIcon : contrôle, pas génération") — this is the rule that blocked generating one on request. The `Preview.png`/`ModIcon.png` size table (128x128, 896x504) was used directly. |
| `WORKSHOP_COMMENTS.md` | `08878789` | Process and registry check only (opening ~60 lines); no comment drafted yet since nothing is public. Re-read in full before actually drafting one. |
| `scripts/SEARCHING.md` | `90d51374` | `Search-Workshop.sh` usage; used it, not raw grep, for every duplicate-coverage search in this reconstruction. |
| `RECOMPOSER_PREVIEW.md` | `6bbc6202` | The exact surcouche-composition prompt/protocol; followed verbatim for `Art/compose-preview.cjs` (title/résumé layout, palette derivation, contrast/font/byte checks). |

## Not read this time, and why

- `PickleTools/README.md`, `PickleTools/Headless/README.md`, `PickleTools/docs/steps.md` —
  no Pickle suite exists yet for this mod (`TESTING.md`); nothing to write against them until
  that starts. Read before writing the first suite.
- `Rimworld-Release-Admin/docs/OPERATIONS.md` — no CI publish workflow touched; this mod has not
  reached `prepublished`. Read before the first `dispatch-publish.sh` call.
- `Rimworld-Ticket-Dispatcher/docs/WELCOME.md`, `docs/SUBMIT.md` — no Pickle run has been
  requested for this mod. Read (and `REGISTER local_...` sent) before the first
  `Submit-PickleRun.ps1` call.
