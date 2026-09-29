# Testing

## Offline (preTest -> done)

Not yet written. Pending: an XML/reflection test harness on the model of ManureComposting's
`Tests/test_resources.py` — verifying the recipe/comp/def wiring, the copy's field-and-doer
reproduction logic, and the `IDEO_` grammar symbols resolve, without needing a running game.

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
