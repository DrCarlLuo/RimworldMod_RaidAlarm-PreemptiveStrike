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
    [HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeDrop), "TryResolveRaidSpawnCenter")]
    class Patch_EdgeDrop_TryResolveRaidSpawnCenter
    {
        [HarmonyPostfix]
        static void PostFix(IncidentParms parms, ref bool __result)
        {
            if (IncidentInterceptorUtility.isIntercepting_EdgeDrop)
                __result = !IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_EdgeDrop>(IncidentDefOf.RaidEnemy, parms);
        }
    }

    [HarmonyPatch(typeof(PawnsArrivalModeWorker_CenterDrop), "TryResolveRaidSpawnCenter")]
    class Patch_CenterDrop_TryResolveRaidSpawnCenter
    {
        [HarmonyPostfix]
        static void PostFix(IncidentParms parms, ref bool __result)
        {
            if (IncidentInterceptorUtility.isIntercepting_CenterDrop)
                __result = !IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_CenterDrop>(IncidentDefOf.RaidEnemy, parms);
        }
    }

    [HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeDropGroups), "TryResolveRaidSpawnCenter")]
    class Patch_EdgeDropGroups_TryResolveRaidSpawnCenter
    {
        [HarmonyPostfix]
        static void PostFix(IncidentParms parms, ref bool __result)
        {
            if (IncidentInterceptorUtility.isIntercepting_EdgeDropGroup)
                __result = !IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_EdgeDropGroup>(IncidentDefOf.RaidEnemy, parms);
        }
    }

    [HarmonyPatch(typeof(PawnsArrivalModeWorker_RandomDrop), "TryResolveRaidSpawnCenter")]
    class Patch_RandomDrop_TryResolveRaidSpawnCenter
    {
        [HarmonyPostfix]
        static void PostFix(IncidentParms parms, ref bool __result)
        {
            if (IncidentInterceptorUtility.isIntercepting_RandomDrop)
                __result = !IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_RandomDrop>(IncidentDefOf.RaidEnemy, parms);
        }
    }

    [HarmonyPatch(typeof(CellFinderLoose), "TryFindSkyfallerCell")]
    class Patch_CellFinderLoose_TryFindSkyfallerCell
    {
        [HarmonyPrefix]
        static bool PreFix(ref IntVec3 cell, ref bool __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose == GeneratorPatchFlag.Generate)
            {
                return true;
            }
            else if (IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose == GeneratorPatchFlag.ReturnZero)
            {
                cell = IntVec3.Zero;
                __result = false;
                return false;
            }
            else
            {
                cell = IncidentInterceptorUtility.tempSkyfallerCellLoose;
                __result = true;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(InfestationCellFinder), "TryFindCell")]
    class Patch_InfestationCellFinder_TryFindCell
    {
        [HarmonyPrefix]
        static bool Prefix(ref IntVec3 cell, ref bool __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_InfestationCell == GeneratorPatchFlag.Generate)
            {
                return true;
            }
            else if (IncidentInterceptorUtility.IsIntercepting_InfestationCell == GeneratorPatchFlag.ReturnZero)
            {
                cell = IntVec3.Zero;
                __result = false;
                return false;
            }
            else
            {
                cell = IncidentInterceptorUtility.tempInfestationCell;
                __result = true;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(DropCellFinder), "RandomDropSpot")]
    class Patch_DropCellFinder_RandomDropSpot
    {
        [HarmonyPrefix]
        static bool Prefix(ref IntVec3 __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_RandomDropSpot == GeneratorPatchFlag.Generate)
            {
                return true;
            }
            else if (IncidentInterceptorUtility.IsIntercepting_RandomDropSpot == GeneratorPatchFlag.ReturnZero)
            {
                __result = IntVec3.Zero;
                return false;
            }
            else
            {
                __result = IncidentInterceptorUtility.tempRandomDropCell;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_MeteoriteImpact), "TryExecuteWorker")]
    class Patch_MeteoriteImpact_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool PreFix(ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.IsIntercepting_Meteorite == WorkerPatchType.ExecuteOrigin)
                return true;
            else
            {
                if (!IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_MeteoriteImpact>(DefDatabase<IncidentDef>.GetNamed("MeteoriteImpact"), parms))
                    return true;
                __result = false;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_ShipChunkDrop), "TryExecuteWorker")]
    class Patch_ShipChunkDrop_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool PreFix(ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.IsIntercepting_ShipChunk == WorkerPatchType.ExecuteOrigin)
                return true;
            else
            {
                if (!IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_ShipChunk>(DefDatabase<IncidentDef>.GetNamed("ShipChunkDrop"), parms))
                    return true;
                __result = false;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_TransportPodCrash), "TryExecuteWorker")]
    class Patch_TransportPod_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool PreFix(ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.IsIntercepting_TransportPod == WorkerPatchType.ExecuteOrigin)
                return true;
            else
            {
                if (!IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_TransportPod>(DefDatabase<IncidentDef>.GetNamed("RefugeePodCrash"), parms))
                    return true;
                __result = false;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_ResourcePodCrash), "TryExecuteWorker")]
    class Patch_ResourcePod_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool PreFix(ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.IsIntercepting_ResourcePod == WorkerPatchType.ExecuteOrigin)
                return true;
            else
            {
                if (!IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_ResourcePod>(DefDatabase<IncidentDef>.GetNamed("ResourcePodCrash"), parms))
                    return true;
                __result = false;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(IncidentWorker), "TryExecute")]
    class Patch_IncidentWorker_TryExecute
    {
        //TODO: I have no choice but do the patch like this
        //'cause the incidentworker for shippart is an internal class
        //and manual patching doesn't work
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker __instance, ref bool __result, IncidentParms parms)
        {
            var def = __instance.def;
            if (def != DefDatabase<IncidentDef>.GetNamed("PsychicEmanatorShipPartCrash") && def != DefDatabase<IncidentDef>.GetNamed("PoisonShipPartCrash"))
                return true;
            if (IncidentInterceptorUtility.IsIntercepting_ShipPart == WorkerPatchType.ExecuteOrigin)
                return true;
            else
            {
                if (!IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_ShipPartCrash>(__instance.def, parms))
                    return true;
                __result = false;
                return false;
            }
        }

        static void Postfix(ref bool __result)
        {
            if (IncidentInterceptorUtility.IsHoaxingStoryTeller)
            {
                __result = true;
                IncidentInterceptorUtility.IsHoaxingStoryTeller = false;
            }
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_Infestation), "TryExecuteWorker")]
    class Patch_Infestation_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool PreFix(ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.IsIntercepting_Infestation == WorkerPatchType.ExecuteOrigin)
                return true;
            else
            {
                if (!IncidentInterceptorUtility.Intercept_Infestation(parms))
                    return true;
                __result = false;
                return false;
            }
        }
    }

}
