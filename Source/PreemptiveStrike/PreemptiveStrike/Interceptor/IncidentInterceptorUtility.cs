using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.RaidGoal;
using PreemptiveStrike.Mod;
using PreemptiveStrike.DetectionSystem;

namespace PreemptiveStrike.Interceptor
{
    enum GeneratorPatchFlag
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
        public static IncidentDef CurrentIncidentDef;//This is used for all the raid incidents, because incidentDef can not be obtained from PawnsArrivalModeWorker

        #region Intercepting Switches
        //Used in harmony Patches
        public static bool IsIntercepting_IncidentExcecution;
        public static GeneratorPatchFlag IsIntercepting_PawnGeneration;
        public static GeneratorPatchFlag IsIntercepting_GroupSpliter;

        public static bool isIntercepting_TraderCaravan_Worker;
        public static bool isIntercepting_TravelerGroup;
        public static bool isIntercepting_VisitorGroup;

        public static WorkerPatchType isIntercepting_FarmAnimalsWanderIn;
        public static WorkerPatchType isIntercepting_HerdMigration;
        public static WorkerPatchType isIntercepting_ThrumboPasses;
        public static WorkerPatchType isIntercepting_Alphabeavers;
        public static WorkerPatchType isIntercepting_ManhunterPack;
        #endregion

        #region Simple Interception Switches
        public static bool isIntercepting_EdgeDrop;
        public static bool isIntercepting_CenterDrop;
        public static bool isIntercepting_EdgeDropGroup;
        public static bool isIntercepting_RandomDrop;

        public static GeneratorPatchFlag IsIntercepting_SkyfallerCell_Loose;
        public static GeneratorPatchFlag IsIntercepting_RandomDropSpot;
        public static GeneratorPatchFlag IsIntercepting_InfestationCell;
        public static WorkerPatchType IsIntercepting_Meteorite;
        public static WorkerPatchType IsIntercepting_ShipChunk;
        public static WorkerPatchType IsIntercepting_ShipPart;
        public static WorkerPatchType IsIntercepting_TransportPod;
        public static WorkerPatchType IsIntercepting_ResourcePod;
        public static WorkerPatchType IsIntercepting_Infestation;
        public static WorkerPatchType IsIntercepting_SolarFlare;

        public static bool IsHoaxingStoryTeller;
        #endregion

        public static List<Pawn> tmpPawnList;
        public static InterceptedIncident tmpIncident;
        public static List<Pair<List<Pawn>, IntVec3>> tempGroupList;
        public static IntVec3 tempSkyfallerCellLoose;
        public static IntVec3 tempRandomDropCell;
        public static IntVec3 tempInfestationCell;

        static IncidentInterceptorUtility()
        {
            IsIntercepting_IncidentExcecution = true;
            IsIntercepting_PawnGeneration = GeneratorPatchFlag.Generate;

            IsIntercepting_GroupSpliter = GeneratorPatchFlag.Generate;

            isIntercepting_TraderCaravan_Worker = true;
            isIntercepting_TravelerGroup = true;
            isIntercepting_VisitorGroup = true;

            isIntercepting_FarmAnimalsWanderIn = WorkerPatchType.Forestall;
            isIntercepting_HerdMigration = WorkerPatchType.Forestall;
            isIntercepting_ThrumboPasses = WorkerPatchType.Forestall;
            isIntercepting_Alphabeavers = WorkerPatchType.Forestall;
            isIntercepting_ManhunterPack = WorkerPatchType.Forestall;

            isIntercepting_EdgeDrop = true;
            isIntercepting_CenterDrop = true;
            isIntercepting_EdgeDropGroup = true;
            isIntercepting_RandomDrop = true;

            IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.Generate;
            IsIntercepting_RandomDropSpot = GeneratorPatchFlag.Generate;
            IsIntercepting_InfestationCell = GeneratorPatchFlag.Generate;

            IsIntercepting_Meteorite = WorkerPatchType.Forestall;
            IsIntercepting_ShipChunk = WorkerPatchType.Forestall;
            IsIntercepting_ShipPart = WorkerPatchType.Forestall;
            IsIntercepting_TransportPod = WorkerPatchType.Forestall;
            IsIntercepting_ResourcePod = WorkerPatchType.Forestall;
            IsIntercepting_Infestation = WorkerPatchType.Forestall;
            IsIntercepting_SolarFlare = WorkerPatchType.Forestall;

            IsHoaxingStoryTeller = false;
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

            if (CurrentIncidentDef == null)
            {
                Log.Error("PES: A raid incident that is not compatible with Preemptive Strike is trying to execute. So this incident won't be intercepted by PES and will be executed in it vanilla way");
                return false; //Fix v1.1.4: In some mods, their raids are implemented without a incidentworker
            }

            incident.incidentDef = CurrentIncidentDef;
            incident.parms = parms;
            if (!incident.ManualDeterminParams())
                return false;
            RaidingGoalUtility.ResolveRaidGoal(incident as InterceptedIncident_HumanCrowd_RaidEnemy);
            if (!IncidentCaravanUtility.AddNewIncidentCaravan(incident))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            IsHoaxingStoryTeller = true;
            if (PES_Settings.DebugModeOn)
                Messages.Message("PES_Debug: Successfully intercepted a raid Incident", MessageTypeDefOf.NeutralEvent);
            return true;
        }

