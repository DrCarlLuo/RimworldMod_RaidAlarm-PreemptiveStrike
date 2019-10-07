using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Interceptor;

namespace PreemptiveStrike.RaidGoal
{
    class RaidingGoal_Smite : RaidingGoal
    {
        public override string Lable => "PES_RaidGoalName_Smite".Translate();

        public override RaidGoalType RaidType => RaidGoalType.Smite;

        public override string GoalExpStr => "";

        public override void Achieve()
        {
            throw new Exception("Try to Achieve Smite war goal!");
        }

        public override IEnumerable<DiaOption> BargainChoices()
        {
            throw new Exception("Try to get Smite Bargain Choices!");
        }

        public override bool CanBargain(out string failReason)
        {
            failReason = "";
            return false;
        }

        public override bool IsAchievable(out string failReason)
        {
            failReason = "";
            return true;
        }
    }
}
