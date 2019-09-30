using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;
using UnityEngine;
using Harmony;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_AnimalHerd_Alphabeavers : InterceptedIncident_AnimalHerd
    {
        IntVec3 intVec;

        public override bool IsHostileToPlayer => true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref intVec, "intVec");
        }

        protected override void SetInterceptFlag(WorkerPatchType value)
        {
            IncidentInterceptorUtility.isIntercepting_Alphabeavers = value;
        }

        public override bool ManualDeterminParams()
        {
            Map map = (Map)parms.target;
            AnimalType = PawnKindDefOf.Alphabeaver;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, false, null))
            {
                return false;
            }
            int freeColonistsCount = map.mapPawns.FreeColonistsCount;

            FloatRange countPerColonistRange = getCountPerColonistRange();

            float randomInRange = countPerColonistRange.RandomInRange;

            float f = (float)freeColonistsCount * randomInRange;
            AnimalNum = Mathf.Clamp(GenMath.RoundRandom(f), 1, 10);

            lookTargets = new TargetInfo(intVec, map, false);
            return true;
        }

        public override bool SubstituionWorkerExecution()
        {
            Map map = (Map)parms.target;
            for (int i = 0; i < AnimalNum; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
                Pawn newThing = PawnGenerator.GeneratePawn(AnimalType, null);
                Pawn pawn = (Pawn)GenSpawn.Spawn(newThing, loc, map, WipeMode.Vanish);
                pawn.needs.food.CurLevelPercentage = 1f;
            }
            Find.LetterStack.ReceiveLetter("LetterLabelBeaversArrived".Translate(), "BeaversArrived".Translate(), LetterDefOf.ThreatSmall, lookTargets, null, null);
            return true;
        }

        private FloatRange getCountPerColonistRange()
        {
            Type type = AccessTools.TypeByName("RimWorld.IncidentWorker_Alphabeavers");
            var field = type.GetField("CountPerColonistRange", BindingFlags.Static | BindingFlags.NonPublic);
            return (FloatRange)field.GetValue(null);
        }
    }
}
