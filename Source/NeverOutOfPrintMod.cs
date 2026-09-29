using HarmonyLib;
using Verse;

namespace NeverOutOfPrint
{
    /// <summary>
    /// Applies the mod's Harmony patches. There are three, and each replaces something the game
    /// gives no other hook for: producing a copy instead of a fresh thing, steering which book a
    /// "fewest copies" bill reaches for, and telling a freshly written ideoligion book who wrote
    /// it.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class NeverOutOfPrintMod
    {
        static NeverOutOfPrintMod()
        {
            new Harmony("nelim.neveroutofprint").PatchAll();
        }
    }
}
