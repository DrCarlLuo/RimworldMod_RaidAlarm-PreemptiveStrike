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
    enum RaidGoalType
    {
        Rescue,
        Despoil,
        Revenge,
        Enslave,
        Conquer,
        Extortion,
        Smite,
        Extermination
    }

    abstract class RaidingGoal : IExposable
    {
        public abstract RaidGoalType RaidType { get; }
        public InterceptedIncident_HumanCrowd_RaidEnemy incident;
        public TravelingIncidentCaravan caravan => incident.parentCaravan;

        public virtual void Achieve() { caravan.Dismiss(); }
        public abstract bool IsAchievable(out string failReason);

        public abstract IEnumerable<DiaOption> BargainChoices();
        public abstract bool CanBargain(out string failReason);

        public virtual void GoalTick() { }

        public virtual string Lable { get; }
        public abstract string GoalExpStr { get; }
        public virtual string BargainPersuasionStr => "";
        public virtual string BargainIntimidationStr => "";
        public virtual string BargainBeguilementStr => "";
        public virtual string RemedyPersuasionStr => "";
        public virtual string RemedyIntimidationStr => "";
        public virtual string RemedyBeguilementStr => "";

        //public abstract float ActiveContactOdds { get; }

        public virtual void ApplyToIncident(InterceptedIncident_HumanCrowd_RaidEnemy _incident)
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
            if (e == RaidGoalType.Smite)
                return typeof(RaidingGoal_Smite);
            if (e == RaidGoalType.Extermination)
                return typeof(RaidingGoal_Extermination);
            throw new Exception("Try to get an unimplemented RaidingGoal Class!");
        }

        public virtual void ExposeData() { }
    }

}
