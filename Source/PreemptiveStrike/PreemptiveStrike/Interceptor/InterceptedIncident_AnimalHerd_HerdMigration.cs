using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using PreemptiveStrike.Mod;
using UnityEngine;
using System.Reflection;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_AnimalHerd_HerdMigration : InterceptedIncident_AnimalHerd
    {
        IntVec3 intVec;
        IntVec3 near;
        List<Pawn> pawnList;

        public override bool IsHostileToPlayer => false;

        public override string IntentionStr => "PES_Intention_HerdMigration".Translate();

        public override string IncidentTitle_Confirmed
        {
            get
            {
                if (incidentTitle_Confirmed == null)
                {
                    incidentTitle_Confirmed = PES_NameGenerator.MigrationAnimalName(AnimalType.label);
                }
                return incidentTitle_Confirmed;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<IntVec3>(ref intVec, "intVec");
            Scribe_Values.Look(ref near, "near");
            Scribe_Collections.Look(ref pawnList, "pawnList", LookMode.Deep);
        }

        protected override void SetInterceptFlag(WorkerPatchType value)
        {
            IncidentInterceptorUtility.isIntercepting_HerdMigration = value;
        }

        public override bool ManualDeterminParams()
        {
            Map map = (Map)parms.target;
            if (!this.TryFindAnimalKind(map.Tile, out AnimalType))
            {
                return false;
            }
            if (!this.TryFindStartAndEndCells(map, out intVec, out near))
            {
                return false;
            }
            pawnList = this.GenerateAnimals(AnimalType, map.Tile);
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
                Pawn newThing = pawnList[i];
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
                GenSpawn.Spawn(newThing, loc, map, rot, WipeMode.Vanish, false);
            }
            LordMaker.MakeNewLord(null, new LordJob_ExitMapNear(near, LocomotionUrgency.Walk, 12f, false, false), map, pawnList);
            IncidentDef def = incidentDef;
            string text = string.Format(def.letterText, AnimalType.GetLabelPlural(-1)).CapitalizeFirst();
            string label = string.Format(def.letterLabel, AnimalType.GetLabelPlural(-1).CapitalizeFirst());
            Find.LetterStack.ReceiveLetter(label, text, def.letterDef, pawnList[0], null, null);
            return true;
        }

        private bool TryFindAnimalKind(int tile, out PawnKindDef animalKind)
        {
            MethodInfo methodInfo = incidentDef.workerClass.GetMethod("TryFindAnimalKind", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invokeParms = new object[2] { tile, null };
            bool res = Convert.ToBoolean(methodInfo.Invoke(incidentDef.Worker, invokeParms));
            animalKind = invokeParms[1] as PawnKindDef;
            return res;
        }

        private bool TryFindStartAndEndCells(Map map, out IntVec3 start, out IntVec3 end)
        {
            MethodInfo methodInfo = incidentDef.workerClass.GetMethod("TryFindStartAndEndCells", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invokeParms = new object[3] { map, null, null };
            bool res = Convert.ToBoolean(methodInfo.Invoke(incidentDef.Worker, invokeParms));
            start = (IntVec3)invokeParms[1];
            end = (IntVec3)invokeParms[2];
            return res;
        }

        private List<Pawn> GenerateAnimals(PawnKindDef animalKind, int tile)
        {
            MethodInfo methodInfo = incidentDef.workerClass.GetMethod("GenerateAnimals", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invokeParms = new object[2] { animalKind, tile };
            return methodInfo.Invoke(incidentDef.Worker, invokeParms) as List<Pawn>;
        }
    }
}
