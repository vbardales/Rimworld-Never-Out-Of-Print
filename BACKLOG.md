# Backlog

Ideas for this mod that are not started. Each entry says what the mechanic would be, what already
covers part of it, and what has to be settled before the first line of code.

An idea earns a place here only if it serves what the mod already does: copying books, on a ladder
of methods from the hand to the machine. Anything already shipped is in the changelog instead.

---

## Copying a research paper: a backup of the knowledge

Proposed 2026-09-28.

[Research Papers](https://steamcommunity.com/sharedfiles/filedetails/?id=3492739424)
(`ChaoticEnrico.ResearchPapers`, Workshop 3492739424, 1.5 and 1.6) makes a finished research
project leave a paper behind. Its own description says what is at stake: *losing it results in a
loss of knowledge*. A book that can burn and whose loss costs a project is exactly what a press is
for, so this mod's copy is the natural backup.

**What the mechanic would be.** A copy of a paper is a second holder of the same knowledge. Lose
the original to a raid or a fire and the colony keeps the project, which is the whole promise of
the name.

**What breaks if the copy is left as it is planned.** Read from the installed 1.6 assembly, not
assumed:

- A paper is a `Book` built on `BookBase`, with a `BookOutcomeProperties_ResearchPaper` doer and,
  beside it, a second comp, `CompResearchPaper`, which holds the list of projects the paper stands
  for.
- That comp registers the paper with the mod's game component when it is created, without touching
  research progress, and unregisters it when the paper is destroyed. A trade adds or removes it
  the same way.
- The copy planned here reproduces a book's own fields and its outcome doers. It does not carry
  the state of any other comp. A copied paper would therefore register with **an empty project
  list** and protect nothing, while looking like a paper in every inspect pane.
- The def is `tradeability None`, sits in its own thing category rather than under Books, and has
  no thing-set-maker tags. So it is never traded and never generated: every paper in a colony
  came from a finished project.

**What it asks of the copy code.** Carry the state of the other comps too, at least this one, and
register the copy so the game component counts it. That is a general gap and not a special case:
any modded book that keeps state outside its doers has the same problem.

### Before starting

1. **Read what decides a loss.** The mod patches `Settlement.Abandon` and the colony-move utility,
   so the check probably runs against the registered list. Whether a copy counts, and whether two
   papers for one project can ever make a project *more* finished, is the whole exploit question.
2. **Read what reading a paper does.** A patch on the base game's research-reading tick suggests
   it feeds progress. If reading grants research, a copy must read like its original and must not
   be a way to grant it twice.
3. **Decide whether a paper is copyable at all.** It is a knowledge record, not a story. Copying
   may reasonably be limited to the hand and the press, or refused outright.
4. **Check the licence** of Research Papers in the four places. It is not checked yet. The
   monorepo backlog only records that the Foundations Only bridge ships no licence file and that
   its About declares none.
5. **Keep it a compatibility, never a dependency.** This mod has to run with Research Papers absent.

### Related, and pulling the other way

The monorepo `BACKLOG.md` holds *Research Papers: books for foundations only, without Node
Research*, which caps how many papers exist. This idea multiplies the copies of the ones that do.
They do not conflict, but they should not be designed apart: a foundations-only cap makes each
surviving paper worth more, which makes a backup worth more.

---

## Language: what a copy owes to the language of a book

Proposed 2026-09-28.

[Rim Languages](https://steamcommunity.com/workshop/filedetails/?id=3450332059) (Cybranian,
namespace `FactionLanguages`, Workshop 3450332059, 1.5 and 1.6, installed) gives every humanoid
faction a language. The idea: a book has a language, and copying it depends on how well the copier
knows it, so that the hand asks more of the copier than the machine does.

**What the mod models, read from its 1.6 assembly and defs.** Less than the idea assumes:

- Language belongs to a **faction**, and what a colony has learned is **colony-wide**. There is no
  per-pawn knowledge and **no split between spoken and written**. The only per-pawn factor is a
  stat, `LanguageLearningAbility`, and it only sets how fast something is learned.
- **One book has a language: `LanguageBook`.** It carries a `BookOutcomeDoerGainLanguageExp`, which
  keeps a private `dialect`, drawn at random weighted by commonality, and reading it teaches that
  dialect. Textbooks, novels, schematics and every other book carry no language at all.
- The book stops giving anything once its dialect and the dialect's parent language are learned.

So the idea as worded has nothing to read: no oral or written data, and no language on most books.

**Where it does apply, and three ways to use it.**

- **Gate the copy of a language book on the colony's knowledge of that dialect.** A hand copy needs
  the dialect learned, a press needs less, a scanner or an e-book nothing. This is the one book
  where language is the content, and the ladder then matters: a foreign lesson found as loot is
  copyable only by a machine.
- **Give every book a language tag of our own**, from the author's faction. This extends the idea
  to all books but rests on data the mod does not provide, and it forces a decision on what a
  novel's language even is.
- **Do nothing**, and copy a language book like any other. It stays correct, and costs nothing.

### What breaks in the copy code, found while reading

- The language doer declares its **own cached `compQuality` field**, pointing at its parent's
  quality comp. The planned copy walks a doer's fields and would copy that cache, so the copy would
  read its quality from the **original**. Any doer that caches a reference to its parent's comps
  has this. The rule to write: never copy a reference to the source thing or to its comps.
- The dialect is drawn **lazily**. A book nobody has asked yet would let the copy draw its own,
  and the two could teach different dialects. The copy has to force the draw on the source first.

### Before starting

1. **Choose between the three options above.** Everything else follows.
2. **Check the licence** of Rim Languages in the four places. Not checked.
3. **Keep it a compatibility, never a dependency**, like Research Papers.

---

## Pull request to the origin repository (PUBLISHING.md, 2026-09-28 rule)

`PUBLISHING.md`, "Départ depuis le projet d'origine": *dès qu'un dépôt d'origine existe, la pull
request vers lui est systématique*. The origin here is
[github.com/emipa606/VanillaBooksExpandedExpanded](https://github.com/emipa606/VanillaBooksExpandedExpanded)
(MIT), whose defs, textures and balance this mod reuses.

**Blocked, not skipped.** The repository is archived (confirmed via `gh repo view`,
`isArchived: true`). GitHub refuses pull requests against an archived repository outright — there
is no fork-and-PR path while it stays archived. Nothing to propose until Mlie (or GitHub) unarchive
it. Recheck on a schedule, or if the repository's state ever changes; do not silently drop the
item.

There is also, practically, nothing to send: every class this mod's C# hooks belongs to the 1.6
book API, and every class the archived repository's own C# hooks was deleted from the game in
1.5. A PR proposing "make this work in 1.6" would not be a patch to the existing code, it would be
this mod's own rewrite — which is not something to hand to an archived, unmaintained repository
as a pull request.
