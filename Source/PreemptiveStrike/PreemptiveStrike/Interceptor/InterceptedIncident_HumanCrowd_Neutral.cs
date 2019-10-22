using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;
using System.Reflection;

namespace PreemptiveStrike.Interceptor
{
    abstract class InterceptedIncident_HumanCrowd_Neutral : InterceptedIncident_HumanCrowd
    {
        protected abstract void SetInterceptFlag(bool value);

        public override bool IsHostileToPlayer => false;

        protected virtual PawnGroupKindDef GetPawnGroupKind()
        {
            return PawnGroupKindDefOf.Peaceful;
        }

        public override void ExecuteNow()
        {
            SetInterceptFlag(false);
            IncidentInterceptorUtility.IsIntercepting_PawnGeneration = GeneratorPatchFlag.ReturnTempList;
            IncidentInterceptorUtility.tmpPawnList = this.pawnList;

            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");

            IncidentInterceptorUtility.tmpPawnList = null;
            SetInterceptFlag(true);
        }

        public override bool ManualDeterminParams()
        {
            //Need to first determine factions then do this!
            pawnList = IncidentInterceptorUtility.GenerateNeutralPawns(GetPawnGroupKind(), parms);
            if (pawnList == null || pawnList.Count <= 0)
            {
                Log.Error("Fail to generate pawns in neutral human crowd");
                return false;
            }
            return true;
        }

        public override void RevealRandomInformation()
        {
            List<Action> availables = new List<Action>();
            if (!intention_revealed)
                availables.Add(RevealIntention);
            if (!faction_revealed)
                availables.Add(RevealFaction);
            if (!crowdSize_revealed)
                availables.Add(RevealCrowdSize);
            if (!spawnPosition_revealed)
                availables.Add(RevealSpawnPosition);
            if (availables.Count != 0)
            {
                Action OneToReveal = availables.RandomElement<Action>();
                OneToReveal();
            }
            parentCaravan.TryNotifyCaravanIntel();
        }

        public override void RevealInformationWhenCommunicationEstablished()
        {
            RevealAllInformation();
        }

        protected override void RevealSpawnPosition()
        {
            lookTargets = new TargetInfo(parms.spawnCenter, parms.target as Map, false);
            base.RevealSpawnPosition();
        }

        protected override void RevealCrowdSize()
        {
            crowdSize_revealed = true;

            if (PES_Settings.DebugModeOn)
            {
                Log.Message("CrowedSize revealed!!!");
                StringBuilder sb = new StringBuilder("pawn number:");
                sb.Append(pawnList.Count + " ");
                foreach (var x in pawnList)
                {
                    sb.Append("\n");
                    sb.Append(x.Name);
                }
                Log.Message(sb.ToString());
            }
        }

    }
}
