using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_HumanCrowd_RaidEnemy_Groups : InterceptedIncident_HumanCrowd_RaidEnemy
    {
        private List<Pair<List<Pawn>, IntVec3>> GroupList;
        private GroupListStorage storage;

        public override string IntentionStr => "PES_Intention_RaidGroup".Translate();

        protected override void ResolveLookTargets()
        {
            IncidentInterceptorUtility.IsIntercepting_GroupSpliter = GeneratorPatchFlag.Generate;
            GroupList = PawnsArrivalModeWorkerUtility.SplitIntoRandomGroupsNearMapEdge(pawnList, parms.target as Map, false);
            storage = new GroupListStorage(GroupList);
            PawnsArrivalModeWorkerUtility.SetPawnGroupsInfo(parms, GroupList);
            var list1 = new List<TargetInfo>();
            foreach (var pair in GroupList)
            {
                if (pair.First.Count > 0)
                    list1.Add(new TargetInfo(pair.Second, parms.target as Map, false));
            }
            lookTargets = list1;
        }

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.tempGroupList = storage.RebuildList();
            IncidentInterceptorUtility.IsIntercepting_GroupSpliter = GeneratorPatchFlag.ReturnTempList;
            base.ExecuteNow();
            IncidentInterceptorUtility.IsIntercepting_GroupSpliter = GeneratorPatchFlag.Generate;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref storage, "storage");
        }
    }
}
