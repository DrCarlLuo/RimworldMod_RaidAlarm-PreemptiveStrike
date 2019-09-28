using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using RimWorld;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.UI;
using RimWorld.Planet;
using Verse;

namespace PreemptiveStrike.Harmony
{
    [HarmonyPatch(typeof(Building_CommsConsole), "GetCommTargets")]
    static class Patch_Building_CommsConsole_GetCommTargets
    {
        [HarmonyPostfix]
        static void PostFix(Building_CommsConsole __instance, ref IEnumerable<ICommunicable> __result)
        {
            __result = __result.Concat(IncidentCaravanUtility.GetAllCommunicableCaravan());
        }
    }
}
