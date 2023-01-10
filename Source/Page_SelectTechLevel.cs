using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

using static StartingTechLevel.LogUtility;

namespace StartingTechLevel
{
    public class Page_SelectTechLevel : Page
    {
        bool defaultTechLevel = true;
        bool randomTechLevel = false;
        bool researchOlderTechs = true;
        TechLevel techLevel;
        bool grantStartingTechs = true;
        List<int> techsByLevel = new List<int>(7);

        public override string PageTitle => "Choose Tech Level";

        static FactionDef GetFactionDef(TechLevel techLevel)
        {
            switch (techLevel)
            {
                case TechLevel.Animal:
                    return FactionDefOf.PlayerBand;

                case TechLevel.Neolithic:
                    return FactionDefOf.PlayerTribe;

                case TechLevel.Medieval:
                    return FactionDefOf.PlayerKingdom;

                case TechLevel.Industrial:
                    return FactionDefOf.PlayerColony;

                case TechLevel.Spacer:
                    return FactionDefOf.PlayerSpaceColony;

                case TechLevel.Ultra:
                    return FactionDefOf.PlayerGlitterworldColony;

                case TechLevel.Archotech:
                    return FactionDefOf.PlayerArchotechColony;
            }
            return null;
        }

        protected override void DoNext()
        {
            if (!defaultTechLevel && Find.GameInitData.playerFaction != null)
            {
                if (randomTechLevel)
                {
                    techLevel = (TechLevel)Rand.RangeInclusive((int)TechLevel.Animal, (int)TechLevel.Archotech);
                    Log($"Randomly selected {techLevel.ToStringSafe()} tech level.");
                }
                else Log($"Setting tech level {techLevel}.");

                // Setting correct player faction type
                FactionDef factionDef = GetFactionDef(techLevel);
                if (factionDef != null)
                    Find.GameInitData.playerFaction.def = factionDef;
                else Find.GameInitData.playerFaction.def.techLevel = techLevel;
                Log($"Set player faction to {Find.GameInitData.playerFaction.def.defName}.");

                // Researching techs from previous levels
                if (researchOlderTechs)
                {
                    int rp = 0;
                    foreach (ResearchProjectDef researchProject in DefDatabase<ResearchProjectDef>.AllDefs.Where(researchProject => researchProject.techLevel < techLevel))
                    {
                        Find.ResearchManager.FinishProject(researchProject, doCompletionLetter: false);
                        rp++;
                    }
                    Log($"Finished {rp} research projects.");
                }
            }

            if (!grantStartingTechs)
                Find.GameInitData?.playerFaction?.def?.startingResearchTags?.Clear();

            base.DoNext();
        }

        public override void PreOpen()
        {
            int total = 0;
            techsByLevel.Add(0);
            for (TechLevel l = TechLevel.Neolithic; l <= TechLevel.Archotech; l++)
            {
                total += ResearchProjectUtility.ResearchProjectsAtTechLevel(l - 1).Count();
                techsByLevel.Add(total);
            }
            base.PreOpen();
        }

        public override void DoWindowContents(Rect rect)
        {
            DrawPageTitle(rect);
            Listing_Standard listing = new Listing_Standard();
            Text.Anchor = TextAnchor.UpperLeft;
            listing.verticalSpacing = 5;
            listing.Begin(GetMainRect(rect).LeftPart(0.3f));

            if (listing.RadioButton("Default", defaultTechLevel))
            {
                defaultTechLevel = true;
                randomTechLevel = false;
                techLevel = TechLevel.Undefined;
            }

            for (TechLevel l = TechLevel.Animal; l <= TechLevel.Archotech; l++)
                if (listing.RadioButton(l.ToStringHuman().CapitalizeFirst(), techLevel == l))
                {
                    defaultTechLevel = randomTechLevel = false;
                    techLevel = l;
                }

            if (listing.RadioButton("Random", randomTechLevel))
            {
                defaultTechLevel = false;
                randomTechLevel = true;
                techLevel = TechLevel.Undefined;
            }
            listing.Gap();

            FactionDef factionDef = defaultTechLevel ? Find.GameInitData?.playerFaction?.def : GetFactionDef(techLevel);
            if (factionDef == null || !factionDef.startingResearchTags.NullOrEmpty())
                listing.CheckboxLabeled("Grant starting technologies", ref grantStartingTechs, "If checked, you will start with several basic technologies already researched (e.g. Electricity for Industrial start). Only applies to Neolithic and Industrial starts.");

            if (randomTechLevel || (!defaultTechLevel && techsByLevel[(int)factionDef.techLevel - 1] > 0))
                listing.CheckboxLabeled("Complete previous levels' technologies", ref researchOlderTechs, "If checked, you will start with all technologies from previous tech levels already researched.");

            listing.End();
            listing.Begin(GetMainRect(rect).RightHalf());

            if (factionDef != null)
            {
                listing.Label($"Your faction will be {factionDef.LabelCap}.");
                if (!defaultTechLevel && researchOlderTechs)
                    listing.Label($"Start with {techsByLevel[(int)factionDef.techLevel - 1]} techs{(grantStartingTechs && !factionDef.startingResearchTags.NullOrEmpty() ? " (not including granted starting technologies)" : "")}.");
                if (ModsConfig.IdeologyActive && (!factionDef.disallowedMemes.NullOrEmpty() || !factionDef.disallowedPrecepts.NullOrEmpty()))
                    listing.Label(
                        $"{factionDef.disallowedMemes.Count.ToStringSafe()} memes and {factionDef.disallowedPrecepts.Count.ToStringSafe()} precepts disallowed.",
                        tooltip: $"Memes: {factionDef.disallowedMemes.Select(meme => meme.LabelCap.RawText).ToCommaList()}.\nPrecepts: {factionDef.disallowedPrecepts.Select(precept => precept.LabelCap.RawText).ToCommaList()}");
                listing.Label($"Forageability factor: {factionDef.forageabilityFactor.ToStringPercent()}");

            }

            listing.End();
            DoBottomButtons(rect);
        }
    }
}
