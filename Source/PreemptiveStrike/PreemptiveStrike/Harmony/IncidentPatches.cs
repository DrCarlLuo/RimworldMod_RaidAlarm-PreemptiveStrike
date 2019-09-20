using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using RimWorld;
using PreemptiveStrike.Interceptor;
using Verse;

namespace PreemptiveStrike.Harmony
{
    [HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeWalkIn), "TryResolveRaidSpawnCenter")]
    static class Patch_EdgeWalkIn_TryResolveRaidSpawnCenter
    {
        [HarmonyPostfix]
        static void Postfix(PawnsArrivalModeWorker_EdgeWalkIn __instance, IncidentParms parms, ref bool __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_IncidentExcecution)
            {
                IncidentInterceptorUtility.Intercept_Raid_EdgeWalkIn(parms);
                __result = false;
            }
        }
    }
}
