using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Interceptor;

namespace PreemptiveStrike.IncidentCaravan
{
    enum RaidGoal
    {
        Rescue,
        Despoil,
        Revenge,
        Enslave,
        Conquer,
        Extortion,
        Smite
    }

    static class RaidingGoalUtility
    {
        public static void ResolveRaidGoal(InterceptedIncident_HumanCrowd_RaidEnemy incident)
        {
            incident.raidGoal = RaidGoal.Extortion;
        }

        public static void SmiteThePlayer(TravelingIncidentCaravan caravan, Pawn pawn)
        {
            if (!(caravan.incident is InterceptedIncident_HumanCrowd_RaidEnemy incident))
                return;
            caravan.Communicable = false;
            incident.raidGoal = RaidGoal.Smite;
            if (pawn.skills.GetSkill(SkillDefOf.Social).Level < 5)
                incident.CombatMoral = -1;
            else if (pawn.skills.GetSkill(SkillDefOf.Social).Level < 10)
                incident.CombatMoral = 0;
            else
                incident.CombatMoral = 1;
        }

        public static void CombatMoralResolver(InterceptedIncident_HumanCrowd_RaidEnemy incident)
        {
            Map map = incident.parms.target as Map;
            var colonists = map.mapPawns.FreeColonists;
            HediffDef hediffPos = PESDefOf.PES_CombatFervor;
            HediffDef hediffNeg = PESDefOf.PES_CombatTiredness;
            if (incident.CombatMoral == 0)
                return;
            foreach(var pawn in colonists)
            {
                if (incident.CombatMoral == 1)
                {
                    if (pawn.health.hediffSet.HasHediff(hediffNeg))
                    {
                        foreach(var hediff in pawn.health.hediffSet.hediffs)
                        {
                            if(hediff.def == hediffNeg)
                            {
                                pawn.health.RemoveHediff(hediff);
                                break;
                            }
                        }
                    }
                    pawn.health.AddHediff(hediffPos, null, null, null);
                }
                else
                {
                    if (!pawn.health.hediffSet.HasHediff(hediffPos))
                        pawn.health.AddHediff(hediffNeg, null, null, null);
                }
            }
        }
    }
}
