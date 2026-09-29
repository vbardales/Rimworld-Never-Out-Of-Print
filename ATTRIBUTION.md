# Never Out of Print — attribution

A 1.6 rebuild of the copying and ideoligion halves of **Vanilla Books Expanded Expanded**, by
**Ben Lubar** and **Efi**, continued by **Mlie**. Original Workshop item
[2650429441](https://steamcommunity.com/sharedfiles/filedetails/?id=2650429441); continuation
[2894816192](https://steamcommunity.com/sharedfiles/filedetails/?id=2894816192); source at
[github.com/emipa606/VanillaBooksExpandedExpanded](https://github.com/emipa606/VanillaBooksExpandedExpanded)
(archived). The ideoligion book artwork is **Erazil**'s, contributed to the continuation.

## Status: public, and licensed

The source is dead — `supportedVersions` stops at 1.4, the repository was archived, and the mod's
own in-game update notice says in as many words: *this mod will not be further updated*. It
carries a licence, so this is not the usual abandoned-and-silent case.

The check ran at four places:

| Where | Result |
|---|---|
| `LICENSE.md` in the published mod | **MIT License, Copyright (c) 2020 Mlie** |
| `About.xml` description | one carve-out, on artwork — see below |
| Body of the Steam page (both items) | identical text; no licence, no prohibition |
| The linked repository | **MIT**, archived |

Searched for a refusal as well as for a permission — `prohibit`, `forbid`, `do not`,
`redistribut`, `no redistribution` — because a refusal never presents itself as a licence. There
is none on either page.

### The one carve-out, and what it excluded

Under *Legal Stuff*, both Steam pages say:

> The modified writers' and typewriter table sprites are based on the work of Oskar Potocki,
> creator of Vanilla Books Expanded, and used with express permission in this mod.

Express permission granted to **that** mod is not a licence to anyone else, and it does not travel
with the MIT notice. So those two sprites are not here — nothing in this mod retextures anything
of Oskar Potocki's.

### Original notice, quoted in full (MIT requires it to accompany the reused portion)

```
MIT License

Copyright (c) 2020 Mlie

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## What was reused, and what was rewritten

Reused under the MIT licence above, textures renamed to this mod's own prefix where the file
sits under a path this mod owns:

- **Textures.** The two presses, in all four rotations with their masks (`PrintingPressManual_*`,
  `PrintingPressElectric_*`, kept at their original filenames since those are pure texPath
  strings and collide with nothing); the ideoligion book cover and its menu icon, renamed to
  `NeverOutOfPrint_IdeoligionBook*.png`.
- **Defs and balance.** The presses' costs, sizes, power draw, work speeds and research gating;
  the copy recipes' work amounts and cloth cost; the book-burning recipe; the copy/original
  stockpile filters; the ideoligion book's certainty numbers and its quality curve, awful's
  negative multiplier included.

Rewritten from nothing, because none of it could be ported:

- **All the C#.** RimWorld 1.5 put books in the base game and Vanilla Books Expanded was rewritten
  on top of them. Every class the old addon hooked belonged to the version before that:
  `VanillaBooksExpanded.JobDriver_ReadBook`, `.CompBook`, `.ITab_Book`, `.SkillBook`,
  `.JoyGiver_ReadBook` are all gone, and the addon's nineteen classes were patches on them. The
  base game's own `RimWorld.CompBook` is a different class that happens to share a short name.
- **Namespace and assembly, deliberately not `PrintingPress`.** A living 1.6 mod,
  [Zaljerem's *Printing Press*](https://steamcommunity.com/workshop/filedetails/?id=3431938877)
  (`zal.printingpress`), is spun out of the same dead source and uses that exact namespace and
  assembly name. `GenTypes` resolves a `compClass`/`workerClass` by scanning every loaded
  assembly: two mods exporting the same full type name collide at def load and both break. This
  mod's namespace, assembly and def prefix are `NeverOutOfPrint`, chosen specifically to not
  collide.
- **The rule packs.** The originals resolved `r_art_name`, the symbol the art-description system
  uses. A 1.6 book resolves `title` and `desc`. Same idea, different grammar.
- **Both translations.** The source shipped English by Ben Lubar and Efi, German by Dimos, French
  by qux. None of it is here: every key those files inject names a def that no longer exists.
  The English and French shipped here were written for this mod.

## Why this is a rebuild and not a port of Zaljerem's Printing Press

Zaljerem's mod is not this mod's origin: nothing of its code was read, copied, or based on.
It is documented here because it targets the same dead source and the same mechanic, and because
its namespace is the reason this mod's own had to be chosen carefully. Two differences worth
recording, found while checking whether this mod would duplicate it (see
`BACKLOG.md` in this repository for the fuller note):

- Zaljerem's copy carries only a book's title and the two description strings; its outcome doers
  are re-rolled by `SetQuality` → `GenerateBook()`. Because it also puts its `CompCopiedBook` on
  `BookBase` itself, an Odyssey map is copyable there, and its quest chance is 1: every printed
  map carries a fresh quest. This mod's copy carries every outcome doer's rolled state verbatim
  and strips the one-off quest discovery specifically, so a copied map is a map with no second
  treasure.
- Neither of the two other mods reused any content from this one, nor the reverse.

## Credit

Thanks to Ben Lubar and Efi for the original mod and for the design this rebuild follows, to Mlie
for keeping it running for two more versions and for licensing it, and to Erazil for the
ideoligion book artwork.

This mod's own work — the C#, the rewritten rule packs, the translations — is MIT; see `LICENSE`.

If the authors would rather this were not published, it comes down on request.
