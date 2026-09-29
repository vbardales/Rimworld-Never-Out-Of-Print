using Verse;

namespace NeverOutOfPrint
{
    /// <summary>
    /// Records whether a book came off a printing press. Patches/CopiedBooks.xml puts this comp on
    /// every def that carries a <see cref="RimWorld.CompProperties_Book"/>, including books added
    /// by other mods, so "copy" and "original" are meaningful for the whole library and not only
    /// for the base game's four kinds.
    /// </summary>
    public class CompCopiedBook : ThingComp
    {
        private bool copied;

        public bool Copied => copied;

        public void MarkCopied()
        {
            copied = true;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref copied, "copied", defaultValue: false);
        }

        // Books never stack (Book.CanStackWith returns false unconditionally), so this only
        // matters for a modded readable that does. Keeping copies and originals apart in a stack
        // is the whole point of the flag.
        public override bool AllowStackWith(Thing other)
        {
            return other.TryGetComp<CompCopiedBook>()?.copied == copied;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompCopiedBook comp = piece.TryGetComp<CompCopiedBook>();
            if (comp != null)
            {
                comp.copied = copied;
            }
        }

        public override string CompInspectStringExtra()
        {
            return copied ? "NeverOutOfPrint_BookIsCopy".Translate().Resolve() : null;
        }
    }
}
