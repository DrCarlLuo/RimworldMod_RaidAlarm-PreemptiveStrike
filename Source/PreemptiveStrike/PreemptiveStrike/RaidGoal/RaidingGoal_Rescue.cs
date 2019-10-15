using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Interceptor;
using PreemptiveStrike.Dialogue;
using PreemptiveStrike.Mod;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;

namespace PreemptiveStrike.RaidGoal
{
    class RaidingGoal_Rescue : RaidingGoal
    {
        public List<Pawn> Prisoners;

        public override string Lable => "PES_RaidGoalName_Rescue".Translate();

        public override string GoalExpStr
        {
            get
            {
                StringBuilder sb = new StringBuilder("");
                for (int i = 0; i < Prisoners.Count; ++i)
                {
                    Pawn pawn = Prisoners[i];
                    if (i == Prisoners.Count - 1)
                        sb.Append("    " + pawn.Name.ToStringShort);
                    else
                        sb.AppendLine("    " + pawn.Name.ToStringShort);
                }
                return "PES_RaidNeg_GoalExp_Rescue".Translate(sb.ToString());
            }
        }

        public override RaidGoalType RaidType => RaidGoalType.Rescue;

        public override void Achieve()
        {
            foreach (var pawn in Prisoners)
            {
                GenGuest.PrisonerRelease(pawn);
                pawn.DeSpawn(DestroyMode.Vanish);
            }
            base.Achieve();
        }

        public override void GoalTick()
        {
            foreach (var pawn in Prisoners)
            {
                if (pawn.Dead)
                {
                    incident.goal = new RaidingGoal_Smite();
                    caravan.Communicable = false;
                    Messages.Message("PES_PrisonerDead".Translate(), MessageTypeDefOf.NegativeEvent);
                    break;
                }
            }
        }

        public override IEnumerable<DiaOption> BargainChoices()
        {
            for (int i = 1; i <= 3; ++i)
            {
                if (Prisoners.Count < i) break;
                DiaOption option = new DiaOption("PES_RaidNeg_NegDeeper_Bargain_Option_Prisoner".Translate(i.ToString()));
                int cur = i;
                Action action = delegate ()
                {
                    for (int j = 0; j < cur; ++j)
                        Prisoners.Remove(Prisoners.RandomElement());
                };
                option.link = DialogMaker_RaidNegotiation.BargainDetailNode(0.75f - i * 0.25f, action);
                yield return option;
            }
        }

        public override bool CanBargain(out string failReason)
        {
            failReason = "PES_RaidNeg_NegDeeper_Bargain_Invalid_OnePrisoner".Translate();
            if (Prisoners.Count <= 1)
                return false;
            return true;
        }

        public override bool IsAchievable(out string failReason)
        {
            failReason = "PES_RaidNeg_NegDeeper_Surrender_Fail_Prisoner".Translate();
            foreach (var pawn in Prisoners)
            {
                if (pawn.Dead || !pawn.Spawned)
                    return false;
            }
            return true;
        }

        public override void ApplyToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
        {
            base.ApplyToIncident(_incident);
            Map map = incident.parms.target as Map;
            List<Pawn> validPrisoners = new List<Pawn>();
            foreach (var pawn in map.mapPawns.PrisonersOfColonySpawned)
            {
                if (!pawn.Dead && pawn.Faction == incident.SourceFaction)
                    validPrisoners.Add(pawn);
            }
            if (validPrisoners.Count <= 0)
            {
                throw new Exception("Try to create prisoner rescue goal but there is no prisoners!");
            }
            this.Prisoners = new List<Pawn>();
            int maxNum = MaxPrisonersByRaidPoints(incident.parms.points);
            foreach (var pawn in validPrisoners.InRandomOrder())
            {
                if (Prisoners.Count >= maxNum) break;
                Prisoners.Add(pawn);
            }

            if (PES_Settings.DebugModeOn)
            {
                string ss = "Rescue Prisoners:";
                foreach (var x in Prisoners) ss += "\n" + x.Name.ToStringShort;
                Log.Message(ss);
            }
        }

        public static bool IsAvailableToIncident(InterceptedIncident_HumanCrowd_RaidEnemy testIncident, out string failReason)
        {
            Map map = testIncident.parms.target as Map;
            failReason = "PES_RaidNeg_Sub_Rescue_NoPrisoner".Translate();
            return map != null && map.mapPawns.PrisonersOfColonySpawnedCount > 0 && map.mapPawns.PrisonersOfColonySpawned.Any((Pawn p) => { return !p.Dead && p.Faction == testIncident.SourceFaction; });
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref Prisoners, "Prisoners", LookMode.Reference);
        }

        public static int MaxPrisonersByRaidPoints(float points) => Math.Min(5, Mathf.CeilToInt(points / 500f));
    }
}
