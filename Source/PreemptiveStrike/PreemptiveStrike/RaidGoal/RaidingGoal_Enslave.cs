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
    class RaidingGoal_Enslave : RaidingGoal
    {
        public List<Pawn> Slaves;

        public override string Lable => "PES_RaidGoalName_Enslave".Translate();

        public override string GoalExpStr
        {
            get
            {
                StringBuilder sb = new StringBuilder("");
                for (int i = 0; i < Slaves.Count; ++i)
                {
                    Pawn pawn = Slaves[i];
                    if (i == Slaves.Count - 1)
                        sb.Append("    " + pawn.Name.ToStringShort);
                    else
                        sb.AppendLine("    " + pawn.Name.ToStringShort);
                }
                return "PES_RaidNeg_GoalExp_Enslave".Translate(sb.ToString());
            }
        }

        public override RaidGoalType RaidType => RaidGoalType.Enslave;

        public override void Achieve()
        {
            foreach (var pawn in Slaves)
                pawn.DeSpawn(DestroyMode.Vanish);
            base.Achieve();
        }

        public override void GoalTick()
        {
            List<Pawn> toremove = new List<Pawn>();
            foreach (var pawn in Slaves)
            {
                if (pawn.Dead)
                    toremove.Add(pawn);
            }
            foreach (var pawn in toremove)
                Slaves.Remove(pawn);
            if (Slaves.Count <= 0)
            {
                RaidingGoal_Extortion newGoal = new RaidingGoal_Extortion();
                newGoal.ApplyToIncident(incident);
                Messages.Message("PES_AllSlaveDead".Translate(), MessageTypeDefOf.NeutralEvent);
            }
        }

        public override IEnumerable<DiaOption> BargainChoices()
        {
            for (int i = 1; i <= 3; ++i)
            {
                if (Slaves.Count < i) break;
                DiaOption option = new DiaOption("PES_RaidNeg_NegDeeper_Bargain_Option_Enslave".Translate(i.ToString()));
                int cur = i;
                Action action = delegate ()
                {
                    for (int j = 0; j < cur; ++j)
                        Slaves.Remove(Slaves.RandomElement());
                };
                option.link = DialogMaker_RaidNegotiation.BargainDetailNode(0.75f - i * 0.25f, action);
                yield return option;
            }
        }

        public override bool CanBargain(out string failReason)
        {
            failReason = "PES_RaidNeg_NegDeeper_Bargain_Invalid_OneSlave".Translate();
            if (Slaves.Count <= 1)
                return false;
            return true;
        }

        public override bool IsAchievable(out string failReason)
        {
            failReason = "PES_RaidNeg_NegDeeper_Surrender_Fail_Enslave".Translate();
            foreach (var pawn in Slaves)
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
            List<Pawn> validColonists = new List<Pawn>();
            foreach (var pawn in map.mapPawns.FreeColonistsSpawned)
            {
                if (!pawn.Dead)
                    validColonists.Add(pawn);
            }
            if (validColonists.Count <= 0)
            {
                throw new Exception("Try to create enslave goal but there is no colonist!");
            }
            this.Slaves = new List<Pawn>();
            int maxNum = MaxColonistsByRaidPoints(incident.parms.points);
            maxNum = Math.Min(maxNum, Mathf.FloorToInt(map.mapPawns.FreeColonistsSpawnedCount * 0.5f));
            if (maxNum < 1)
                throw new Exception("Should not resolve enslave goal when there are only 1 colonist!");
            foreach (var pawn in validColonists.InRandomOrder())
            {
                if (Slaves.Count >= maxNum) break;
                Slaves.Add(pawn);
            }

            if (PES_Settings.DebugModeOn)
            {
                string ss = "Want Colonists:";
                foreach (var x in Slaves) ss += "\n" + x.Name.ToStringShort;
                Log.Message(ss);
            }
        }

        public static bool IsAvailableToIncident(InterceptedIncident_HumanCrowd_RaidEnemy testIncident, out string failReason)
        {
            Map map = testIncident.parms.target as Map;
            failReason = "Not enought colonists";
            return map != null && map.mapPawns.FreeColonistsSpawnedCount > 0;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref Slaves, "Slaves", LookMode.Reference);
        }

        public static int MaxColonistsByRaidPoints(float points) => Mathf.CeilToInt(points / 1000f);
    }
}
