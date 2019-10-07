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
    class RaidingGoal_Extortion : RaidingGoal
    {
        public int needSilver = 0;

        public override string Lable => "PES_RaidGoalName_Extortion".Translate();

        public override RaidGoalType RaidType => RaidGoalType.Extortion;

        public override string GoalExpStr => "PES_RaidNeg_GoalExp_Extortion".Translate(needSilver.ToString());

        private int TotalSilverAndGoldValueInMap
        {
            get
            {
                Map map = caravan.incident.parms.target as Map;
                return RaidingGoalUtility.SilverInMap(map) + RaidingGoalUtility.GoldInMap(map) * 10;
            }
        }

        public override void Achieve()
        {
            if (TotalSilverAndGoldValueInMap < needSilver)
                throw new Exception("Not Possible to Achieve!");
            Map map = caravan.incident.parms.target as Map;
            var silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);
            if (silverStacks != null && silverStacks.Count > 0)
            {
                foreach (Thing stack in silverStacks.InRandomOrder())
                {
                    if (needSilver <= 0) break;
                    int num = Math.Min(needSilver, stack.stackCount);
                    stack.SplitOff(num).Destroy(DestroyMode.Vanish);
                    needSilver -= num;
                }
            }
            if (needSilver > 0)
            {
                int needGold = needSilver / 10;
                if (needSilver % 10 != 0) ++needGold;
                var goldStacks = map.listerThings.ThingsOfDef(ThingDefOf.Gold);
                if (goldStacks != null && goldStacks.Count > 0)
                {
                    foreach (Thing stack in goldStacks.InRandomOrder())
                    {
                        if (needGold <= 0) break;
                        int num = Math.Min(needGold, stack.stackCount);
                        stack.SplitOff(num).Destroy(DestroyMode.Vanish);
                        needGold -= num;
                    }
                }
            }
            base.Achieve();
        }

        public override IEnumerable<DiaOption> BargainChoices()
        {
            List<int> nums = new List<int> { Mathf.CeilToInt(needSilver * 0.75f), Mathf.CeilToInt(needSilver * 0.5f), Mathf.CeilToInt(needSilver * 0.25f) };
            for (int i = 0; i < 3; ++i)
            {
                if (i > 0 && nums[i] == nums[i - 1])
                    continue;
                DiaOption option = new DiaOption(String.Format("{0}x {1}", nums[i].ToString(), ThingDefOf.Silver.label));
                int cur = nums[i];//Fix: Lambda expression works wrong in for-loop before C# 5.0!!!
                option.link = DialogMaker_RaidNegotiation.BargainDetailNode(0.75f - i * 0.25f, () => { needSilver = cur; });
                yield return option;
            }
            yield break;
        }

        public override bool CanBargain(out string failReason)
        {
            failReason = "";
            return true;
        }

        public override bool IsAchievable(out string failReason)
        {
            failReason = "PES_RaidNeg_NegDeeper_Surrender_Fail_NoMoney".Translate();
            return TotalSilverAndGoldValueInMap >= needSilver;
        }

        public override void ApplyToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
        {
            base.ApplyToIncident(_incident);
            needSilver = Mathf.RoundToInt(incident.parms.points);
        }

        public static bool IsAvailableToIncident(InterceptedIncident_HumanCrowd_RaidEnemy testIncident, out string failReason)
        {
            failReason = "This should always be available";
            return true;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref needSilver, "needSilver", 0, false);
        }
    }
}
