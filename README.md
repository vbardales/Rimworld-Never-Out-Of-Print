# Never Out of Print

A RimWorld 1.6 mod. Copy the books you own, and print the ones that argue.

The base game has no way to make a book. Books arrive by trade, by quest reward and by map
generation, and there is exactly one of each. This adds the press that answers that, and a fourth
kind of book worth running off in quantity.

Requires nothing. Works with [Vanilla Books
Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2193152410) when it is present,
and does not need it. English and French.

## What it adds

### Printing presses

Two, behind a printing research project. The manual press is medieval, 150 wood or metal; the
electric press needs power and runs four times as fast. Both take bills.

| Bill | What it does |
|---|---|
| Copy book | Prints another of whichever book the bill's ingredient filter allows. 100 cloth. |
| Copy book with fewest copies | The same, aimed at whichever title the colony is shortest of. |

The book is not consumed. It goes back on the shelf when the job is done.

### What a copy is

**A copy teaches exactly what the original teaches.** Skill rates, research subjects, joy factor,
mental-break chance, quality — all carried across verbatim.

That is a decision, not an accident. A book's benefits are rolled once, when it is generated, and
stored on the instance; making a fresh book of the same def and calling it a copy would re-roll
them, and re-rolling turns the press into a slot machine — print schematics until one of them
happens to advance the project you wanted.

**With one exception: a one-off discovery does not survive copying.** Some books hold a quest, and
an Odyssey map holds one every time. A copy of a map is a map of the same place, without a second
treasure at the end of it. The blurb on the copy stops promising one.

Copies are marked as copies: a tenth of the original's market value, a line in the inspect pane and
in the stats report, and two new stockpile filters — *allow copied books*, *allow original books* —
so a trade beacon can sell the printed ones and keep what the colony actually found.

This works on every book on the base game's book system: textbooks, schematics, novels, Anomaly's
tomes, Odyssey's maps, and books from other mods. None of those books are patched; the press reads
whatever is in them.

### Ideoligion books (needs Ideology)

A book that argues for one ideoligion and is persuasive while it is being read. A believer reading
their own faith's book comes away steadier. Anyone else loses certainty, and a reader who runs out
converts — through the game's own conversion path, the one the conversion ritual uses, with the
same history event and the same memory of the faith they left.

Quality decides how hard it pushes, and an **awful one is negative**: a badly argued tract drives
its own believers away and hardens the doubters it was written for. The blurb says which way a
given copy actually pushes.

Colonists do not go looking for a rival faith's book on their own: the game asks each book whether
reading it would leave the reader steadier, and that one answers no. Leave it where they will find
it anyway, or order the read.

They appear in trader stock and quest rewards on their own. With a press you can print more of one.
With Vanilla Books Expanded you can also write them at a writers' table, and a colonist writes for
their own faith.

Reading policies get an *ideoligion* effect alongside skill gain and research, so a policy can
allow every book but that one.

### Burning books

The base game marks books `burnableByRecipe` and ships no recipe that burns one. This adds it,
wherever burning drugs already is — the campfire, the crematorium.

## Why the code is new

This is a rebuild of the copying and ideoligion halves of **Vanilla Books Expanded Expanded** by
Ben Lubar and Efi, continued by Mlie — a 1.3/1.4 mod that no longer runs. Its defs, artwork and
balance are reused under its MIT licence. None of its code is, because none of it could be:
RimWorld 1.5 put books in the base game, Vanilla Books Expanded was rewritten on top of them, and
every class the old addon hooked belonged to the version before that. Its nineteen classes were all
patches on classes that no longer exist.

The gain is visible in the ideoligion half. In 1.3 it took a `Thing` subclass, a `JobDef`, a
`JobDriver` and a `JoyGiver` to arrange for a colonist to sit down with a book at all. Since 1.5 the
base game does that itself and ticks every `ReadingOutcomeDoer` on whatever is being read — so what
is left to write is one outcome class and what happens on the tick. The `Thing` subclass survives
only to stamp the ideoligion's symbol on the cover.

The consequence worth knowing: **this does not extend Vanilla Books Expanded and does not need it.**
If you have it the two fit together — the presses hang off its printing research instead of adding
a second project under the same name, and its tables can write ideoligion books. If you do not,
everything here still works on the base game's books.

**This is also not a port of Zaljerem's *Printing Press*.** A living 1.6 mod of that name is spun
out of the same dead source; nothing of its code was read or reused. See `ATTRIBUTION.md` for what
differs, and why the namespace here is deliberately `NeverOutOfPrint`, not `PrintingPress`.

## Layout

```
NeverOutOfPrint/
  Mod/          published; the junction into RimWorld/Mods points here
    Ideology/   French translations of the defs that only exist with Ideology, gated by LoadFolders
  Source/       C#; builds straight into Mod/Assemblies/
  Art/          source images, never published
```

Build: `cd Source && dotnet build`.

## Licence and credit

MIT for this mod's own work; the reused portion carries the original MIT notice, quoted in full
in [ATTRIBUTION.md](ATTRIBUTION.md) — see that file for what was reused, what was deliberately
left behind, and where the licence was checked.

Thanks to Ben Lubar and Efi for the original, to Mlie for the continuation and the licence, and to
Erazil for the ideoligion book artwork.
