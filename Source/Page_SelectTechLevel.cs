using RimWorld;
using UnityEngine;
using Verse;

using static StartingTechLevel.LogUtility;

namespace StartingTechLevel
{
    public class Page_SelectTechLevel : Page
    {
        bool defaultTechLevel = true;
        TechLevel techLevel;

        public override string PageTitle => "Choose Tech Level";

        void SetTechLevel(Faction faction, TechLevel techLevel)
        {
            TechLevel oldTechLevel = faction.def.techLevel;
            faction.def.techLevel = techLevel;
            Find.ResearchManager.ResetAllProgress();

            foreach (ResearchProjectDef researchProject in DefDatabase<ResearchProjectDef>.AllDefs)
                if (researchProject.techLevel < techLevel)
                {
                    Log($"Finishing research project {researchProject} ({researchProject.LabelCap}).");
                    Find.ResearchManager.FinishProject(researchProject, doCompletionLetter: false);
                }

            Log($"Player faction {faction.Name} changed tech level from {oldTechLevel.ToStringSafe()} to {faction.def.techLevel.ToStringSafe()}");
        }

        protected override void DoNext()
        {
            Log($"DoNext (defaultTechLevel: {defaultTechLevel}; techLevel: {techLevel})");
            Faction playerFaction = Find.GameInitData.playerFaction;
            if (!defaultTechLevel && playerFaction != null)
                SetTechLevel(playerFaction, techLevel);
            base.DoNext();
        }

        bool RadioButtonLabeled(ref Rect rect, string labelText, bool chosen)
        {
            bool res = Widgets.RadioButton(rect.x, rect.y, chosen);
            float y = rect.y;
            Widgets.Label(rect.x + Widgets.RadioButtonSize + 5, ref y, rect.width - Widgets.RadioButtonSize - 5, labelText);
            rect.y = y + Widgets.RadioButtonSize + 5;
            return res;
        }

        public override void DoWindowContents(Rect rect)
        {
            DrawPageTitle(rect);
            Rect mainRect = GetMainRect(rect);
            Widgets.BeginGroup(mainRect);
            if (RadioButtonLabeled(ref mainRect, "Default", defaultTechLevel))
            {
                defaultTechLevel = true;
                techLevel = TechLevel.Undefined;
            }
            for (TechLevel l = TechLevel.Animal; l < TechLevel.Archotech; l++)
                if (RadioButtonLabeled(ref mainRect, l.ToStringHuman().CapitalizeFirst(), techLevel == l))
                {
                    defaultTechLevel = false;
                    techLevel = l;
                }
            Widgets.EndGroup();
            DoBottomButtons(rect);
        }
    }
}
