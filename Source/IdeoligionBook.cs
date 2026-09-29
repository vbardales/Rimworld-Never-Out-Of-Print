using RimWorld;
using UnityEngine;
using Verse;

namespace NeverOutOfPrint
{
    /// <summary>
    /// A book with an ideoligion's own symbol stamped on the cover.
    ///
    /// This is the only reason the class exists. Everything a reader gets from the book comes from
    /// <see cref="BookOutcomeDoer_Ideoligion"/>, and everything about picking it up, carrying it to
    /// a chair and reading it is the base game's; but an outcome doer has no draw hook, and the
    /// cover art is deliberately blank so that two of these are told apart by their symbol.
    ///
    /// Only DrawAt is overridden. Items default to DrawerType.RealtimeOnly, and Verse.Book relies
    /// on that already to swap in the open-book graphic, so there is no map mesh print to match.
    /// </summary>
    public class IdeoligionBook : Book
    {
        private static readonly Vector3 IconOffset = new Vector3(0.0125f, 0.01f, 0.08f);
        private static readonly Vector2 IconSize = new Vector2(0.5f, 0.5f);

        private Material iconCached;
        private Ideo iconCachedFor;

        private Ideo Ideoligion => BookComp?.GetDoer<BookOutcomeDoer_Ideoligion>()?.Ideoligion;

        private Material IconFor(Ideo ideo)
        {
            if (iconCached == null || iconCachedFor != ideo)
            {
                // Halved towards a muted rose so a bright ideoligion colour does not read as a
                // separate light source sitting on the floor.
                Color tint = Color.Lerp(ideo.Color, new Color32(133, 89, 113, 255), 0.5f);
                iconCached = MaterialPool.MatFrom(ideo.Icon, ShaderDatabase.Cutout, tint);
                iconCachedFor = ideo;
            }

            return iconCached;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);

            // Not while it is open: the open-book graphic is a different shape and the symbol
            // would float over the pages.
            if (IsOpen)
            {
                return;
            }

            Ideo ideo = Ideoligion;
            if (ideo?.Icon == null)
            {
                return;
            }

            Graphics.DrawMesh(MeshPool.plane05, drawLoc + IconOffset, Quaternion.identity, IconFor(ideo), 0);
        }
    }
}
