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
                IncidentInterceptorUtility.CreateIncidentCaraven_HumanNeutral<InterceptedIncident_HumanCrowd_TraderCaravan>(IncidentDefOf.TraderCaravanArrival, parms);
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
                IncidentInterceptorUtility.CreateIncidentCaraven_HumanNeutral<InterceptedIncident_HumanCrowd_TravelerGroup>(IncidentDefOf.TravelerGroup, parms);
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
                IncidentInterceptorUtility.CreateIncidentCaraven_HumanNeutral<InterceptedIncident_HumanCrowd_VisitorGroup>(IncidentDefOf.VisitorGroup, parms);
            return true;
        }
    }

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
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_FarmAnimalsWanderIn>(DefDatabase<IncidentDef>.GetNamed("FarmAnimalsWanderIn"), parms);
                __result = false;
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
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_HerdMigration>(DefDatabase<IncidentDef>.GetNamed("HerdMigration"), parms);
                __result = false;
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
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_ThrumboPasses>(DefDatabase<IncidentDef>.GetNamed("ThrumboPasses"), parms);
                __result = false;
            }
            else
                __result = IncidentInterceptorUtility.tmpIncident.SubstituionWorkerExecution();
            return false;
        }
    }

    static class Patch_IncidentWorker_Alphabeavers_TryExecuteWorker
    {
        public static bool Prefix(Object __instance, ref bool __result, IncidentParms parms)
        {
            if (IncidentInterceptorUtility.isIntercepting_Alphabeavers == WorkerPatchType.ExecuteOrigin)
                return true;
            if (IncidentInterceptorUtility.isIntercepting_Alphabeavers == WorkerPatchType.Forestall)
            {
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_Alphabeavers>(DefDatabase<IncidentDef>.GetNamed("Alphabeavers"), parms);
                __result = false;
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
                IncidentInterceptorUtility.CreateIncidentCaravan_Animal<InterceptedIncident_AnimalHerd_ManhunterPack>(IncidentDefOf.ManhunterPack, parms);
                __result = false;
            }
            else
                __result = IncidentInterceptorUtility.tmpIncident.SubstituionWorkerExecution();
            return false;
        }
    }
}
