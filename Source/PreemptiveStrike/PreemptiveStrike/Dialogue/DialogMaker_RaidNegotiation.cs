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
using PreemptiveStrike.RaidGoal;

namespace PreemptiveStrike.Dialogue
{
    static class DialogMaker_RaidNegotiation
    {
        public static DiaNode PrologueNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;
            RaidingGoal goal = incident.goal;

            if (incident == null) return null;
            if (incident.raidGoalType == RaidGoalType.Smite) return null;

            if (incident.SourceFaction == Faction.OfMechanoids)
                return DialogMaker_TryToContact.MechanoidAnswers();

            StringBuilder sb = new StringBuilder("PES_RaidNeg_Proglog".Translate(caravan.CaravanTitle, incident.SourceFaction));
            sb.AppendLine();
            sb.AppendLine();

            bool polite = false;
            if (incident.raidGoalType == RaidGoalType.Rescue)
            {
                if (new FloatRange(0f, 1f).RandomInRange < 0.5f)
                    polite = true;
            }
            if (incident.raidGoalType == RaidGoalType.Extortion)
            {
                if (new FloatRange(0f, 1f).RandomInRange < 0.2f)
                    polite = true;
            }

            if (polite)
                sb.Append("PES_RaidNeg_Attitude_Polite".Translate());
            else
                sb.Append("PES_RaidNeg_Attitude_Rude".Translate());
            sb.AppendLine(goal.GoalExpStr);
            sb.AppendLine();

            if (polite)
                sb.AppendLine("PES_RaidNeg_DemandSurrender_Polite".Translate());
            else
                sb.AppendLine("PES_RaidNeg_DemandSurrender_rude".Translate());

            DiaNode diaNode = new DiaNode(sb.ToString());

            DiaOption option;

            string rebuffStr = "PES_RaidNeg_Rebuff_Head".Translate() + ("PES_RaidNeg_Rebuff_" + incident.raidGoalType.ToString()).Translate() + "\n";
            option = new DiaOption(rebuffStr);
            option.link = RebuffNode();
            diaNode.options.Add(option);

            option = new DiaOption("PES_RaidNeg_NegDeeper".Translate() + "\n");
            option.link = NegotiateDeepNode();

            diaNode.options.Add(option);

            option = new DiaOption("PES_RaidNeg_Delay".Translate() + "\n");
            option.link = DelayNode();
            if (caravan.stageRemainingTick > 0)
                option.Disable("PES_RaidNeg_Delay_Staging".Translate(GenDate.ToStringTicksToPeriod(caravan.stageRemainingTick)));
            else if (caravan.StagedBefore)
                option.Disable("PES_RaidNeg_Delay_DoneBefore".Translate());
            else if (caravan.delayCoolDownTick > 0)
                option.Disable("PES_RaidNeg_Delay_CoolDown".Translate(GenDate.ToStringTicksToPeriod(caravan.delayCoolDownTick)));
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
            if (incident.raidGoalType == RaidGoalType.Smite) return null;

            StringBuilder sb = new StringBuilder("PES_RaidNeg_Rebuff_Confirmation".Translate());
            sb.AppendLine();
            if (incident.raidGoalType == RaidGoalType.Rescue)
            {
                sb.AppendLine("PES_RaidNeg_Rebuff_Confirmation_Rescue".Translate());
                RaidingGoal_Rescue goal = incident.goal as RaidingGoal_Rescue;
                foreach (var p in goal.Prisoners)
                {
                    sb.AppendLine("    " + p.Name.ToStringShort);
                }
            }
            sb.AppendLine();
            sb.Append(@"<i>");
            sb.Append("PES_RaidNeg_Rebuff_Explanation".Translate(pawn.Name.ToStringShort));
            sb.Append(@"</i>");

            DiaNode diaNode = new DiaNode(sb.ToString());

