using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Interceptor;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.Dialogue
{
    static class DialogMaker_RaidNegotiation
    {
        public static DiaNode PrologueNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;

            if (incident == null) return null;
            if (incident.raidGoal == RaidGoal.Smite) return null;

            StringBuilder sb = new StringBuilder("PES_RaidNeg_Proglog".Translate(caravan.CaravanTitle, incident.SourceFaction));
            sb.AppendLine();
            sb.AppendLine();

            bool polite = false;
            if (incident.raidGoal == RaidGoal.Rescue)
            {
                if (new FloatRange(0f, 1f).RandomInRange < 0.5f)
                    polite = true;
            }
            if (incident.raidGoal == RaidGoal.Extortion)
            {
                if (new FloatRange(0f, 1f).RandomInRange < 0.2f)
                    polite = true;
            }

            if (polite)
                sb.Append("PES_RaidNeg_Attitude_Polite".Translate());
            else
                sb.Append("PES_RaidNeg_Attitude_Rude".Translate());
            sb.AppendLine("PES_RaidNeg_GoalExp_Rescue".Translate());
            sb.AppendLine();

            if (polite)
                sb.AppendLine("PES_RaidNeg_DemandSurrender_Polite".Translate());
            else
                sb.AppendLine("PES_RaidNeg_DemandSurrender_rude".Translate());

            DiaNode diaNode = new DiaNode(sb.ToString());

            DiaOption option;

            string rebuffStr = "PES_RaidNeg_Rebuff_Head".Translate() + ("PES_RaidNeg_Rebuff_" + incident.raidGoal.ToString()).Translate() + "\n";
            option = new DiaOption(rebuffStr);
            option.link = RebuffNode();
            diaNode.options.Add(option);

            option = new DiaOption("PES_RaidNeg_NegDeeper".Translate() + "\n");
            option.link = NegotiateDeepNode();
            diaNode.options.Add(option);

            option = new DiaOption("PES_RaidNeg_Delay".Translate() + "\n");
            option.link = DelayNode();
            diaNode.options.Add(option);

            diaNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));
            return diaNode;
        }

        public static DiaNode RebuffNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;

            if (incident == null) return null;
            if (incident.raidGoal == RaidGoal.Smite) return null;

            StringBuilder sb = new StringBuilder("PES_RaidNeg_Rebuff_Confirmation".Translate());
            sb.AppendLine();
            if (incident.raidGoal == RaidGoal.Rescue)
            {
                sb.AppendLine("PES_RaidNeg_Rebuff_Confirmation_Rescue".Translate());
            }
            sb.AppendLine();
            sb.Append(@"<i>");
            sb.Append("PES_RaidNeg_Rebuff_Explanation".Translate(pawn.Name.ToStringShort));
            sb.Append(@"</i>");

            DiaNode diaNode = new DiaNode(sb.ToString());

            void rebuffAction() { RaidingGoalUtility.SmiteThePlayer(caravan, pawn); }
            diaNode.options.Add(DialogUtilities.CurtOption("PES_SimpleConfirm", null, rebuffAction, true));
            diaNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));
            return diaNode;
        }

        public static DiaNode NegotiateDeepNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;

            if (incident == null) return null;
            if (incident.raidGoal == RaidGoal.Smite) return null;

            DiaNode diaNode = new DiaNode("PES_RaidNeg_NegDeeper_Prolog".Translate());
            diaNode.options.Add(DialogUtilities.CurtOption("PES_RaidNeg_NegDeeper_Surrender", null, null, true));
            diaNode.options.Add(DialogUtilities.CurtOption("PES_RaidNeg_NegDeeper_Bargain", null, null, true));
            diaNode.options.Add(DialogUtilities.CurtOption("PES_RaidNeg_NegDeeper_Remedy", null, null, true));
            diaNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));

            return diaNode;
        }

        public static DiaNode DelayNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;

            if (incident == null) return null;
            if (incident.raidGoal == RaidGoal.Smite) return null;

            DiaNode delaySuccessNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Delay_Success".Translate(caravan.CaravanTitle));
                diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, () => { caravan.StageForThreeHours(); }, true));
                return diaNode;
            }
            DiaNode delayFailNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Delay_Fail".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_DAMNIT", null, null, true));
                return diaNode;
            }
            DiaNode delaySmiteNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Delay_Smite".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_DAMNIT", null, () =>
                {
                    caravan.Communicable = false;
                    incident.raidGoal = RaidGoal.Smite;
                }, true));
                return diaNode;
            }

            DiaNode delayNode = new DiaNode("PES_RaidNeg_Delay_Intro".Translate());
            DiaOption option;

            //Persuasion
            float successOdds = Mathf.Clamp01(PES_Settings.BaseDelayPersuasionChance * pawn.NegotiatePowerFactor());
            StringBuilder sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_Persuade_noun".Translate(), "PES_RaidNeg_Delay_Persuasion".Translate()));
            sb.AppendLine(OddsIndicator(successOdds, "PES_RaidNeg_Delay_Success_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, delaySuccessNode(), null, delayFailNode());
            delayNode.options.Add(option);

            //Intimidation
            successOdds = Mathf.Clamp01(PES_Settings.BaseDelayIntimidationSuccessChance * pawn.NegotiatePowerFactor());
            float smiteOdds = Mathf.Clamp01(PES_Settings.BaseDelayIntimidationSmiteChance * pawn.NegotiatePowerFactorNeg());
            string intimidationText = ("PES_RaidNeg_Delay_Intimidation_" + incident.raidGoal.ToString()).Translate();
            sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_intimidate_noun".Translate(), intimidationText));
            sb.AppendLine(OddsIndicator(successOdds, "PES_RaidNeg_Delay_Success_Name", smiteOdds, "PES_RaidNeg_Delay_Smite_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, delaySuccessNode(), smiteOdds, null, delaySmiteNode(), null, delayFailNode());
            delayNode.options.Add(option);

            //Beguilement
            successOdds = Mathf.Clamp01(PES_Settings.BaseDelayBeguilementChance * pawn.NegotiatePowerFactor());
            string beguilementText = ("PES_RaidNeg_Delay_Beguilement_" + incident.raidGoal.ToString()).Translate();
            sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_beguile_noun".Translate(), beguilementText));
            sb.AppendLine(OddsIndicator(successOdds, "PES_RaidNeg_Delay_Success_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, delaySuccessNode(), null, delayFailNode());
            delayNode.options.Add(option);

            return delayNode;
        }

        private static string OddsIndicator(float odds, string verb, float odds2 = 0f, string verb2 = null)
        {
            StringBuilder sb = new StringBuilder("(");
            sb.Append(DialogUtilities.GetOddsString(odds));
            sb.Append(" ");
            sb.Append(verb.Translate());
            if (verb2 != null)
            {
                sb.Append("; ");
                sb.Append(DialogUtilities.GetOddsString(odds2));
                sb.Append(" ");
                sb.Append(verb2.Translate());
            }
            sb.Append(")");
            return sb.ToString();
        }

    }
}
