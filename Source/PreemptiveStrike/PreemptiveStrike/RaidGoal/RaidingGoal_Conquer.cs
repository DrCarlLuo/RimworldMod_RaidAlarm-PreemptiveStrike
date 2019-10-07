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
    class RaidingGoal_Conquer : RaidingGoal
    {
        public override string Lable => "PES_RaidGoalName_Conquer".Translate();

        public override string GoalExpStr => "PES_RaidNeg_GoalExp_Conquer".Translate();

        public override RaidGoalType RaidType => RaidGoalType.Conquer;

        public override void Achieve()
        {
            throw new Exception("Try to Acieve Conquere war goal!");
        }

        public override IEnumerable<DiaOption> BargainChoices()
        {
            throw new Exception("Try to get conquere bargain choices!");
        }

        public override bool CanBargain(out string failReason)
        {
            failReason = "PES_RaidNeg_NegDeeper_Bargain_Invalid_Conquer".Translate();
            return false;
        }

        public override bool IsAchievable(out string failReason)
        {
            failReason = "PES_RaidNeg_NegDeeper_Surrender_Fail_Conquer".Translate();
            return false;
        }

        public static bool IsAvailableToIncident(InterceptedIncident_HumanCrowd_RaidEnemy testIncident, out string failReason)
        {
            failReason = "Not enought raid power";
            return testIncident.parms.points >= 1000;
        }
    }
}
