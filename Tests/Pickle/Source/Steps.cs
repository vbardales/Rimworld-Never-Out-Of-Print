using System.Linq;
using System.Reflection;
using RimWorld;
using RimWorks.Pickle;
using Verse;

namespace NeverOutOfPrint.PickleSteps
{
    /// <summary>
    /// What only a running game can show about the two mechanics: that a real recipe leaves the
    /// original untouched and marks the copy (comp state and computed market value are both per-Thing
    /// instance data, not something the offline harness or a def-level Pickle step can see), and that
    /// reading actually moves a Pawn's Ideo certainty in the documented direction. The XML/reflection
    /// wiring (recipe users, research prerequisites, RulePackDef references, translation keys) is
    /// proved without the game by Tests/test_resources.py and is not repeated here.
    /// </summary>
    [PickleSteps]
    public class BookSteps
    {
        /// <summary>Cached, since BookOutcomeDoer_Ideoligion.ideoligion has no public setter: Pickle
        /// scenarios need to force a rival faith the same way OnBookGenerated(author) would if a
        /// generated author Pawn of that faith existed, which the fixture does not guarantee.</summary>
        private static readonly FieldInfo IdeoligionField =
            typeof(BookOutcomeDoer_Ideoligion).GetField("ideoligion", BindingFlags.NonPublic | BindingFlags.Instance);

        private sealed class ReadingContext
        {
            public BookOutcomeDoer_Ideoligion Doer;
            public Pawn Reader;
            public float CertaintyBefore;
        }

        private static ThingDef Def(PickleContext ctx, string name)
        {
            var def = DefDatabase<ThingDef>.GetNamedSilentFail(name);
            ctx.Require(def != null, $"no ThingDef named '{name}'");
            return def;
        }

        private static Map CurrentMap(PickleContext ctx)
        {
            var map = Find.CurrentMap;
            ctx.Require(map != null, "no current map");
            return map;
        }

        private static Pawn FindPawn(PickleContext ctx, string name)
        {
            var pawn = CurrentMap(ctx).mapPawns.AllPawns.FirstOrDefault(p => p.Name?.ToStringShort == name);
            ctx.Require(pawn != null, $"no pawn named '{name}' on the map");
            return pawn;
        }

        [Then("the newest {string} on the map is marked as a printed copy")]
        public void NewestIsCopy(PickleContext ctx, string defName)
        {
            var def = Def(ctx, defName);
            var things = CurrentMap(ctx).listerThings.ThingsOfDef(def).OrderByDescending(t => t.thingIDNumber).ToList();
            ctx.Require(things.Count > 0, $"no '{defName}' on the map");
            var comp = things[0].TryGetComp<CompCopiedBook>();
            ctx.Require(comp != null, $"newest '{defName}' (id {things[0].thingIDNumber}) has no CompCopiedBook");
            ctx.Require(comp.Copied, $"newest '{defName}' (id {things[0].thingIDNumber}) is not marked as a copy");
        }

        [Then("the newest {string} on the map is worth about a tenth of the oldest one to a trader")]
        public void NewestIsWorthATenth(PickleContext ctx, string defName)
        {
            var def = Def(ctx, defName);
            var things = CurrentMap(ctx).listerThings.ThingsOfDef(def).OrderBy(t => t.thingIDNumber).ToList();
            ctx.Require(things.Count >= 2, $"need at least two '{defName}' on the map, found {things.Count}");
            Thing original = things[0];
            Thing copy = things[things.Count - 1];
            float originalValue = original.GetStatValue(StatDefOf.MarketValue);
            float copyValue = copy.GetStatValue(StatDefOf.MarketValue);
            ctx.Require(originalValue > 0f, "the original's market value is 0, cannot compute a ratio");
            float ratio = copyValue / originalValue;
            ctx.Require(ratio > 0.08f && ratio < 0.12f,
                $"expected the copy to be worth ~0.1x the original, got {ratio:0.###} (original {originalValue}, copy {copyValue})");
        }

        private void SpawnIdeoligionBook(PickleContext ctx, string pawnName, bool sameFaith)
        {
            ctx.Require(ModsConfig.IdeologyActive, "Ideology is not active");
            var pawn = FindPawn(ctx, pawnName);
            ctx.Require(pawn.ideo?.Ideo != null, $"'{pawnName}' has no ideoligion");

            Ideo targetIdeo = sameFaith
                ? pawn.ideo.Ideo
                : Find.IdeoManager.IdeosListForReading?.FirstOrDefault(i => i != pawn.ideo.Ideo);
            ctx.Require(targetIdeo != null, sameFaith
                ? $"'{pawnName}' has no ideoligion"
                : "no rival ideoligion exists in this world to write a book for");

            var def = Def(ctx, "NeverOutOfPrint_IdeoligionBook");
            var thing = (Book)ThingMaker.MakeThing(def);
            var doer = thing.BookComp?.GetDoer<BookOutcomeDoer_Ideoligion>();
            ctx.Require(doer != null, "spawned book carries no BookOutcomeDoer_Ideoligion");

            // Sets doer.ideoligion via the private field: OnBookGenerated(Pawn author) reads
            // author?.Ideo, and no fixture pawn is guaranteed to hold the rival faith it would need
            // to author a book for it. This is the same field the shipped code itself assigns.
            IdeoligionField.SetValue(doer, targetIdeo);

            GenSpawn.Spawn(thing, pawn.Position, pawn.Map);
            ctx.Set(new ReadingContext { Doer = doer, Reader = pawn, CertaintyBefore = pawn.ideo.Certainty });
        }

        [When("an ideoligion book for {string}'s own faith is spawned near {string}")]
        public void SpawnOwnFaithBook(PickleContext ctx, string pawnNameAgain, string pawnName)
        {
            SpawnIdeoligionBook(ctx, pawnName, sameFaith: true);
        }

        [When("an ideoligion book for a rival faith is spawned near {string}")]
        public void SpawnRivalFaithBook(PickleContext ctx, string pawnName)
        {
            SpawnIdeoligionBook(ctx, pawnName, sameFaith: false);
        }

        [When("{string} reads it for {int} ticks")]
        public void ReadsFor(PickleContext ctx, string pawnName, int ticks)
        {
            var reading = ctx.Get<ReadingContext>();
            ctx.Require(reading.Reader.Name?.ToStringShort == pawnName,
                $"the spawned book was for '{reading.Reader.Name?.ToStringShort}', not '{pawnName}'");
            for (int i = 0; i < ticks; i++)
            {
                reading.Doer.OnReadingTick(reading.Reader, 1f);
            }
        }

        [Then("{string}'s certainty increased")]
        public void CertaintyIncreased(PickleContext ctx, string pawnName)
        {
            var reading = ctx.Get<ReadingContext>();
            float after = reading.Reader.ideo.Certainty;
            ctx.Require(after > reading.CertaintyBefore,
                $"'{pawnName}' certainty was {reading.CertaintyBefore}, is now {after} - expected an increase");
        }

        [Then("{string}'s certainty decreased")]
        public void CertaintyDecreased(PickleContext ctx, string pawnName)
        {
            var reading = ctx.Get<ReadingContext>();
            float after = reading.Reader.ideo.Certainty;
            ctx.Require(after < reading.CertaintyBefore,
                $"'{pawnName}' certainty was {reading.CertaintyBefore}, is now {after} - expected a decrease");
        }
    }
}
