using RimWorld;
using UnityEngine;
using Verse;

using static StartingTechLevel.LogUtility;

namespace StartingTechLevel
{
    public class Page_SelectTechLevel : Page
    {
        public override string PageTitle => "Choose Tech Level";

        public override void DoWindowContents(Rect rect)
        {
            DrawPageTitle(rect);
            Widgets.BeginGroup(GetMainRect(rect));
            Widgets.EndGroup();
            DoBottomButtons(rect);
        }
    }
}
