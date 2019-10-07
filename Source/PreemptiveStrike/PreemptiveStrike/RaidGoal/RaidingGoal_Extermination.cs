using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Interceptor;

namespace PreemptiveStrike.RaidGoal
{
    class RaidingGoal_Extermination : RaidingGoal
    {
        public override string Lable => "PES_RaidGoalName_Extermination".Translate();

        public override RaidGoalType RaidType => RaidGoalType.Extermination;

        public override string GoalExpStr => "";

        public override void Achieve()
        {
            throw new Exception("Try to Achieve Extermination war goal!");
        }

        public override IEnumerable<DiaOption> BargainChoices()
        {
            throw new Exception("Try to get Extermiantion Bargain Choices!");
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
