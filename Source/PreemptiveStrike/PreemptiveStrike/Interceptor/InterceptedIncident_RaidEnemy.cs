using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_HumanCrowd_RaidEnemy : InterceptedIncident_HumanCrowd
    {
        public bool raidStrategy_revealed = false;
        public List<Pawn> pawnList;

        public virtual RaidStrategyDef RaidStrategy => parms.raidStrategy;

        protected virtual void RevealStrategy()
        {
            raidStrategy_revealed = true;
        }

        public override int CrowdSize => pawnList != null ? pawnList.Count : 0;

        protected override void RevealCrowdSize()
        {
            crowdSize_revealed = true;
            PawnGroupKindDef combat = PawnGroupKindDefOf.Combat;
            parms.points = IncidentWorker_Raid.AdjustedRaidPoints(parms.points, parms.raidArrivalMode, parms.raidStrategy, parms.faction, combat);
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms, false);
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms, true).ToList<Pawn>();
            if (list.Count == 0)
                Log.Error("Got no pawns spawning raid from parms " + parms, false);

            pawnList = list;

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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref raidStrategy_revealed, "raidStrategy_revealed", false, false);
        }

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_IncidentExcecution = false;
            IncidentInterceptorUtility.IsIntercepting_PawnGeneration = true;
            IncidentInterceptorUtility.tmpPawnList = this.pawnList;

            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");

            IncidentInterceptorUtility.tmpPawnList = null;
            IncidentInterceptorUtility.IsIntercepting_PawnGeneration = false;
            IncidentInterceptorUtility.IsIntercepting_IncidentExcecution = true;
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
            if (!raidStrategy_revealed && intention_revealed)
                availables.Add(RevealStrategy);
            Action OneToReveal = availables.RandomElement<Action>();
            OneToReveal();
        }

        public override void RevealAllInformation()
        {
            base.RevealAllInformation();
            if (!raidStrategy_revealed)
                RevealStrategy();
        }

        public InterceptedIncident_HumanCrowd_RaidEnemy() { }

        public InterceptedIncident_HumanCrowd_RaidEnemy(IncidentParms parms) : base(IncidentDefOf.RaidEnemy, parms) { }
    }
}