        public static bool CreateIncidentCaraven_HumanNeutral<T>(IncidentDef incidentDef,  IncidentParms parms) where T : InterceptedIncident, new()
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
            IsHoaxingStoryTeller = true;
            if (PES_Settings.DebugModeOn)
                Messages.Message("PES_Debug: Successfully intercepted a neutral Incident", MessageTypeDefOf.NeutralEvent);
            //IsIntercepting_PawnGeneration = GeneratorPatchFlag.ReturnZero;
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
            //Hoxing Should be done in the patch
            //IsHoaxingStoryTeller = true;
            if (PES_Settings.DebugModeOn)
                Messages.Message("PES_Debug: Successfully intercepted an animal Incident", MessageTypeDefOf.NeutralEvent);
            return true;
        }

        public static bool Intercept_SkyFaller<T>(IncidentDef incidentDef, IncidentParms parms, bool needHoaxing = false, bool checkHostileFaction = false) where T : InterceptedIncident_SkyFaller, new()
        {
            if (checkHostileFaction && parms.faction.PlayerRelationKind != FactionRelationKind.Hostile)
                return false;

            if (incidentDef == null)
            {
                Log.Error("PES: A raid incident that is not compatible with Preemptive Strike is trying to execute. So this incident won't be intercepted by PES and will be executed in it vanilla way");
                return false;
            }

            InterceptedIncident_SkyFaller incident = new T();
            incident.incidentDef = incidentDef;
            incident.parms = parms;
            if (incident.FallerType == SkyFallerType.Big && !PESDefOf.PES_SkyIDL.IsFinished)
                return false;
            if (incident.FallerType == SkyFallerType.Small && !PESDefOf.PES_SkyIDS.IsFinished)
                return false;
            if (!incident.PreCalculateDroppingSpot())
            {
                return false;
            }
            int totDuration = incident.FallerType == SkyFallerType.Big ? PES_Settings.LargeSkyFallerDuration : PES_Settings.SmallSkyFallerDuration;
            int decTime = incident.FallerType == SkyFallerType.Big ? PES_Settings.LargeSkyFallerIdentificationTime : PES_Settings.SmallSkyFallerIdentificationTime;
            if (!IncidentCaravanUtility.AddSimpleIncidentCaravan(incident, totDuration, decTime))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            if(needHoaxing)
                IsHoaxingStoryTeller = true;
            if (PES_Settings.DebugModeOn)
                Messages.Message("PES_Debug: Successfully intercepted a skyfaller Incident", MessageTypeDefOf.NeutralEvent);
            return true;
        }

        public static bool Intercept_Infestation(IncidentParms parms)
        {
            if (!PESDefOf.PES_InfestationDetection.IsFinished)
                return false;
            Map map = parms.target as Map;
            if (!map.listerBuildings.allBuildingsColonist.Any(b => b.def == DefDatabase<ThingDef>.GetNamed("GroundPenetratingScanner")))
                return false;

            InterceptedIncident_Infestation incident = new InterceptedIncident_Infestation();
            incident.incidentDef = DefDatabase<IncidentDef>.GetNamed("Infestation");
            incident.parms = parms;
            if (!incident.DeterminSpot())
                return false;
            if (!IncidentCaravanUtility.AddSimpleIncidentCaravan(incident, PES_Settings.InfestationDuration, 1250,true))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            Dialogue.SparkUILetter.Make("PES_Infestation_Letter".Translate(), "PES_Infestation_Text".Translate(), LetterDefOf.ThreatBig, incident.parentCaravan);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            IsHoaxingStoryTeller = true;
            if (PES_Settings.DebugModeOn)
                Messages.Message("PES_Debug: Successfully intercepted an infestation Incident", MessageTypeDefOf.NeutralEvent);
            return true;
        }

        public static bool Intercept_SolarFlare(IncidentParms parms)
        {
            if (DetectDangerUtilities.LastSolarFlareDetectorTick != Find.TickManager.TicksGame)
                return false;

            InterceptedIncident_SolarFlare incident = new InterceptedIncident_SolarFlare();
            incident.incidentDef = DefDatabase<IncidentDef>.GetNamed("SolarFlare");
            incident.parms = parms;
            if(!IncidentCaravanUtility.AddSimpleIncidentCaravan(incident, 2500 * 12,0,true))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            if (PES_Settings.DebugModeOn)
                Messages.Message("PES_Debug: Successfully intercepted an solar flare", MessageTypeDefOf.NeutralEvent);
            Dialogue.OpenUILetter.Make("PES_Warning_Flare_Early".Translate(), "PES_Warning_Flare_Early_Text".Translate(), LetterDefOf.NegativeEvent);
            IsHoaxingStoryTeller = true;
            return true;
        }

        public static List<Pawn> GenerateRaidPawns(IncidentParms parms)
        {
            IsIntercepting_PawnGeneration = GeneratorPatchFlag.Generate;

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
            IsIntercepting_PawnGeneration = GeneratorPatchFlag.Generate;

            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(pawnGroupKind, parms, true);
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms, false).ToList<Pawn>();
            return list;
        }

        public static bool IncidentInQueue(IncidentParms parms, IncidentDef incidentDef)
        {
            foreach(QueuedIncident qi in Find.Storyteller.incidentQueue)
            {
                if (qi.FiringIncident.parms == parms && qi.FiringIncident.def == incidentDef)
                    return true;
            }
            return false;
        }
        
    }
}
