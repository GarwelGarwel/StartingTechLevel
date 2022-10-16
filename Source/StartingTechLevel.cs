using HarmonyLib;
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
            harmony = new Harmony("Garwel.StartingTechLevel");
            Log("StartingTechLevel initialized successfully.", LogLevel.Important);
        }
    }
}
