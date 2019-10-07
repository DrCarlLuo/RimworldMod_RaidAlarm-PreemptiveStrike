using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.RaidGoal;

namespace PreemptiveStrike.Interceptor
{
    enum PawnPatchType
    {
        Generate,
        ReturnZero,
        ReturnTempList
    }

    enum WorkerPatchType
    {
        ExecuteOrigin,
        Forestall,
        Substitution
    }

    [StaticConstructorOnStartup]
    class IncidentInterceptorUtility
    {
        #region Intercepting Switches
        //Used in harmony Patches
        public static bool IsIntercepting_IncidentExcecution;
        public static PawnPatchType IsIntercepting_PawnGeneration;
        public static PawnPatchType IsIntercepting_GroupSpliter;

        public static bool isIntercepting_TraderCaravan_Worker;
        public static bool isIntercepting_TravelerGroup;
        public static bool isIntercepting_VisitorGroup;

        public static WorkerPatchType isIntercepting_FarmAnimalsWanderIn;
        public static WorkerPatchType isIntercepting_HerdMigration;
        public static WorkerPatchType isIntercepting_ThrumboPasses;
        public static WorkerPatchType isIntercepting_Alphabeavers;
        public static WorkerPatchType isIntercepting_ManhunterPack;
        #endregion

        public static List<Pawn> tmpPawnList;
        public static InterceptedIncident tmpIncident;
        public static List<Pair<List<Pawn>, IntVec3>> tempGroupList;

        static IncidentInterceptorUtility()
        {
            IsIntercepting_IncidentExcecution = true;
            IsIntercepting_PawnGeneration = PawnPatchType.Generate;

            IsIntercepting_GroupSpliter = PawnPatchType.Generate;

            isIntercepting_TraderCaravan_Worker = true;
            isIntercepting_TravelerGroup = true;
            isIntercepting_VisitorGroup = true;

            isIntercepting_FarmAnimalsWanderIn = WorkerPatchType.Forestall;
            isIntercepting_HerdMigration = WorkerPatchType.Forestall;
            isIntercepting_ThrumboPasses = WorkerPatchType.Forestall;
            isIntercepting_Alphabeavers = WorkerPatchType.Forestall;
            isIntercepting_ManhunterPack = WorkerPatchType.Forestall;
        }

        public static bool Intercept_Raid(IncidentParms parms, bool splitInGroups = false)
        {
            if (parms.faction.PlayerRelationKind != FactionRelationKind.Hostile)
                return false;
            InterceptedIncident incident;
            if (splitInGroups)
                incident = new InterceptedIncident_HumanCrowd_RaidEnemy_Groups();
            else
                incident = new InterceptedIncident_HumanCrowd_RaidEnemy();
            incident.incidentDef = IncidentDefOf.RaidEnemy;
            incident.parms = parms;
            if (!incident.ManualDeterminParams())
                return false;
            RaidingGoalUtility.ResolveRaidGoal(incident as InterceptedIncident_HumanCrowd_RaidEnemy);
            if (!IncidentCaravanUtility.AddNewIncidentCaravan(incident))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            return true;
        }

        public static bool CreateIncidentCaraven_HumanNeutral<T>(IncidentDef incidentDef, IncidentParms parms) where T : InterceptedIncident, new()
        {
            InterceptedIncident incident = new T();
            incident.incidentDef = incidentDef;
            incident.parms = parms;
            IsIntercepting_PawnGeneration = PawnPatchType.ReturnZero;
            if (!IncidentCaravanUtility.AddNewIncidentCaravan(incident))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            return true;
        }

        public static bool CreateIncidentCaravan_Animal<T>(IncidentDef incidentDef, IncidentParms parms) where T : InterceptedIncident, new()
        {
            InterceptedIncident incident = new T();
            incident.incidentDef = incidentDef;
            incident.parms = parms;
            if (!incident.ManualDeterminParams())
                return false;
            if (!IncidentCaravanUtility.AddNewIncidentCaravan(incident))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            return true;
        }

        public static List<Pawn> GenerateRaidPawns(IncidentParms parms)
        {
            IsIntercepting_PawnGeneration = PawnPatchType.Generate;

            PawnGroupKindDef combat = PawnGroupKindDefOf.Combat;
            parms.points = IncidentWorker_Raid.AdjustedRaidPoints(parms.points, parms.raidArrivalMode, parms.raidStrategy, parms.faction, combat);
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms, false);
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms, true).ToList<Pawn>();
            if (list.Count == 0)
                Log.Error("Got no pawns spawning raid from parms " + parms, false);
            return list;
        }

        public static List<Pawn> GenerateNeutralPawns(PawnGroupKindDef pawnGroupKind, IncidentParms parms)
        {
            IsIntercepting_PawnGeneration = PawnPatchType.Generate;

            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(pawnGroupKind, parms, true);
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms, false).ToList<Pawn>();
            return list;
        }
    }
}
