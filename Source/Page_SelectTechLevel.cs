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
        TechLevel techLevel;
        bool grantStartingTechs = true;
        List<int> techsByLevel = new List<int>(7);

        public override string PageTitle => "Choose Tech Level";

        void SetTechLevel(Faction faction)
        {
            // Setting tech level
            TechLevel oldTechLevel = faction.def.techLevel;
            faction.def.techLevel = techLevel;
            Log($"Player faction {faction.Name} changed tech level from {oldTechLevel.ToStringSafe()} to {faction.def.techLevel.ToStringSafe()}");

            // Researching techs from previous levels
            foreach (ResearchProjectDef researchProject in DefDatabase<ResearchProjectDef>.AllDefs.Where(researchProject => researchProject.techLevel < techLevel))
            {
                Log($"Finishing research project {researchProject} ({researchProject.LabelCap}).");
                Find.ResearchManager.FinishProject(researchProject, doCompletionLetter: false);
            }

            // Setting correct starting research tags
            faction.def.startingResearchTags.Clear();
            if (grantStartingTechs)
                if (techLevel >= TechLevel.Industrial)
                    faction.def.startingResearchTags.Add(ResearchProjectTagDefOf.ClassicStart);
                else if (techLevel >= TechLevel.Neolithic)
                    faction.def.startingResearchTags.Add(DefDatabase<ResearchProjectTagDef>.GetNamed("TribalStart"));
        }

        protected override void DoNext()
        {
            Faction playerFaction = Find.GameInitData.playerFaction;
            if (!defaultTechLevel && playerFaction != null)
                SetTechLevel(playerFaction);
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

        bool RadioButtonLabeled(ref Rect rect, string labelText, bool chosen, TipSignal tipSignal)
        {
            bool res = Widgets.RadioButton(rect.x, rect.y, chosen);
            float y = rect.y;
            Widgets.Label(rect.x + Widgets.RadioButtonSize + 5, ref y, rect.width - Widgets.RadioButtonSize - 5, labelText, tipSignal);
            rect.y += Widgets.RadioButtonSize + 15;
            rect.height -= Widgets.RadioButtonSize + 15;
            return res;
        }

        public override void DoWindowContents(Rect rect)
        {
            DrawPageTitle(rect);
            Rect mainRect = GetMainRect(rect);
            Widgets.BeginGroup(mainRect);
            Text.Anchor = TextAnchor.UpperLeft;

            if (RadioButtonLabeled(ref mainRect, "Default", defaultTechLevel, "Vanilla start"))
            {
                defaultTechLevel = true;
                techLevel = TechLevel.Undefined;
            }
            for (TechLevel l = TechLevel.Animal; l <= TechLevel.Archotech; l++)
                if (RadioButtonLabeled(ref mainRect, l.ToStringHuman().CapitalizeFirst(), techLevel == l, new TipSignal($"Start with {techsByLevel[(int)l - 1]} techs")))
                {
                    defaultTechLevel = false;
                    techLevel = l;
                }

            if (techLevel == TechLevel.Neolithic || techLevel == TechLevel.Industrial)
                Widgets.CheckboxLabeled(mainRect, "Grant starting technologies", ref grantStartingTechs);

            Widgets.EndGroup();
            DoBottomButtons(rect);
        }
    }
}
