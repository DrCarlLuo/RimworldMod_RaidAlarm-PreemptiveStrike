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

        protected override void ResolveLookTargets()
        {
            IncidentInterceptorUtility.IsIntercepting_GroupSpliter = PawnPatchType.Generate;
            GroupList = PawnsArrivalModeWorkerUtility.SplitIntoRandomGroupsNearMapEdge(pawnList, parms.target as Map, false);
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
            IncidentInterceptorUtility.tempGroupList = GroupList;
            IncidentInterceptorUtility.IsIntercepting_GroupSpliter = PawnPatchType.ReturnTempList;
            base.ExecuteNow();
            IncidentInterceptorUtility.IsIntercepting_GroupSpliter = PawnPatchType.Generate;
        }
    }
}
