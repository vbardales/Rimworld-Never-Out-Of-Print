# Publication

What the Workshop page needs that lives nowhere else in this repository. Written before the
first envoi; the running `docs/runs/` notes trim what is already sent (AUDIT.md, "avant la mise
en veille du mod").

## Steam description

Not yet migrated to the `PUBLICATION.md` single-source form (PUBLISHING.md, "Source unique de la
description", 2026-09-25 decision): `Mod/About/About.xml`'s `<description>` is written by hand
and is the authoritative source for now. Copy it here verbatim before the first envoi if this mod
later adopts the CI's `sync-about-description.mjs` pipeline.

## Screenshot order

Not yet decided or produced. `About/Preview.png` exists (generated, see STATUS.md); the Workshop
gallery captures do not. Pending: a press mid-bill, a copy sitting next to its original with the
stats-report line visible, an ideoligion book's benefits tooltip, the two new stockpile filters.

## Thank-you comments

Registry: `WORKSHOP_COMMENTS.md` at the monorepo root, keyed by the recipient's Workshop id, not
by this mod. Checked 2026-09-29: neither recipient id below has a row in the registry yet
(`grep` for `2894816192` and `2193152410` returns nothing). Drafts are written and rows added
only once this mod is actually visible to its readers (WORKSHOP_COMMENTS.md, "Post only once the
item is visible"), which it is not yet — nothing has been envoi'd. To prepare then:

| Recipient | Workshop id | Why |
|---|---|---|
| Vanilla Books Expanded Expanded (Mlie / Ben Lubar and Efi) | 2894816192 | source of the defs, textures and balance this mod reuses under MIT |
| Vanilla Books Expanded (Oskar Potocki) | 2193152410 | owns the 1.6 book system this mod builds on; `loadAfter` compat |

Not owed a comment: Zaljerem's *Printing Press* and the three ideoligion-book mods found during
the duplicate check (Alpha Books, Enhanced Beliefs, PropagandaAndManifestos) — nothing of theirs
was read, studied or reused; see ATTRIBUTION.md, "Why this is a rebuild and not a port of
Zaljerem's Printing Press".

## Dependencies and DLC

- `modDependencies`: none. Nothing is required to load.
- `loadAfter`: `Ludeon.RimWorld.Ideology`, `Ludeon.RimWorld.Odyssey` (both optional content this
  mod's defs are gated on with `MayRequire`), `VanillaExpanded.VBooksE` (optional compat, see
  `Mod/Patches/VanillaBooksExpanded.xml`).

## Adult content

Not applicable. No such content.

## Version notes (Steam)

Written at the moment of the first envoi.

## Rollback target

Not yet applicable; no version has shipped.
