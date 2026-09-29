using RimWorld;
using Verse;

namespace NeverOutOfPrint
{
    [DefOf]
    public static class NeverOutOfPrintDefOf
    {
        public static SpecialThingFilterDef NeverOutOfPrint_AllowCopies;

        static NeverOutOfPrintDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(NeverOutOfPrintDefOf));
        }
    }
}