            void rebuffAction() { RaidingGoalUtility.RebuffDemandAndSmiteThePlayer(caravan, pawn); }
            DiaOption option = new DiaOption("PES_SimpleConfirm".Translate());
            option.action = rebuffAction;
            option.resolveTree = true;
            if (pawn.story.WorkTagIsDisabled(WorkTags.Violent))
                option.Disable("PES_RaidNeg_Rebuff_Rescue_Fail".Translate());
            diaNode.options.Add(option);
            diaNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));
            return diaNode;
        }

        public static DiaNode NegotiateDeepNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;
            RaidingGoal goal = incident.goal;

            if (incident == null) return null;
            if (incident.raidGoalType == RaidGoalType.Smite) return null;

            DiaNode diaNode = new DiaNode("PES_RaidNeg_NegDeeper_Prolog".Translate());
            DiaOption option;
            StringBuilder sb;
            string failReason;

            //surrender
            option = new DiaOption("PES_RaidNeg_NegDeeper_Surrender".Translate() + "\n");
            if (!goal.IsAchievable(out failReason))
            {
                option.disabled = true;
                option.disabledReason = failReason;
            }
            else
            {
                DiaNode surrenderConfirmation()
                {
                    DiaNode cnode = new DiaNode("PES_RaidNeg_NegDeeper_Surrender_Comfirmation".Translate());
                    cnode.options.Add(DialogUtilities.CurtOption("PES_SimpleConfirm", null, () => { goal.Achieve(); }, true));
                    cnode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));
                    return cnode;
                }
                option.link = surrenderConfirmation();
            }
            diaNode.options.Add(option);

            //bargain
            option = new DiaOption("PES_RaidNeg_NegDeeper_Bargain".Translate() + "\n");
            if (!goal.CanBargain(out failReason))
            {
                option.disabled = true;
                option.disabledReason = failReason;
            }
            else if (caravan.negotiateCoolDownTick > 0)
                option.Disable("PES_RaidNeg_NegCoolDown".Translate(GenDate.ToStringTicksToPeriod(caravan.negotiateCoolDownTick)));
            else
            {
                DiaNode BargainNode()
                {
                    DiaNode bNode = new DiaNode("PES_RaidNeg_NegDeeper_Bargain_Intro".Translate());
                    foreach (var x in goal.BargainChoices())
                        bNode.options.Add(x);
                    bNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));
                    return bNode;
                }
                option.link = BargainNode();
            }
            diaNode.options.Add(option);

            //Remedy
            option = new DiaOption("PES_RaidNeg_NegDeeper_Remedy".Translate());
            if (caravan.negotiateCoolDownTick > 0)
                option.Disable("PES_RaidNeg_NegCoolDown".Translate(GenDate.ToStringTicksToPeriod(caravan.negotiateCoolDownTick)));
            else
            {
                DiaNode RemedyNode()
                {
                    DiaNode rNode = new DiaNode("PES_RaidNeg_Sub_Intro".Translate());
                    DiaOption rOption;
                    if (incident.raidGoalType != RaidGoalType.Rescue)
                    {
                        rOption = new DiaOption("PES_RaidNeg_Sub_Rescue".Translate());
                        if (!RaidingGoal_Rescue.IsAvailableToIncident(incident, out failReason))
                            rOption.Disable(failReason);
                        rOption.link = RemedyDetail(() => { (new RaidingGoal_Rescue()).ApplyToIncident(incident); });
                        rNode.options.Add(rOption);
                    }

                    if (incident.raidGoalType != RaidGoalType.Extortion)
                    {
                        rOption = new DiaOption("PES_RaidNeg_Sub_Extortion".Translate());
                        rOption.link = RemedyDetail(() => { (new RaidingGoal_Extortion()).ApplyToIncident(incident); });
                        rNode.options.Add(rOption);
                        rNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));
                    }
                    return rNode;
                }
                option.link = RemedyNode();
            }
            diaNode.options.Add(option);

            diaNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));

            return diaNode;
        }

        public static DiaNode BargainDetailNode(float difficultFactor, Action successAction)
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;
            RaidingGoal goal = incident.goal;

            if (incident == null) return null;
            if (incident.raidGoalType == RaidGoalType.Smite) return null;

            DiaNode bargainSuccessNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_NegDeeper_Bargain_Success".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, () =>
                {
                    successAction();
                    caravan.ApplyNegotiationCoolDown();
                    DialogUtilities.NegotiatorLearnSocial(true);
                }, true));
                return diaNode;
            }
            DiaNode bargainFailNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_NegDeeper_Bargain_Fail".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_ASHAME", null, () => {
                    caravan.ApplyNegotiationCoolDown();
                    DialogUtilities.NegotiatorLearnSocial(false);
                }, true));
                return diaNode;
            }
            DiaNode bargainSmiteNode()
            {
                DiaNode diaNode = new DiaNode("PES_raidNeg_NegDeeper_Bargain_Smite".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_DAMNIT", null, () =>
                {
                    caravan.Communicable = false;
                    incident.raidGoalType = RaidGoalType.Smite;
                    incident.goal = null;
                    caravan.ApplyNegotiationCoolDown();
                    DialogUtilities.NegotiatorLearnSocial(false);
                }, true));
                return diaNode;
            }

            DiaNode bargainNode = new DiaNode("PES_RaidNeg_NegDeeper_Bargin_Begin".Translate());
            DiaOption option;

            //Persuasion
            float successOdds = Mathf.Clamp01(PES_Settings.BaseBargainPersuasionChance * pawn.NegotiatePowerFactor() * difficultFactor);
            StringBuilder sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_Persuade_noun".Translate(), goal.BargainPersuasionStr));
            sb.AppendLine(OddsIndicator(successOdds, "PES_raidNeg_NegDeeper_Bargain_Success_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, bargainSuccessNode(), null, bargainFailNode());
            bargainNode.options.Add(option);

            //Intimidation
            successOdds = Mathf.Clamp01(PES_Settings.BaseBargainIntimidationSuccessChance * pawn.NegotiatePowerFactor() * difficultFactor);
            float smiteOdds = Mathf.Clamp01(PES_Settings.BaseBargainIntimidationSmiteChance * pawn.NegotiatePowerFactorNeg() * difficultFactor);
            sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_intimidate_noun".Translate(), goal.BargainIntimidationStr));
            sb.AppendLine(OddsIndicator(successOdds, "PES_raidNeg_NegDeeper_Bargain_Success_Name", smiteOdds, "PES_RaidNeg_Negdeeper_Bargain_Smite_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, bargainSuccessNode(), smiteOdds, null, bargainSmiteNode(), null, bargainFailNode());
            bargainNode.options.Add(option);

            //Beguilement
            if (pawn.skills.GetSkill(SkillDefOf.Social).Level >= 15)
            {
                successOdds = Mathf.Clamp01(PES_Settings.BaseBargainBeguilementChance * pawn.NegotiatePowerFactor() * difficultFactor);
                sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_beguile_noun".Translate(), goal.BargainBeguilementStr));
                sb.AppendLine(OddsIndicator(successOdds, "PES_raidNeg_NegDeeper_Bargain_Success_Name"));
                option = new DiaOption(sb.ToString());
                option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, bargainSuccessNode(), null, bargainFailNode());
                bargainNode.options.Add(option);
            }

            bargainNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));

            return bargainNode;
        }

        public static DiaNode RemedyDetail(Action successAction)
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;
            RaidingGoal goal = incident.goal;

            if (incident == null) return null;
            if (incident.raidGoalType == RaidGoalType.Smite) return null;

            DiaNode RemedySuccessNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Sub_success".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, () =>
                {
                    successAction();
                    caravan.ApplyNegotiationCoolDown();
                    DialogUtilities.NegotiatorLearnSocial(true);
                }, true));
                return diaNode;
            }
            DiaNode RemedyFailNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Sub_Fail".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_ASHAME", null, () => {
                    caravan.ApplyNegotiationCoolDown();
                    DialogUtilities.NegotiatorLearnSocial(false);
                }, true));
                return diaNode;
            }
            DiaNode RemedySmiteNode()
            {
                DiaNode diaNode = new DiaNode("PES_raidNeg_NegDeeper_Bargain_Smite".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_DAMNIT", null, () =>
                {
                    caravan.Communicable = false;
                    incident.raidGoalType = RaidGoalType.Smite;
                    incident.goal = null;
                    caravan.ApplyNegotiationCoolDown();
                    DialogUtilities.NegotiatorLearnSocial(false);
                }, true));
                return diaNode;
            }

            DiaNode remedyNode = new DiaNode("PES_RaidNeg_Sub_Intro".Translate());
            DiaOption option;

            //Persuasion
            float successOdds = Mathf.Clamp01(PES_Settings.BaseRemedyPersuasionChance * pawn.NegotiatePowerFactor());
            StringBuilder sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_Persuade_noun".Translate(), goal.RemedyPersuasionStr));
            sb.AppendLine(OddsIndicator(successOdds, "PES_raidNeg_NegDeeper_Bargain_Success_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, RemedySuccessNode(), null, RemedyFailNode());
            remedyNode.options.Add(option);

            //Intimidation
            successOdds = Mathf.Clamp01(PES_Settings.BaseRemedyIntimidationSuccessChance * pawn.NegotiatePowerFactor());
            float smiteOdds = Mathf.Clamp01(PES_Settings.BaseRemedyIntimidationSmiteChance * pawn.NegotiatePowerFactorNeg());
            sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_intimidate_noun".Translate(), goal.RemedyIntimidationStr));
            sb.AppendLine(OddsIndicator(successOdds, "PES_raidNeg_NegDeeper_Bargain_Success_Name", smiteOdds, "PES_RaidNeg_Negdeeper_Bargain_Smite_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, RemedySuccessNode(), smiteOdds, null, RemedySmiteNode(), null, RemedyFailNode());
            remedyNode.options.Add(option);

            //Beguilement
            if (pawn.skills.GetSkill(SkillDefOf.Social).Level >= 15)
            {
                successOdds = Mathf.Clamp01(PES_Settings.BaseRemedyBeguilementChance * pawn.NegotiatePowerFactor());
                sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_beguile_noun".Translate(), goal.RemedyBeguilementStr));
                sb.AppendLine(OddsIndicator(successOdds, "PES_raidNeg_NegDeeper_Bargain_Success_Name"));
                option = new DiaOption(sb.ToString());
                option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, RemedySuccessNode(), null, RemedyFailNode());
                remedyNode.options.Add(option);
            }

            remedyNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));

            return remedyNode;
        }

        public static DiaNode DelayNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;
            InterceptedIncident_HumanCrowd_RaidEnemy incident = caravan.incident as InterceptedIncident_HumanCrowd_RaidEnemy;

            if (incident == null) return null;
            if (incident.raidGoalType == RaidGoalType.Smite) return null;

            DiaNode delaySuccessNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Delay_Success".Translate(caravan.CaravanTitle));
                diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, () => {
                    caravan.StageForThreeHours();
                    DialogUtilities.NegotiatorLearnSocial(true);
                }, true));
                return diaNode;
            }
            DiaNode delayFailNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Delay_Fail".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_DAMNIT", null, () => {
                    caravan.ApplyDelayCoolDown();
                    DialogUtilities.NegotiatorLearnSocial(false);
                }, true));
                return diaNode;
            }
            DiaNode delaySmiteNode()
            {
                DiaNode diaNode = new DiaNode("PES_RaidNeg_Delay_Smite".Translate());
                diaNode.options.Add(DialogUtilities.CurtOption("PES_DAMNIT", null, () =>
                {
                    caravan.Communicable = false;
                    incident.raidGoalType = RaidGoalType.Smite;
                    incident.goal = null;
                    DialogUtilities.NegotiatorLearnSocial(false);
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
            string intimidationText = ("PES_RaidNeg_Delay_Intimidation_" + incident.raidGoalType.ToString()).Translate();
            sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_intimidate_noun".Translate(), intimidationText));
            sb.AppendLine(OddsIndicator(successOdds, "PES_RaidNeg_Delay_Success_Name", smiteOdds, "PES_RaidNeg_Delay_Smite_Name"));
            option = new DiaOption(sb.ToString());
            option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, delaySuccessNode(), smiteOdds, null, delaySmiteNode(), null, delayFailNode());
            delayNode.options.Add(option);

            //Beguilement
            if (pawn.skills.GetSkill(SkillDefOf.Social).Level >= 15)
            {
                successOdds = Mathf.Clamp01(PES_Settings.BaseDelayBeguilementChance * pawn.NegotiatePowerFactor());
                string beguilementText = ("PES_RaidNeg_Delay_Beguilement_" + incident.raidGoalType.ToString()).Translate();
                sb = new StringBuilder(string.Format("[{0}]: {1}\n", "PES_beguile_noun".Translate(), beguilementText));
                sb.AppendLine(OddsIndicator(successOdds, "PES_RaidNeg_Delay_Success_Name"));
                option = new DiaOption(sb.ToString());
                option.action = DialogUtilities.ResolveActionByOdds(successOdds, null, delaySuccessNode(), null, delayFailNode());
                delayNode.options.Add(option);
            }

            delayNode.options.Add(DialogUtilities.CurtOption("PES_Cancel", null, null, true));

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
