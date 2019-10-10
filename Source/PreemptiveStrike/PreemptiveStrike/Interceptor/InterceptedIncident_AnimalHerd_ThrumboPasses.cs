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
    class InterceptedIncident_AnimalHerd_ThrumboPasses : InterceptedIncident_AnimalHerd
    {
        IntVec3 invalid;
        IntVec3 intVec;

        public override bool IsHostileToPlayer => false;

        public override string IntentionStr => "PES_Intention_ThrumboPass".Translate();

        public override string IncidentTitle_Confirmed
        {
            get
            {
                if (incidentTitle_Confirmed == null)
                {
                    incidentTitle_Confirmed = PES_NameGenerator.ThrumboName();
                }
                return incidentTitle_Confirmed;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref invalid, "invalid");
            Scribe_Values.Look(ref intVec, "intVec");
        }

        protected override void SetInterceptFlag(WorkerPatchType value)
        {
            IncidentInterceptorUtility.isIntercepting_ThrumboPasses = value;
        }

        public override bool ManualDeterminParams()
        {
            Map map = (Map)parms.target;
            if (!this.TryFindEntryCell(map, out intVec))
            {
                return false;
            }
            AnimalType = PawnKindDefOf.Thrumbo;
            float num = StorytellerUtility.DefaultThreatPointsNow(map);
            AnimalNum = GenMath.RoundRandom(num / AnimalType.combatPower);
            int max = Rand.RangeInclusive(2, 4);
            AnimalNum = Mathf.Clamp(AnimalNum, 1, max);
            invalid = IntVec3.Invalid;
            if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(intVec, map, 10f, out invalid))
            {
                invalid = IntVec3.Invalid;
            }
            lookTargets = new TargetInfo(intVec, map, false);
            return true;
        }

        public override bool SubstituionWorkerExecution()
        {
            Map map = (Map)parms.target;
            Pawn pawn = null;
            int num3 = Rand.RangeInclusive(90000, 150000);
            for (int i = 0; i < AnimalNum; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
                pawn = PawnGenerator.GeneratePawn(AnimalType, null);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);
                pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num3;
                if (invalid.IsValid)
                {
                    pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, map, 10, null);
                }
            }
            Find.LetterStack.ReceiveLetter("LetterLabelThrumboPasses".Translate(AnimalType.label).CapitalizeFirst(), "LetterThrumboPasses".Translate(AnimalType.label), LetterDefOf.PositiveEvent, pawn, null, null);
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            MethodInfo methodInfo = incidentDef.workerClass.GetMethod("TryFindEntryCell", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invokeParms = new object[2] { map, null };
            bool res = Convert.ToBoolean(methodInfo.Invoke(incidentDef.Worker, invokeParms));
            cell = (IntVec3)invokeParms[1];
            return res;
        }
    }
}
