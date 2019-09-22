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
                if(IncidentInterceptorUtility.Intercept_Raid_EdgeWalkIn(parms))
                    __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns")]
    static class Patch_PawnGroupMakerUtility_GeneratePawns
    {
        [HarmonyPrefix]
        static bool Prefix(ref IEnumerable<Pawn> __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_PawnGeneration == PawnPatchType.Generate)
                return true;
            if (IncidentInterceptorUtility.IsIntercepting_PawnGeneration == PawnPatchType.ReturnTempList)
                __result = IncidentInterceptorUtility.tmpPawnList;
            else
                __result = new List<Pawn>();
            IncidentInterceptorUtility.IsIntercepting_PawnGeneration = PawnPatchType.Generate;
            return false;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival), "TryExecuteWorker")]
    static class Patch_IncidentWorker_TraderCaravanArrival_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_TraderCaravanArrival __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_TraderCaravan_Worker)
                IncidentInterceptorUtility.CreateIncidentCaraven<InterceptedIncident_HumanCrowd_TraderCaravan>(IncidentDefOf.TraderCaravanArrival, parms);
            return true;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_TravelerGroup), "TryExecuteWorker")]
    static class IncidentWorker_TravelerGroup_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_TravelerGroup __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_TravelerGroup)
                IncidentInterceptorUtility.CreateIncidentCaraven<InterceptedIncident_HumanCrowd_TravelerGroup>(IncidentDefOf.TravelerGroup, parms);
            return true;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_VisitorGroup), "TryExecuteWorker")]
    static class IncidentWorker_VisitorGroup_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_VisitorGroup __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_VisitorGroup)
                IncidentInterceptorUtility.CreateIncidentCaraven<InterceptedIncident_HumanCrowd_VisitorGroup>(IncidentDefOf.VisitorGroup, parms);
            return true;
        }
    }

}
