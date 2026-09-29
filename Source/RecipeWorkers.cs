using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace NeverOutOfPrint
{
    /// <summary>
    /// The book on the press is read, not spent. Everything else in the bill - the cloth - is
    /// consumed as usual.
    /// </summary>
    public class RecipeWorker_CopyBook : RecipeWorker
    {
        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            if (ingredient is Book)
            {
                return;
            }

            base.ConsumeIngredient(ingredient, recipe, map);
        }
    }

    /// <summary>
    /// Counts what a "copy book" bill has already produced. The base counter counts things of the
    /// recipe's product def, and this recipe's product def is a placeholder that never exists;
    /// what the bill is really making is copies, of whichever books its ingredient filter allows.
    /// </summary>
    public class RecipeWorkerCounter_CopyBook : RecipeWorkerCounter
    {
        public override bool CanCountProducts(Bill_Production bill)
        {
            return true;
        }

        public override int CountProducts(Bill_Production bill)
        {
            List<ThingDef> outputs = BookSearch.PossibleOutputs(bill);
            int count = 0;
            foreach (Thing thing in BookSearch.Candidates(bill))
            {
                if (outputs.Contains(thing.def) && IsCountableCopy(thing, bill))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// CountValidThing is the base game's own gate - hit points, quality range, allowed stuff,
        /// fog - and it is given the thing's own def so that only those checks apply. The def has
        /// already been matched against the bill's filter by the caller.
        /// </summary>
        protected bool IsCountableCopy(Thing thing, Bill_Production bill)
        {
            return (thing.TryGetComp<CompCopiedBook>()?.Copied ?? false) && CountValidThing(thing, bill, thing.def);
        }

        public override string ProductsDescription(Bill_Production bill)
        {
            List<ThingDef> outputs = BookSearch.PossibleOutputs(bill);
            return outputs.Count == 1 ? outputs[0].label : null;
        }

        public override bool CanPossiblyStore(Bill_Production bill, ISlotGroup slotGroup)
        {
            StorageSettings settings = slotGroup.Settings;
            if (settings == null)
            {
                return true;
            }

            if (!settings.filter.Allows(NeverOutOfPrintDefOf.NeverOutOfPrint_AllowCopies))
            {
                return false;
            }

            QualityRange allowed = settings.filter.AllowedQualityLevels;
            if (allowed.min > bill.qualityRange.max || allowed.max < bill.qualityRange.min)
            {
                return false;
            }

            return BookSearch.PossibleOutputs(bill).Any(settings.AllowedToAccept);
        }
    }

    /// <summary>
    /// The same bill, aimed at whichever work is currently least stocked. The count it reports is
    /// the number of copies of the scarcest title, so a target of three means "three copies of
    /// everything", not "three copies in total". Patch_WorkGiver_DoBill is what makes a colonist
    /// actually pick that book up.
    /// </summary>
    public class RecipeWorkerCounter_CopyBookFewest : RecipeWorkerCounter_CopyBook
    {
        public static List<BookKey> LeastCopiedWorks(Bill_Production bill, out int count)
        {
            var counts = new Dictionary<BookKey, int>();
            List<ThingDef> outputs = BookSearch.PossibleOutputs(bill);

            foreach (Thing thing in BookSearch.Candidates(bill))
            {
                if (!outputs.Contains(thing.def) || !CountValidThingStatic(thing, bill))
                {
                    continue;
                }

                BookKey key = BookKey.For(thing);
                if (key == null)
                {
                    continue;
                }

                counts.TryGetValue(key, out int existing);
                counts[key] = existing + ((thing.TryGetComp<CompCopiedBook>()?.Copied ?? false) ? 1 : 0);
            }

            if (counts.Count == 0)
            {
                // No book to copy at all. Reporting zero would make the bill run forever against
                // nothing; the work giver finds no ingredient either way, but a bill that thinks
                // it is done is the quieter of the two failures.
                count = bill.targetCount;
                return new List<BookKey>();
            }

            int fewest = counts.Values.Min();
            count = fewest;
            return counts.Where(pair => pair.Value == fewest).Select(pair => pair.Key).ToList();
        }

        private static bool CountValidThingStatic(Thing thing, Bill_Production bill)
        {
            return bill.recipe.WorkerCounter.CountValidThing(thing, bill, thing.def);
        }

        public override int CountProducts(Bill_Production bill)
        {
            LeastCopiedWorks(bill, out int count);
            return count;
        }

        public override string ProductsDescription(Bill_Production bill)
        {
            return null;
        }
    }
}
