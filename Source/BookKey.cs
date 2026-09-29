using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace NeverOutOfPrint
{
    /// <summary>
    /// Identity of a work, as opposed to identity of a volume: two things are the same book when
    /// they are the same def and carry the same generated title. That is what a copy shares with
    /// its original, and it is all the "copy the book with the fewest copies" bill needs in order
    /// to count.
    ///
    /// Titles are resolved once per book from a hash of its thingIDNumber, so two books generated
    /// independently do not collide in practice. The base game has no other handle: unlike the
    /// 1.3-era mod this replaces, a 1.6 book carries no tale reference to key on.
    /// </summary>
    public sealed class BookKey : IEquatable<BookKey>
    {
        private readonly ThingDef def;
        private readonly string title;

        private BookKey(ThingDef def, string title)
        {
            this.def = def;
            this.title = title;
        }

        public static BookKey For(Thing thing)
        {
            if (!(thing is Book book) || book.Title.NullOrEmpty())
            {
                return null;
            }

            return new BookKey(book.def, book.Title);
        }

        public bool Equals(BookKey other)
        {
            return other != null && other.def == def && other.title == title;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BookKey);
        }

        public override int GetHashCode()
        {
            return Gen.HashCombine(def?.GetHashCode() ?? 0, title);
        }
    }

    /// <summary>
    /// Where the presses look for books. The base game splits its own book search the same way
    /// (see Verse.BookUtility.TryGetRandomBookToRead): loose books are in the thing lister, shelved
    /// books are not, and a book in a colonist's hands is in neither.
    /// </summary>
    public static class BookSearch
    {
        public static List<ThingDef> PossibleOutputs(Bill_Production bill)
        {
            return bill.ingredientFilter.AllowedThingDefs.Where(def => def.HasComp<CompBook>()).ToList();
        }

        public static IEnumerable<Thing> Candidates(Bill_Production bill)
        {
            Map map = bill.Map;
            if (map == null)
            {
                yield break;
            }

            ISlotGroup includeGroup = bill.GetIncludeSlotGroup();
            if (includeGroup != null)
            {
                foreach (Thing thing in includeGroup.HeldThings)
                {
                    if (thing is Book)
                    {
                        yield return thing;
                    }
                }

                yield break;
            }

            foreach (Thing thing in map.listerThings.ThingsInGroup(ThingRequestGroup.Book))
            {
                yield return thing;
            }

            foreach (Building_Bookcase bookcase in map.listerThings.GetThingsOfType<Building_Bookcase>())
            {
                foreach (Thing held in bookcase.HeldBooks)
                {
                    yield return held;
                }
            }

            foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
            {
                if (pawn.carryTracker?.CarriedThing is Book carried)
                {
                    yield return carried;
                }
            }
        }
    }
}
