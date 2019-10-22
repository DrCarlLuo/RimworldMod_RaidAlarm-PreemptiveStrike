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
    [HarmonyPatch(typeof(IncidentWorker), "TryExecute")]
    class Patch_IncidentWorker_TryExecute
    {

        [HarmonyPrefix]
        static bool Prefix(IncidentWorker __instance, ref bool __result, IncidentParms parms)
        {
            //TODO: This is for the ship part incident
            //I have no choice but do the patch like this
            //'cause the incidentworker for shippart is an internal class
            //and manual patching doesn't work
            var def = __instance.def;
            if (def != DefDatabase<IncidentDef>.GetNamed("PsychicEmanatorShipPartCrash") && def != DefDatabase<IncidentDef>.GetNamed("PoisonShipPartCrash"))
                return true;
            if (IncidentInterceptorUtility.IsIntercepting_ShipPart == WorkerPatchType.ExecuteOrigin)
                return true;
            else
            {
                if (!IncidentInterceptorUtility.Intercept_SkyFaller<InterceptedIncident_SkyFaller_ShipPartCrash>(__instance.def, parms))
                    return true;
                __result = true;
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

    //This patch is made for all the raid incidents to help them get the right incidentDef
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
    class Patch_RaidEnemy_TryExecuteWorker
    {
        [HarmonyPrefix]
        static void Prefix(IncidentWorker_RaidEnemy __instance)
        {
            IncidentInterceptorUtility.CurrentIncidentDef = __instance.def;
        }
    }

    //----------------------------------------------------------

    #region Raid Patches
    [HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeWalkIn), "TryResolveRaidSpawnCenter")]
    static class Patch_EdgeWalkIn_TryResolveRaidSpawnCenter
    {
        [HarmonyPostfix]
        static void Postfix(PawnsArrivalModeWorker_EdgeWalkIn __instance, IncidentParms parms, ref bool __result)
        {
            //This is a temporary fix for refugee chased
            if (IncidentInterceptorUtility.IncidentInQueue(parms, IncidentDefOf.RaidEnemy))
                return;

            if (IncidentInterceptorUtility.IsIntercepting_IncidentExcecution)
            {
                if (IncidentInterceptorUtility.Intercept_Raid(parms))
                    __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeWalkInGroups), "TryResolveRaidSpawnCenter")]
    static class Patch_EdgeWalkInGroups_TryResolveRaidSpawnCenter
    {
        [HarmonyPostfix]
        static void Postfix(PawnsArrivalModeWorker_EdgeWalkIn __instance, IncidentParms parms, ref bool __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_IncidentExcecution)
            {
                if (IncidentInterceptorUtility.Intercept_Raid(parms, true))
                    __result = false;
            }
        }
    }
    #endregion

    #region Pawn Generation Patches
    [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns")]
    static class Patch_PawnGroupMakerUtility_GeneratePawns
    {
        [HarmonyPrefix]
        static bool Prefix(ref IEnumerable<Pawn> __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_PawnGeneration == GeneratorPatchFlag.Generate)
                return true;
            if (IncidentInterceptorUtility.IsIntercepting_PawnGeneration == GeneratorPatchFlag.ReturnTempList)
                __result = IncidentInterceptorUtility.tmpPawnList;
            else
                __result = new List<Pawn>();
            IncidentInterceptorUtility.IsIntercepting_PawnGeneration = GeneratorPatchFlag.Generate;
            return false;
        }
    }

    [HarmonyPatch(typeof(PawnsArrivalModeWorkerUtility), "SplitIntoRandomGroupsNearMapEdge")]
    static class Patch_PawnsArrivalModeWorkerUtility_SplitIntoRandomGroupsNearMapEdge
    {
        [HarmonyPrefix]
        static bool Prefix(ref List<Pair<List<Pawn>, IntVec3>> __result)
        {
            if (IncidentInterceptorUtility.IsIntercepting_GroupSpliter == GeneratorPatchFlag.Generate)
                return true;
            if (IncidentInterceptorUtility.IsIntercepting_GroupSpliter == GeneratorPatchFlag.ReturnTempList)
                __result = IncidentInterceptorUtility.tempGroupList;
            else
                __result = new List<Pair<List<Pawn>, IntVec3>>();
            IncidentInterceptorUtility.IsIntercepting_GroupSpliter = GeneratorPatchFlag.Generate;
            return false;
        }
    }
    #endregion

    #region Human Netral Patch

    [HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival), "TryExecuteWorker")]
    static class Patch_IncidentWorker_TraderCaravanArrival_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_TraderCaravanArrival __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_TraderCaravan_Worker)
                return !IncidentInterceptorUtility.CreateIncidentCaraven_HumanNeutral<InterceptedIncident_HumanCrowd_TraderCaravan>(__instance.def, parms);
            return true;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_TravelerGroup), "TryExecuteWorker")]
    static class Patch_IncidentWorker_TravelerGroup_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_TravelerGroup __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_TravelerGroup)
                return !IncidentInterceptorUtility.CreateIncidentCaraven_HumanNeutral<InterceptedIncident_HumanCrowd_TravelerGroup>(__instance.def, parms);
            return true;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_VisitorGroup), "TryExecuteWorker")]
    static class Patch_IncidentWorker_VisitorGroup_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_VisitorGroup __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_VisitorGroup)
                return !IncidentInterceptorUtility.CreateIncidentCaraven_HumanNeutral<InterceptedIncident_HumanCrowd_VisitorGroup>(__instance.def, parms);
            return true;
        }
    }
    #endregion

    #region  Animal Incident Patch
    [HarmonyPatch(typeof(IncidentWorker_FarmAnimalsWanderIn), "TryExecuteWorker")]
    static class Patch_IncidentWorker_FarmAnimalsWanderIn_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_FarmAnimalsWanderIn __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_FarmAnimalsWanderIn == WorkerPatchType.ExecuteOrigin)
                return true;
            if (IncidentInterceptorUtility.isIntercepting_FarmAnimalsWanderIn == WorkerPatchType.Forestall)
            {
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_FarmAnimalsWanderIn>(__instance.def, parms);
                __result = true;
            }
            else
                __result = IncidentInterceptorUtility.tmpIncident.SubstituionWorkerExecution();
            return false;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_HerdMigration), "TryExecuteWorker")]
    static class Patch_IncidentWorker_HerdMigration_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_HerdMigration __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_HerdMigration == WorkerPatchType.ExecuteOrigin)
                return true;
            if (IncidentInterceptorUtility.isIntercepting_HerdMigration == WorkerPatchType.Forestall)
            {
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_HerdMigration>(__instance.def, parms);
                __result = true;
            }
            else
                __result = IncidentInterceptorUtility.tmpIncident.SubstituionWorkerExecution();
            return false;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_ThrumboPasses), "TryExecuteWorker")]
    static class Patch_IncidentWorker_ThrumboPasses_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_ThrumboPasses __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_ThrumboPasses == WorkerPatchType.ExecuteOrigin)
                return true;
            if (IncidentInterceptorUtility.isIntercepting_ThrumboPasses == WorkerPatchType.Forestall)
            {
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_ThrumboPasses>(__instance.def, parms);
                __result = true;
            }
            else
                __result = IncidentInterceptorUtility.tmpIncident.SubstituionWorkerExecution();
            return false;
        }
    }

    static class Patch_IncidentWorker_Alphabeavers_TryExecuteWorker
    {
        public static bool Prefix(IncidentWorker __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_Alphabeavers == WorkerPatchType.ExecuteOrigin)
                return true;
            if (IncidentInterceptorUtility.isIntercepting_Alphabeavers == WorkerPatchType.Forestall)
            {
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_Alphabeavers>(__instance.def, parms);
                __result = true;
            }
            else
                __result = IncidentInterceptorUtility.tmpIncident.SubstituionWorkerExecution();
            return false;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_ManhunterPack), "TryExecuteWorker")]
    static class Patch_IncidentWorker_ManhunterPack_TryExecuteWorker
    {
        [HarmonyPrefix]
        static bool Prefix(IncidentWorker_ManhunterPack __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_ManhunterPack == WorkerPatchType.ExecuteOrigin)
                return true;
            if (IncidentInterceptorUtility.isIntercepting_ManhunterPack == WorkerPatchType.Forestall)
            {
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_ManhunterPack>(__instance.def, parms);
                __result = true;
            }
            else
                __result = IncidentInterceptorUtility.tmpIncident.SubstituionWorkerExecution();
            return false;
        }
    }
    #endregion
}
