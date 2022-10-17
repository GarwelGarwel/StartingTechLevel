using HarmonyLib;
using RimWorld;
using Verse;

using static StartingTechLevel.LogUtility;

namespace StartingTechLevel
{
    [StaticConstructorOnStartup]
    public static class StartingTechLevel
    {
        static Harmony harmony;

        static StartingTechLevel()
        {
            if (harmony != null)
                return;

            harmony = new Harmony("Garwel.StartingTechLevel");
            harmony.Patch(
                AccessTools.Method($"RimWorld.Scenario:GetFirstConfigPage"),
                postfix: new HarmonyMethod(typeof(StartingTechLevel).GetMethod("Scenario_GetFirstConfigPage")));
            Log("StartingTechLevel initialized successfully.", LogLevel.Important);
        }

        /// <summary>
        /// Adds Select Tech Level page after the first one
        /// </summary>
        /// <param name="__result"></param>
        public static void Scenario_GetFirstConfigPage(ref Page __result)
        {
            Log("Scenario_GetFirstConfigPage");
            Page storytellerPage = __result;
            while (storytellerPage != null && !(storytellerPage is Page_SelectStoryteller))
                storytellerPage = storytellerPage.next;
            if (storytellerPage == null)
            {
                Log($"Select Storyteller page not found. First page: {__result?.PageTitle}.", LogLevel.Important);
                return;
            }
            Page_SelectTechLevel newPage = new Page_SelectTechLevel();
            newPage.prev = storytellerPage;
            newPage.next = storytellerPage.next;
            storytellerPage.next = newPage;
            newPage.next.prev = newPage;
            Log("Select Tech Level page added.");
        }
    }
}
