# Never Out of Print - Pickle suite

In-game acceptance tests. Development only: the companion under `Mod/` is never published, and
nothing here is part of the Workshop payload. Writing it is the `preTest -> done` criterion;
playing it and reading its captures is `done -> tested`. **Nothing here has been played.**

## What is in Gherkin, and what deliberately is not

`Tests/test_resources.py` proves the XML/reflection wiring offline: def cross-references, EN/FR
coverage, the recipe's users and research prerequisite, the ideoligion book's RulePackDef
references, packaging. What is left needs a running game, because it is per-Thing-instance state
(a comp's `copied` flag, a computed stat) or a generated `Pawn`/`Ideo` (only Ideology can build
one), neither of which exists outside a live world.

| Feature | Scenarios | Why only a running game can show it |
|---|---|---|
| 01 loading | 3 | the loader admitted the mod, the presses/recipe/research def database is real, the copy patch actually reached `Novel` and `MarketValueBase` |
| 02 copying | 1 | a real bill leaves the original on the shelf, marks the printed copy's `CompCopiedBook`, and the copy's *computed* market value (post-`StatPart_CopiedBook`) lands at the documented tenth |
| 03 ideoligion | 2 | a believer's own-faith read raises `Pawn_IdeoTracker.Certainty`; a rival-faith read lowers it |

**Not converted:** the awful-quality-book scenario TESTING.md also scopes ("an awful quality book
pushes the opposite way"). Forcing a specific `QualityCategory` on a freshly generated book through
Pickle's built-in steps was not found; needs either a custom step that sets the comp's quality
directly or confirmation that one of the vanilla quality-setting debug actions applies to books.
Left `unverified` in STATUS.md rather than guessed at.

## Custom steps (`Source/Steps.cs`)

Pickle's built-in `I add bill`/`I wait for bill to finish` pair does not fit this mod's own
recipe: `I wait for bill` polls the *declared* product def's count, which for
`NeverOutOfPrint_CopyBook` is `NeverOutOfPrint_CopiedBookPlaceholder` — a def that is never
actually spawned (`RecipeWorkers.cs`'s Harmony patch swaps the real output in before the
placeholder exists, exactly like `specialProducts` does, which the built-in step already refuses
to wait on for the same reason). `02-copying.feature` waits a fixed tick budget instead and counts
the real book def (`Novel`) directly.

Two more custom steps read per-Thing-instance state no built-in step reaches: the newest
`Novel`'s `CompCopiedBook.Copied`, and the ratio between the newest and oldest `Novel`'s computed
`MarketValue` stat (the built-in def-level stat steps read a `StatDef`'s XML value or its
game-computed default *without stuff*, not a specific spawned Thing's post-`StatPart` value).

`03-ideoligion.feature`'s steps call `BookOutcomeDoer_Ideoligion.OnReadingTick` directly — the same
method `RimWorld.JobDriver_Reading` ticks during a real read — rather than orchestrating a full
in-game reading session through a `JobDriver`, for which Pickle has no built-in step and 400 ticks
of simulated real reading would cost meaningfully more real time for the same proof. Setting the
rival-faith scenario's target `Ideo` goes through the `ideoligion` private field via reflection,
since `OnBookGenerated(Pawn author)` reads `author?.Ideo` and no fixture pawn is guaranteed to
hold the rival faith it would need to author that book.

## Running it

Never launch the game yourself: file requests with
`Rimworld-Ticket-Dispatcher/scripts/Submit-PickleRun.ps1` (one request per pass, the mod's SHA in
the label, the tree frozen on that revision until `RUN_DONE`). No `wsl-deps.map`/config seed is
needed yet: this mod declares no hard `modDependencies` and no settings (`settings_audit:
not_applicable`).

At minimum, per AUDIT.md: one pass without optional mods (Core + Ideology DLC + Harmony +
RimLogging + Pickle + this mod — Ideology is required for `03-ideoligion.feature`, tagged
`@requires:Ludeon.RimWorld.Ideology`), one pass with Vanilla Books Expanded present (this mod's
only `loadAfter`). Neither has run yet.

## Evidence to keep

After a run, keep only, per pass, the latest `summary.md`, `junit.xml`, `messages.ndjson` and
`Player.log` for the revision now in the repository. Copies live in `Tests/Pickle/Evidence/`
(gitignored); history is one text line per run in `docs/runs/`.
