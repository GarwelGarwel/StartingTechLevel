using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace StartingTechLevel
{
    public static class ResearchProjectUtility
    {
        public static IEnumerable<ResearchProjectDef> ResearchProjectsAtTechLevel(TechLevel techLevel) =>
            DefDatabase<ResearchProjectDef>.AllDefs.Where(researchProject => researchProject.techLevel == techLevel);
    }
}
