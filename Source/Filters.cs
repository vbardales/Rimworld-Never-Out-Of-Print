using RimWorld;
using Verse;

namespace NeverOutOfPrint
{
    /// <summary>Stockpile filter: books that came off a press.</summary>
    public class SpecialThingFilterWorker_CopiedBooks : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            return t.TryGetComp<CompCopiedBook>()?.Copied ?? false;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.HasComp<CompCopiedBook>();
        }
    }

    /// <summary>Stockpile filter: books that did not.</summary>
    public class SpecialThingFilterWorker_OriginalBooks : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            CompCopiedBook comp = t.TryGetComp<CompCopiedBook>();
            return comp != null && !comp.Copied;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.HasComp<CompCopiedBook>();
        }
    }

    /// <summary>
    /// Reading-policy filter, built the way the base game builds its own effect filters (see
    /// RimWorld.SpecialThingFilterWorker_AllowBookSkill): ask the book's comp whether it carries
    /// the doer, rather than testing the def. A modded book that gains this outcome is covered
    /// without naming it.
    /// </summary>
    public class SpecialThingFilterWorker_AllowBookIdeoligion : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            return t is Book book && book.BookComp != null && book.BookComp.GetDoer<BookOutcomeDoer_Ideoligion>() != null;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.HasComp<CompBook>();
        }
    }

    /// <summary>
    /// A copy is worth a fraction of the original. Reading it is worth exactly as much; what a
    /// press cannot print is provenance.
    /// </summary>
    public class StatPart_CopiedBook : StatPart
    {
        private float factorCopied = 1f;
        private float factorOriginal = 1f;

        public override void TransformValue(StatRequest req, ref float val)
        {
            CompCopiedBook comp = req.Thing?.TryGetComp<CompCopiedBook>();
            if (comp == null)
            {
                return;
            }

            val *= comp.Copied ? factorCopied : factorOriginal;
        }

        public override string ExplanationPart(StatRequest req)
        {
            CompCopiedBook comp = req.Thing?.TryGetComp<CompCopiedBook>();
            if (comp == null)
            {
                return null;
            }

            return comp.Copied
                ? "NeverOutOfPrint_StatsReport_BookIsCopy".Translate() + ": x" + factorCopied.ToStringPercent()
                : "NeverOutOfPrint_StatsReport_BookIsOriginal".Translate() + ": x" + factorOriginal.ToStringPercent();
        }
    }
}
