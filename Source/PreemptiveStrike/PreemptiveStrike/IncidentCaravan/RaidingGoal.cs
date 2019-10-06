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

namespace PreemptiveStrike.IncidentCaravan
{
    enum RaidGoalType
    {
        Rescue,
        Despoil,
        Revenge,
        Enslave,
        Conquer,
        Extortion,
        Smite
    }

    abstract class RaidingGoal : IExposable
    {
        public RaidGoalType type;
        public InterceptedIncident_HumanCrowd_RaidEnemy incident;
        public TravelingIncidentCaravan caravan => incident.parentCaravan;

        public virtual void Achieve() { caravan.Dismiss(); }
        public abstract bool IsAchievable(out string failReason);

        public abstract IEnumerable<DiaOption> BargainChoices();
        public abstract bool CanBargain(out string failReason);

        public virtual void GoalTick() { }

        public abstract string GoalExpStr { get; }
        public virtual string BargainPersuasionStr => "";
        public virtual string BargainIntimidationStr => "";
        public virtual string BargainBeguilementStr => "";
        public virtual string RemedyPersuasionStr => "";
        public virtual string RemedyIntimidationStr => "";
        public virtual string RemedyBeguilementStr => "";

        public virtual void ResolveToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
        {
            _incident.goal = this;
            this.incident = _incident;
        }

        public static Type GetRaidClassByEnum(RaidGoalType e)
        {
            if (e == RaidGoalType.Conquer)
                return typeof(RaidingGoal_Conquer);
            if (e == RaidGoalType.Enslave)
                return typeof(RaidingGoal_Enslave);
            if (e == RaidGoalType.Extortion)
                return typeof(RaidingGoal_Extortion);
            if (e == RaidGoalType.Rescue)
                return typeof(RaidingGoal_Rescue);
            throw new Exception("Try to get an unimplemented RaidingGoal Class!");
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref type, "raidType");
        }
    }

    class RaidingGoal_Conquer : RaidingGoal
    {
        public override string GoalExpStr => "PES_RaidNeg_GoalExp_Conquer".Translate();

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

        public override void ResolveToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
        {
            base.ResolveToIncident(_incident);
            incident.raidGoalType = RaidGoalType.Conquer;
        }

        public static bool IsAvailableToIncident(InterceptedIncident_HumanCrowd_RaidEnemy testIncident, out string failReason)
        {
            failReason = "Not enought raid power";
            return testIncident.parms.points >= 1000;
        }
    }

    class RaidingGoal_Extortion : RaidingGoal
    {
        public int needSilver = 0;

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

        public override void ResolveToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
        {
            base.ResolveToIncident(_incident);
            incident.raidGoalType = RaidGoalType.Extortion;
            needSilver = Mathf.RoundToInt(incident.parms.points);
        }

        public static bool IsAvailableToIncident(InterceptedIncident_HumanCrowd_RaidEnemy testIncident, out string failReason)
        {
            failReason = "This should always be available";
            return true;
        }
    }

    class RaidingGoal_Rescue : RaidingGoal
    {
        public List<Pawn> Prisoners;

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
            foreach(var pawn in Prisoners)
            {
                if(pawn.Dead)
                {
                    incident.raidGoalType = RaidGoalType.Smite;
                    incident.goal = null;
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

        public override void ResolveToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
        {
            base.ResolveToIncident(_incident);
            incident.raidGoalType = RaidGoalType.Rescue;
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

        public static int MaxPrisonersByRaidPoints(float points) => Math.Min(5,Mathf.CeilToInt(points / 500f));
    }

    class RaidingGoal_Enslave : RaidingGoal
    {
        public List<Pawn> Slaves;

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

        public override void Achieve()
        {
            foreach (var pawn in Slaves)
                pawn.DeSpawn(DestroyMode.Vanish);
            base.Achieve();
        }

        public override void GoalTick()
        {
            List<Pawn> toremove = new List<Pawn>();
            foreach(var pawn in Slaves)
            {
                if (pawn.Dead)
                    toremove.Add(pawn);
            }
            foreach (var pawn in toremove)
                Slaves.Remove(pawn);
            if(Slaves.Count <=0)
            {
                incident.raidGoalType = RaidGoalType.Extortion;
                RaidingGoal_Extortion newGoal = new RaidingGoal_Extortion();
                newGoal.ResolveToIncident(incident);
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

        public override void ResolveToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
        {
            base.ResolveToIncident(_incident);
            incident.raidGoalType = RaidGoalType.Enslave;
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

        public static bool IsAvailableToIncident(InterceptedIncident_HumanCrowd_RaidEnemy testIncident,out string failReason)
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
