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
    class InterceptedIncident_AnimalHerd_FarmAnimalsWanderIn : InterceptedIncident_AnimalHerd
    {
        public override bool IsHostileToPlayer => false;

        public override string IntentionStr => "PES_Intention_FarmAnimalsWanderIn".Translate();

        public override string IncidentTitle_Confirmed
        {
            get
            {
                if (incidentTitle_Confirmed == null)
                {
                    incidentTitle_Confirmed = PES_NameGenerator.FarmAnimalName(AnimalType.label);
                }
                return incidentTitle_Confirmed;
            }
        }

        protected override void SetInterceptFlag(WorkerPatchType value)
        {
            IncidentInterceptorUtility.isIntercepting_FarmAnimalsWanderIn = value;
        }

        private IntVec3 EntryVec;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<IntVec3>(ref EntryVec, "EntryVec");
        }

        public override bool ManualDeterminParams()
        {
            Map map = (Map)parms.target;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out EntryVec, map, CellFinder.EdgeRoadChance_Animal, false, null))
            {
                return false;
            }
            if (!this.TryFindRandomPawnKind(map, out AnimalType))
            {
                return false;
            }
            AnimalNum = Mathf.Clamp(GenMath.RoundRandom(2.5f / AnimalType.RaceProps.baseBodySize), 2, 10);
            lookTargets = new TargetInfo(EntryVec, map, false);
            return true;
        }

        public override bool SubstituionWorkerExecution()
        {
            Map map = (Map)parms.target;
            for (int i = 0; i < AnimalNum; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(EntryVec, map, 12, null);
                Pawn pawn = PawnGenerator.GeneratePawn(AnimalType, null);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);
                pawn.SetFaction(Faction.OfPlayer, null);
            }
            Find.LetterStack.ReceiveLetter("LetterLabelFarmAnimalsWanderIn".Translate(AnimalType.GetLabelPlural(-1)).CapitalizeFirst(), "LetterFarmAnimalsWanderIn".Translate(AnimalType.GetLabelPlural(-1)), LetterDefOf.PositiveEvent, lookTargets, null, null);
            return true;
        }

        private bool TryFindRandomPawnKind(Map map, out PawnKindDef kind)
        {
            MethodInfo methodInfo = incidentDef.workerClass.GetMethod("TryFindRandomPawnKind",BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invokeParms = new object[2] { map, null };
            bool res = Convert.ToBoolean(methodInfo.Invoke(incidentDef.Worker, invokeParms));
            kind = invokeParms[1] as PawnKindDef;
            return res;
        }
    }
}
