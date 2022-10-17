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
        /// Adds Select Tech Level page
        /// </summary>
        public static void Scenario_GetFirstConfigPage(ref Page __result)
        {
            Log("Scenario_GetFirstConfigPage");
            Page previousPage = __result;
            while (previousPage != null && !(previousPage is Page_SelectStartingSite))
                previousPage = previousPage.next;
            if (previousPage == null)
            {
                Log($"Page_SelectStartingSite not found. First page: {__result?.PageTitle}.", LogLevel.Important);
                return;
            }
            Page_SelectTechLevel newPage = new Page_SelectTechLevel();
            newPage.prev = previousPage;
            newPage.next = previousPage.next;
            previousPage.next = newPage;
            newPage.next.prev = newPage;
            Log("Select Tech Level page added.");
        }
    }
}
