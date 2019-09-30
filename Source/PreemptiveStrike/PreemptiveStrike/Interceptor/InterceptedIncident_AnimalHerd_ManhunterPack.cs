using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;
using UnityEngine;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_AnimalHerd_ManhunterPack : InterceptedIncident_AnimalHerd
    {
        IntVec3 intVec;
        List<Pawn> pawnList;

        public override bool IsHostileToPlayer => true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref intVec, "intVec");
            Scribe_Collections.Look(ref pawnList, "pawnList", LookMode.Deep);
        }

        protected override void SetInterceptFlag(WorkerPatchType value)
        {
            IncidentInterceptorUtility.isIntercepting_ManhunterPack = value;
        }

        public override bool ManualDeterminParams()
        {
            Map map = (Map)parms.target;
            if (!ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(parms.points, map.Tile, out AnimalType))
            {
                return false;
            }
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, false, null))
            {
                return false;
            }
            pawnList = ManhunterPackIncidentUtility.GenerateAnimals(AnimalType, map.Tile, parms.points * 1f);
            AnimalNum = pawnList.Count;
            lookTargets = new TargetInfo(intVec, map, false);
            return true;
        }

        public override bool SubstituionWorkerExecution()
        {
            Map map = (Map)parms.target;
            Rot4 rot = Rot4.FromAngleFlat((map.Center - intVec).AngleFlat);
            for (int i = 0; i < pawnList.Count; i++)
            {
                Pawn pawn = pawnList[i];
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
                GenSpawn.Spawn(pawn, loc, map, rot, WipeMode.Vanish, false);
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, false, false, null, false);
                pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.Range(60000, 120000);
            }
            Find.LetterStack.ReceiveLetter("LetterLabelManhunterPackArrived".Translate(), "ManhunterPackArrived".Translate(AnimalType.GetLabelPlural(-1)), LetterDefOf.ThreatBig, pawnList[0], null, null);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Important);
            return true;
        }
    }
}
