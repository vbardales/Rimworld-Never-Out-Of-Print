using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NeverOutOfPrint
{
    /// <summary>
    /// Makes one book into another book that reads the same.
    ///
    /// WHAT A COPY IS. A book's benefits do not live on its def: they are rolled once, when the
    /// book is generated, and stored on the instance. Verse.Book keeps the title, the blurb, the
    /// mental-break chance and the joy factor in private fields; each of its
    /// RimWorld.ReadingOutcomeDoer objects keeps its own rolled state - which skills and at what
    /// rate, which research projects, whether there is a quest waiting inside. Making a fresh
    /// thing of the same def and calling it a copy would re-roll all of that, and re-rolling is an
    /// exploit rather than a feature: print schematics until one happens to advance the project
    /// you actually wanted. So a copy takes that state across verbatim, quality included - quality
    /// is what caps how far a textbook can teach, so a copy that lost it would not read the same.
    ///
    /// WITH ONE EXCEPTION. BookOutcomeDoer_GiveQuest holds a one-off discovery, and Odyssey's map
    /// holds one with certainty (questChance 1). Duplicating it would print treasure. A copy
    /// therefore carries every repeatable benefit and no one-off discovery: printing a map does
    /// not print what is buried at the end of it. The blurb is rebuilt on the copy so that it
    /// stops promising a quest which is not in it.
    ///
    /// Copies are marked by <see cref="CompCopiedBook"/>, which is what the stockpile filters and
    /// the market-value penalty read.
    /// </summary>
    public static class BookCopyUtility
    {
        /// <summary>
        /// Verse.Book's own generated state. isOpen is deliberately absent - a book coming off the
        /// press is shut - and descCanBeInvalidated is set by hand afterwards.
        /// </summary>
        private static readonly string[] BookFieldNames =
        {
            "title",
            "descriptionFlavor",
            "description",
            "mentalBreakChancePerHour",
            "joyFactor",
            "generations"
        };

        private static readonly List<FieldInfo> BookFields = ResolveBookFields();

        private static readonly FieldInfo DescCanBeInvalidatedField =
            AccessTools.Field(typeof(Book), "descCanBeInvalidated");

        private static List<FieldInfo> ResolveBookFields()
        {
            var fields = new List<FieldInfo>(BookFieldNames.Length);
            foreach (string name in BookFieldNames)
            {
                FieldInfo field = AccessTools.Field(typeof(Book), name);
                if (field == null)
                {
                    Log.Error("[Never Out of Print] Verse.Book has no field '" + name +
                              "'; copies will not reproduce that part of the original.");
                    continue;
                }

                fields.Add(field);
            }

            return fields;
        }

        public static Book MakeCopy(Book source)
        {
            if (source == null)
            {
                return null;
            }

            if (!(ThingMaker.MakeThing(source.def, source.Stuff) is Book copy))
            {
                Log.Error("[Never Out of Print] " + source.def.defName + " did not make a Book; nothing copied.");
                return null;
            }

            // Quality first. CompQuality.SetQuality triggers Book.PostQualitySet, which generates a
            // whole fresh book - so it has to happen before the state is copied over, not after.
            CompQuality sourceQuality = source.TryGetComp<CompQuality>();
            CompQuality copyQuality = copy.TryGetComp<CompQuality>();
            if (sourceQuality != null && copyQuality != null)
            {
                copyQuality.SetQuality(sourceQuality.Quality, ArtGenerationContext.Colony);
            }

            CopyBookFields(source, copy);
            CopyDoers(source, copy);
            copy.TryGetComp<CompCopiedBook>()?.MarkCopied();
            return copy;
        }

        private static void CopyBookFields(Book source, Book copy)
        {
            foreach (FieldInfo field in BookFields)
            {
                field.SetValue(copy, field.GetValue(source));
            }

            // Force the detailed description to be rebuilt from the copied title, blurb and doers
            // the first time it is asked for; Verse.Book.EnsureDescriptionUpToDate does the work.
            // This is how the "there is a quest in this book" line leaves a copy.
            DescCanBeInvalidatedField?.SetValue(copy, true);
        }

        private static void CopyDoers(Book source, Book copy)
        {
            CompReadable sourceComp = source.BookComp;
            CompReadable copyComp = copy.BookComp;
            if (sourceComp == null || copyComp == null)
            {
                return;
            }

            // Both lists are built from the same CompProperties_Readable.doers, in order, so they
            // line up by index.
            List<ReadingOutcomeDoer> sourceDoers = sourceComp.Doers?.ToList();
            List<ReadingOutcomeDoer> copyDoers = copyComp.Doers?.ToList();
            if (sourceDoers == null || copyDoers == null)
            {
                return;
            }

            int count = Math.Min(sourceDoers.Count, copyDoers.Count);
            for (int i = 0; i < count; i++)
            {
                ReadingOutcomeDoer from = sourceDoers[i];
                ReadingOutcomeDoer to = copyDoers[i];
                if (from.GetType() != to.GetType())
                {
                    continue;
                }

                CopyDoerFields(from, to);
                StripOneOffDiscoveries(to);
            }
        }

        private static void CopyDoerFields(ReadingOutcomeDoer from, ReadingOutcomeDoer to)
        {
            // Walk the concrete type's own fields and those of any intermediate bases, stopping
            // short of the two framework classes: ReadingOutcomeDoer holds the parent thing and
            // its comp, and BookOutcomeDoer caches the parent's CompQuality. Copying either would
            // point the copy's doer back at the original book. A modded doer that caches its own
            // reference to the parent (or one of its comps) is not caught by this stop list - see
            // NeverOutOfPrint/BACKLOG.md, "Language: what a copy owes to the language of a book",
            // for a concrete case found while reading Rim Languages.
            for (Type type = from.GetType();
                 type != null && type != typeof(ReadingOutcomeDoer) && type != typeof(BookOutcomeDoer);
                 type = type.BaseType)
            {
                FieldInfo[] fields = type.GetFields(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                foreach (FieldInfo field in fields)
                {
                    if (field.IsLiteral || field.IsInitOnly)
                    {
                        continue;
                    }

                    field.SetValue(to, CloneValue(field.GetValue(from)));
                }
            }
        }

        /// <summary>
        /// A copy carries every benefit that can be had again and none that can be had only once.
        /// Cleared by field name rather than by rebuilding the doer, so that a renamed field is
        /// reported at the log instead of silently letting copies duplicate quests.
        /// </summary>
        private static void StripOneOffDiscoveries(ReadingOutcomeDoer doer)
        {
            if (!(doer is BookOutcomeDoer_GiveQuest))
            {
                return;
            }

            ClearField(doer, "hasQuest", false);
            ClearField(doer, "questDef", null);
            ClearField(doer, "quest", null);
            ClearField(doer, "giveNext", false);
        }

        private static void ClearField(object obj, string fieldName, object value)
        {
            FieldInfo field = AccessTools.Field(obj.GetType(), fieldName);
            if (field == null)
            {
                Log.ErrorOnce(
                    "[Never Out of Print] BookOutcomeDoer_GiveQuest has no field '" + fieldName +
                    "'; copies may duplicate quests.",
                    0x2C0DE000 ^ fieldName.GetHashCode());
                return;
            }

            field.SetValue(obj, value);
        }

        /// <summary>
        /// Deep enough to keep two books' collections apart, shallow enough to be safe on doer
        /// types this mod has never seen. Defs, types and strings are shared on purpose: they are
        /// the same object everywhere in the game.
        /// </summary>
        private static object CloneValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            Type type = value.GetType();
            if (type.IsValueType || value is string || value is Def || value is Type)
            {
                return value;
            }

            if (value is IDictionary dictionary)
            {
                if (!(Activator.CreateInstance(type) is IDictionary clone))
                {
                    return value;
                }

                foreach (DictionaryEntry entry in dictionary)
                {
                    clone[CloneValue(entry.Key)] = CloneValue(entry.Value);
                }

                return clone;
            }

            if (value is IList list)
            {
                if (!(Activator.CreateInstance(type) is IList clone))
                {
                    return value;
                }

                foreach (object item in list)
                {
                    clone.Add(CloneValue(item));
                }

                return clone;
            }

            // Anything else - a Quest, a Pawn, an Ideo - is a reference into the world, and a copy
            // should point at it exactly as the original does.
            return value;
        }
    }
}
