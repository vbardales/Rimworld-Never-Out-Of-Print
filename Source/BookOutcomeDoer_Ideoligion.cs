using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace NeverOutOfPrint
{
    public class BookOutcomeProperties_Ideoligion : BookOutcomeProperties
    {
        /// <summary>Certainty per reading tick for a reader of the book's own ideoligion.</summary>
        public float certaintyPerTickReassuring = 1.5e-05f;

        /// <summary>Certainty per reading tick, as a loss, for a reader of any other.</summary>
        public float certaintyPerTickOpposing = 1e-05f;

        /// <summary>
        /// How much of the above a book of each quality delivers, awful through legendary. Awful is
        /// negative on purpose: a badly argued tract pushes the reader the wrong way, which is why
        /// it also steadies a reader it was meant to unsettle.
        /// </summary>
        public SimpleCurve qualityFactor = new SimpleCurve
        {
            new CurvePoint(0f, -0.25f),
            new CurvePoint(1f, 0.5f),
            new CurvePoint(2f, 1f),
            new CurvePoint(3f, 1.5f),
            new CurvePoint(4f, 2f),
            new CurvePoint(5f, 3.5f),
            new CurvePoint(6f, 5.5f)
        };

        public override Type DoerClass => typeof(BookOutcomeDoer_Ideoligion);
    }

    /// <summary>
    /// Reading an ideoligion book moves the reader's certainty: up in their own faith, down in any
    /// other, and a reader whose certainty runs out converts to the book's.
    ///
    /// This is the whole of the ideoligion half. In RimWorld 1.3, when the mod this replaces was
    /// written, it took a Thing subclass, a JobDef, a JobDriver and a JoyGiver to arrange for a
    /// pawn to sit down with a book at all. Since 1.5 the base game does all of that itself, and
    /// RimWorld.JobDriver_Reading ticks every ReadingOutcomeDoer on whatever is being read - so
    /// what is left to write is only what happens on the tick.
    ///
    /// Nothing in RimWorld 1.6 Ideology already covers this. Certainty moves through rituals, the
    /// moral guide's role and the CertaintyLossFactor stat; reading is not one of its levers.
    /// </summary>
    public class BookOutcomeDoer_Ideoligion : BookOutcomeDoer
    {
        /// <summary>
        /// Certainty earned but not yet handed over. Pawn_IdeoTracker.OffsetCertainty throws a
        /// floating "certainty" mote on every call, so applying a ten-thousandth of a percent per
        /// tick would bury the colonist in text. It is banked and paid out in visible steps: about
        /// one for an ordinary book read end to end, half a dozen for a legendary one.
        /// </summary>
        private const float ApplyThreshold = 0.05f;

        private Ideo ideoligion;
        private float pending;

        /// <summary>
        /// Who the bank belongs to. The doer lives on the book, not on the reader, so without this
        /// a colonist who put the book down at four fifths of a step would leave that fifth for
        /// whoever picked it up next.
        /// </summary>
        private Pawn pendingFor;

        public new BookOutcomeProperties_Ideoligion Props => (BookOutcomeProperties_Ideoligion)props;

        public Ideo Ideoligion => ideoligion;

        private static bool CertaintyIsMeaningful =>
            ModsConfig.IdeologyActive && Find.IdeoManager != null && !Find.IdeoManager.classicMode;

        public override void Reset()
        {
            ideoligion = null;
            pending = 0f;
            pendingFor = null;
        }

        /// <summary>
        /// A book argues for its author's faith, or - when the game is handing one to a trader or
        /// dropping it in a ruin - for whichever faith the world happens to hold.
        /// </summary>
        public override void OnBookGenerated(Pawn author = null)
        {
            if (!CertaintyIsMeaningful)
            {
                return;
            }

            ideoligion = author?.Ideo ?? Find.IdeoManager.IdeosListForReading?.RandomElementWithFallback();
        }

        public override bool DoesProvidesOutcome(Pawn reader)
        {
            if (ideoligion == null || !CertaintyIsMeaningful || reader?.ideo == null || reader.Ideo == null)
            {
                return false;
            }

            // Only when reading it would leave this pawn steadier than it found them - which is
            // their own faith's book, or a badly enough argued rival's. The base game reads this
            // to decide which book a colonist reaches for on their own
            // (Verse.BookUtility.TryGetRandomBookToRead), and a colonist should not go looking for
            // the volume that will talk them out of their religion. Left lying around it can still
            // be picked up as a fallback, and a player can always order the read; the harm lands
            // either way, because OnReadingTick does not consult this.
            return CertaintyPerTick(reader) > 0f && reader.ideo.Certainty < 1f;
        }

        public override void OnReadingTick(Pawn reader, float factor)
        {
            if (ideoligion == null || !CertaintyIsMeaningful || reader?.ideo == null || reader.Ideo == null)
            {
                return;
            }

            if (pendingFor != reader)
            {
                // A different reader: whatever the last one had banked was under a full step and
                // is not theirs to hand over.
                pendingFor = reader;
                pending = 0f;
            }

            pending += CertaintyPerTick(reader) * factor;
            if (Mathf.Abs(pending) < ApplyThreshold)
            {
                return;
            }

            float amount = pending;
            pending = 0f;

            if (amount < 0f && reader.Ideo != ideoligion)
            {
                // The base game's own conversion path: it records the history event, remembers the
                // ideoligion the pawn came from, and resets certainty on the far side.
                reader.ideo.IdeoConversionAttempt(-amount, ideoligion);
                return;
            }

            reader.ideo.OffsetCertainty(amount);
        }

        private float CertaintyPerTick(Pawn reader)
        {
            bool reassuring = reader.Ideo == ideoligion;
            float amount = (reassuring ? Props.certaintyPerTickReassuring : -Props.certaintyPerTickOpposing)
                           * Props.qualityFactor.Evaluate((int)Quality);

            // A pawn who is hard to shake is hard to shake by a book too.
            if (!reassuring)
            {
                amount *= reader.GetStatValue(StatDefOf.CertaintyLossFactor);
            }

            return amount;
        }

        public override string GetBenefitsString(Pawn reader = null)
        {
            if (ideoligion == null)
            {
                return null;
            }

            string faith = ideoligion.name.ApplyTag(ideoligion).Resolve();
            if (reader?.Ideo == null || !CertaintyIsMeaningful)
            {
                return " - " + "NeverOutOfPrint_BookArguesFor".Translate(faith);
            }

            float perHour = Mathf.Abs(CertaintyPerTick(reader)) * 2500f;
            string line = reader.Ideo == ideoligion
                ? "NeverOutOfPrint_BookReassures".Translate(faith, perHour.ToStringPercent("0.0"))
                : "NeverOutOfPrint_BookUnsettles".Translate(faith, perHour.ToStringPercent("0.0"));

            // An awful tract has a negative quality factor, so it does the opposite of what it set
            // out to do. Saying which way it actually pushes is more useful than the label.
            if (CertaintyPerTick(reader) < 0f == (reader.Ideo == ideoligion))
            {
                line += " (" + "NeverOutOfPrint_BookBackfires".Translate() + ")";
            }

            return " - " + line;
        }

        public override bool BenefitDetailsCanChange(Pawn reader = null)
        {
            // The line depends on who is holding it, and on their certainty-loss stat.
            return true;
        }

        /// <summary>
        /// Feeds the ideoligion's own name, its member name and its plural into the title and blurb
        /// grammars, through the game's own GrammarUtility.RulesForIdeo.
        ///
        /// These arrive as rule strings and not as a rule pack on purpose. Verse.Book.GenerateBook
        /// resolves each doer's rule PACKS before it collects that doer's rule STRINGS, so a pack
        /// that said "subject -&gt; [IDEO_name]" would be resolved while IDEO_name was still out of
        /// scope. The rule packs in Defs/RulePacks.xml use the IDEO_ symbols directly instead.
        /// </summary>
        public override IEnumerable<Rule_String> GetTopicRuleStrings()
        {
            if (ideoligion != null)
            {
                foreach (Rule rule in GrammarUtility.RulesForIdeo("IDEO", ideoligion))
                {
                    if (rule is Rule_String ruleString)
                    {
                        yield return ruleString;
                    }
                }

                yield break;
            }

            // Ideology off, classic mode, or a world with no ideoligions: the grammar still has to
            // resolve, so give it something neutral rather than leaving the symbols dangling.
            yield return new Rule_String("IDEO_name", "NeverOutOfPrint_NamelessFaith".Translate().Resolve());
            yield return new Rule_String("IDEO_memberName", "NeverOutOfPrint_NamelessBeliever".Translate().Resolve());
            yield return new Rule_String("IDEO_memberNamePlural", "NeverOutOfPrint_NamelessBelievers".Translate().Resolve());
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref ideoligion, "ideoligion");
            Scribe_Values.Look(ref pending, "pendingCertainty", 0f);
            Scribe_References.Look(ref pendingFor, "pendingCertaintyFor");
        }
    }
}
