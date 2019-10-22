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
    class InterceptedIncident_HumanCrowd_TraderCaravan : InterceptedIncident_HumanCrowd_Neutral
    {
        public bool caravanType_revealed = false;

        public TraderKindDef traderKind = null;

        public override string IntentionStr => "PES_Intention_TradeCaravan".Translate();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref caravanType_revealed, "caravanType_revealed", false, false);
        }

        public override string IncidentTitle_Confirmed
        {
            get
            {
                if (incidentTitle_Confirmed == null)
                {
                    incidentTitle_Confirmed = PES_NameGenerator.TraderName(SourceFaction.Name);
                }
                return incidentTitle_Confirmed;
            }
        }

        public override bool ManualDeterminParams()
        {
            MethodInfo vanillaParmsResolver = typeof(IncidentWorker_NeutralGroup).GetMethod("TryResolveParms", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)vanillaParmsResolver.Invoke(new IncidentWorker_TraderCaravanArrival(), new object[] { parms });
            if (!result)
            {
                return false;
            }
            if (parms.faction.HostileTo(Faction.OfPlayer))
            {
                return false;
            }
            return base.ManualDeterminParams();
        }

        protected override void SetInterceptFlag(bool value)
        {
            IncidentInterceptorUtility.isIntercepting_TraderCaravan_Worker = value;
        }

        protected override PawnGroupKindDef GetPawnGroupKind()
        {
            return PawnGroupKindDefOf.Trader;
        }

        protected virtual void RevealTraderKind()
        {
            if (!crowdSize_revealed)
                RevealCrowdSize();

            var list = pawnList;
            for (int j = 0; j < list.Count; j++)
            {
                Pawn pawn = list[j];
                if (pawn.TraderKind != null)
                {
                    traderKind = pawn.TraderKind;
                    break;
                }
            }

            caravanType_revealed = true;

            if (PES_Settings.DebugModeOn)
            {
                Log.Message("Trader Kind Revealed: " + traderKind.label);
            }
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
            if (!caravanType_revealed && intention_revealed && crowdSize_revealed)
                availables.Add(RevealTraderKind);
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
            if (!caravanType_revealed)
                RevealTraderKind();
        }

    }
}
