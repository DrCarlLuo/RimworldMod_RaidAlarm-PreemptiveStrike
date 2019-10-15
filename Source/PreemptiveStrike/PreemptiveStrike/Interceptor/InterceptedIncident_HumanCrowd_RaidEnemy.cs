using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.RaidGoal;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_HumanCrowd_RaidEnemy : InterceptedIncident_HumanCrowd
    {
        public RaidGoalType raidGoalType => goal.RaidType;
        public RaidingGoal goal;

        public override bool IsHostileToPlayer => true;

        public bool raidStrategy_revealed = false;

        public override string IntentionStr => "PES_Intention_Raid".Translate();

        private string strategyString;

        public int CombatMoral = 0;

        public override string IncidentTitle_Confirmed
        {
            get
            {
                if(incidentTitle_Confirmed == null)
                {
                    if (SourceFaction == Faction.OfMechanoids)
                        incidentTitle_Confirmed = PES_NameGenerator.MechArmyName();
                    else
                        incidentTitle_Confirmed = PES_NameGenerator.ArmyName();
                }
                return incidentTitle_Confirmed;
            }
        }

        public virtual string StrategyString
        {
            get
            {
                if(strategyString == null)
                {
                    Type workerClass = parms.raidStrategy.workerClass;
                    if (workerClass == typeof(RaidStrategyWorker_ImmediateAttack))
                        strategyString = "PES_Strategy_Normal".Translate();
                    if (workerClass == typeof(RaidStrategyWorker_ImmediateAttackSappers))
                        strategyString = "PES_Strategy_Sapper".Translate();
                    if (workerClass == typeof(RaidStrategyWorker_ImmediateAttackSmart))
                        strategyString = "PES_Strategy_Smart".Translate();
                    if (workerClass == typeof(RaidStrategyWorker_Siege))
                        strategyString = "PES_Strategy_Siege".Translate();
                    if (workerClass == typeof(RaidStrategyWorker_StageThenAttack))
                        strategyString = "PES_Strategy_Stage".Translate();
                }
                return strategyString;
            }
        }

        protected virtual void RevealStrategy()
        {
            raidStrategy_revealed = true;

            if (PES_Settings.DebugModeOn)
            {

            }
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

        public override bool ManualDeterminParams()
        {
            pawnList = IncidentInterceptorUtility.GenerateRaidPawns(parms);
            ResolveLookTargets();
            return true;
        }

        protected virtual void ResolveLookTargets()
        {
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, parms.target as Map, 8, null);
            lookTargets = new TargetInfo(loc, parms.target as Map, false);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref raidStrategy_revealed, "raidStrategy_revealed", false, false);
            Scribe_Values.Look(ref CombatMoral, "CombatMoral", 0, false);
            Scribe_Deep.Look(ref goal, "goal");
            if(Scribe.mode != LoadSaveMode.Saving)
            {
                goal.incident = this;
            }
        }

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_IncidentExcecution = false;
            IncidentInterceptorUtility.IsIntercepting_PawnGeneration = GeneratorPatchFlag.ReturnTempList;
            IncidentInterceptorUtility.tmpPawnList = this.pawnList;

            if (incidentDef != null && this.parms != null)
            {
                if(incidentDef.Worker.TryExecute(this.parms))
                {
                    RaidingGoalUtility.CombatMoralResolver(this);
                }
            }
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");

            IncidentInterceptorUtility.tmpPawnList = null;
            IncidentInterceptorUtility.IsIntercepting_PawnGeneration = GeneratorPatchFlag.Generate;
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
            if (availables.Count != 0)
            {
                Action OneToReveal = availables.RandomElement<Action>();
                OneToReveal();
            }
            parentCaravan.TryNotifyCaravanIntel();
        }

        public override void RevealAllInformation()
        {
            base.RevealAllInformation();
            if (!raidStrategy_revealed)
                RevealStrategy();

        }

        public override void RevealInformationWhenCommunicationEstablished()
        {
            RevealFaction();
            RevealIntention();
        }

        //public void TryActiveMakeContact()
        //{
        //    if((new FloatRange(0f,1f)).RandomInRange < goal.ActiveContactOdds)
        //    {
        //        RevealIntention();
        //        parentCaravan.EstablishCommunication();
        //    }
        //}

    }
}
