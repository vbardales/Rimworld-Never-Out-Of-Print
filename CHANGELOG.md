# Changelog

## [Unreleased]

First reconstruction, for RimWorld 1.6. A rebuild of the copying and ideoligion halves of
*Vanilla Books Expanded Expanded* (Ben Lubar and Efi, continued by Mlie), whose defs, artwork and
balance are reused under its MIT licence and whose code could not be — see
[ATTRIBUTION.md](ATTRIBUTION.md).

### Added

- Two printing presses, manual and electric, behind a printing research project. When Vanilla
  Books Expanded is loaded they hang off its printing research instead, and this mod's own project
  is removed rather than sitting in the tree under the same name.
- **Copy book** and **copy book with fewest copies** bills. The book is not consumed.
- Copies carry every repeatable benefit of the original verbatim — skill rates, research subjects,
  joy factor, mental-break chance, quality — and no one-off discovery. Printing an Odyssey map does
  not print a second treasure.
- Copies are worth a tenth of the original, say so in the inspect pane and the stats report, and
  can be separated from originals in any stockpile.
- **Ideoligion book** (needs Ideology): reading it raises certainty in its own faith, erodes it in
  any other, and converts a reader who runs out — through the base game's own conversion path. An
  awful one does the opposite of what it intends. Reading policies get an *ideoligion* effect.
- **Burn books**, wherever burning drugs already is. The base game marks books burnable by recipe
  and ships no recipe that burns one.
- English and French.

### Not carried over from the source

- The retextured writers' and typewriter tables. Those sprites are Oskar Potocki's, used in the
  original under a permission personal to it, which the MIT notice does not carry.
- The two-tile bookshelf. It existed to give LWM's Deep Storage a book container; 1.6 has
  `Building_Bookcase` in the base game.
- The book-burning precept, which needed Vanilla Ideology Expanded — Memes and Structures.
- The fixes and tweaks aimed at Vanilla Books Expanded itself: its joy giver, its book tab, its
  library room role, its stuffable tables. All of them targeted classes that 1.6 no longer has, and
  Vanilla Books Expanded now ships its own library room role.
