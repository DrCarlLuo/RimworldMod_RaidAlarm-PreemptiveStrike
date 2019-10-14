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
    static class DialogMaker_TryToContact
    {
        public static DiaNode PrologueNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;

            if (!(caravan.incident is InterceptedIncident_HumanCrowd incident))
                return null;
            if (caravan.broadcastMessageCoolDownTick > 0)
                return ContactCoolDownNode();

            string prologue;
            string key;
            if (incident.IntelLevel == IncidentIntelLevel.Unknown)
                key = "PES_TryContactPrologue_Unknown";
            else if (incident.IntelLevel == IncidentIntelLevel.Danger)
                key = "PES_TryContactPrologue_Hostile";
            else
                key = "PES_TryContactPrologue_Neutral";
            prologue = key.Translate(caravan.CaravanTitle, pawn.Name.ToStringShort);

            DiaNode dianode = new DiaNode(prologue);
            DiaOption option;

            option = new DiaOption("PES_ConfirmChoice".Translate());
            option.link = BroadCastMessageChoiceNode();
            dianode.options.Add(option);

            dianode.options.Add(DialogUtilities.NormalCancelOption());

            return dianode;
        }

        public static DiaNode ContactCoolDownNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;

            DiaNode diaNode = new DiaNode("PES_TryContact_CoolDown".Translate(GenDate.ToStringTicksToPeriod(caravan.broadcastMessageCoolDownTick)));
            diaNode.options.Add(DialogUtilities.CurtOption("PES_Disconnect", null, null, true));
            return diaNode;
        }

        public static DiaNode BroadCastMessageChoiceNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;

            if (!(caravan.incident is InterceptedIncident_HumanCrowd incident))
                return null;

            string intro;
            if (incident.faction_revealed && incident.SourceFaction == Faction.OfMechanoids)
                intro = "PES_BroadCastMessage_Mechanoid".Translate();
            else
                intro = "PES_BroadCastMessage".Translate(pawn.Name.ToStringShort);

            DiaNode diaNode = new DiaNode(intro);
            DiaOption option;

            //Mechanoid
            if (incident.faction_revealed && incident.SourceFaction == Faction.OfMechanoids)
            {
                option = new DiaOption("PES_TryContactPrologue_Mechanoid".Translate())
                {
                    link = MechanoidAnswers()
                };
                diaNode.options.Add(option);
            }
            else
            {
                string key;
                void CommEstablishAction() { caravan.EstablishCommunication(); caravan.ApplyBroadCastCoolDown(); DialogUtilities.NegotiatorLearnSocial(true); }
                void LeaveAction() { caravan.Dismiss(); caravan.ApplyBroadCastCoolDown(); DialogUtilities.NegotiatorLearnSocial(true); }
                void LeaveActionBad() { caravan.Dismiss(); caravan.ApplyBroadCastCoolDown(); DialogUtilities.NegotiatorLearnSocial(false); }
                void FailAction() { caravan.ApplyBroadCastCoolDown(); DialogUtilities.NegotiatorLearnSocial(false); }

                float MessageOdds = incident.SourceFaction == Faction.OfMechanoids ? 0f : DialogUtilities.MessageReceiveChance;

                #region Persuasion
                StringBuilder sb = new StringBuilder(string.Format(@"<b>[{0}]</b>  ", "PES_Persuade_noun".Translate().CapitalizeFirst()));

                //Odds
                float friendlyOdds = Mathf.Clamp01(PES_Settings.BasePersuadeChance_Friendly * pawn.NegotiatePowerFactor());
                float hostileOdds = Mathf.Clamp01(PES_Settings.BasePersuadeChance_Hostile * pawn.NegotiatePowerFactor());

                //Choices
                if (incident.IntelLevel == IncidentIntelLevel.Unknown)
                {
                    key = "PES_TryContact_Persuasion_Unknown";
                    sb.AppendLine(string.Format(@"<i>{0}</i>", key.Translate(pawn.Name.ToStringShort, Faction.OfPlayer.Name)));
                    sb.AppendLine(OddsIndicator(1, friendlyOdds, "PES_TryOutcome_Positive"));
                    sb.AppendLine(OddsIndicator(2, hostileOdds, "PES_TryOutcome_Positive"));
                }
                else if (incident.IntelLevel == IncidentIntelLevel.Danger)
                {
                    key = "PES_TryContact_Persuasion_Hostile";
                    sb.AppendLine(string.Format(@"<i>{0}</i>", key.Translate(pawn.Name.ToStringShort, Faction.OfPlayer.Name)));
                    sb.AppendLine(OddsIndicator(0, hostileOdds, "PES_TryOutcome_Positive"));
                }
                else
                {
                    key = "PES_TryContact_Persuasion_Neutral";
                    sb.AppendLine(string.Format(@"<i>{0}</i>", key.Translate(pawn.Name.ToStringShort, Faction.OfPlayer.Name)));
                    sb.AppendLine(OddsIndicator(0, friendlyOdds, "PES_TryOutcome_Positive"));
                }

                //Action
                option = new DiaOption(sb.ToString());
                if (incident.IsHostileToPlayer)
                    option.action = DialogUtilities.ResolveActionByOdds(hostileOdds * MessageOdds, CommEstablishAction, PersuasionSuccessNode(), FailAction, ContactFailNode());
                else
                    option.action = DialogUtilities.ResolveActionByOdds(friendlyOdds * MessageOdds, CommEstablishAction, PersuasionSuccessNode(), FailAction, ContactFailNode());
                diaNode.options.Add(option);
                #endregion

                #region intimidation
                float friendlyContactOdds = Mathf.Clamp01(PES_Settings.BaseIntimidationContactChance_Friendly * pawn.NegotiatePowerFactor());
                float friendlyFleeOdds = Mathf.Clamp01(PES_Settings.BaseIntimidationFrightenChance_Friendly * pawn.NegotiatePowerFactorNeg());
                float EnemyContactOdds = Mathf.Clamp01(PES_Settings.BaseIntimidationContactChance_Hostile * pawn.NegotiatePowerFactor());
                float EnemyFleeOdds = Mathf.Clamp01(PES_Settings.BaseIntimidationFrightenChance_Hostile * pawn.NegotiatePowerFactor());

                sb = new StringBuilder(string.Format(@"<b>[{0}]</b>  ", "PES_intimidate_noun".Translate()).CapitalizeFirst());
                sb.AppendLine(string.Format(@"<i>{0}</i>", "PES_TryContact_Intimidation_have_mortar".Translate()));
                if (incident.IntelLevel == IncidentIntelLevel.Unknown)
                {
                    sb.AppendLine(OddsIndicator(1, friendlyContactOdds, "PES_TryOutcome_Positive", friendlyFleeOdds, "PES_TryOutcome_Scared"));
                    sb.AppendLine(OddsIndicator(2, EnemyContactOdds, "PES_TryOutcome_Positive", EnemyFleeOdds, "PES_TryOutcome_Scared"));
                }
                else if (incident.IntelLevel == IncidentIntelLevel.Danger)
                    sb.AppendLine(OddsIndicator(0, EnemyContactOdds, "PES_TryOutcome_Positive", EnemyFleeOdds, "PES_TryOutcome_Scared"));
                else
                    sb.AppendLine(OddsIndicator(0, friendlyContactOdds, "PES_TryOutcome_Positive", friendlyFleeOdds, "PES_TryOutcome_Scared"));
                option = new DiaOption(sb.ToString());
                if (!incident.IsHostileToPlayer)
                    option.action = DialogUtilities.ResolveActionByOdds(friendlyContactOdds * MessageOdds, CommEstablishAction, PersuasionSuccessNode(), friendlyFleeOdds * MessageOdds, LeaveActionBad, IntimidationSuccessNode(), FailAction, ContactFailNode());
                else
                    option.action = DialogUtilities.ResolveActionByOdds(EnemyContactOdds * MessageOdds, CommEstablishAction, PersuasionSuccessNode(), EnemyFleeOdds * MessageOdds, LeaveAction, IntimidationSuccessNode(), FailAction, ContactFailNode());
                diaNode.options.Add(option);
                #endregion

                #region Beguilement
                if (incident.IntelLevel != IncidentIntelLevel.Neutral && pawn.skills.GetSkill(SkillDefOf.Social).Level >= 15)
                {
                    friendlyFleeOdds = Mathf.Clamp01(PES_Settings.BaseBeguilementFrightenChance_Friendly * pawn.NegotiatePowerFactor());
                    EnemyContactOdds = Mathf.Clamp01(PES_Settings.BaseBeguilementContactChance_Hostile * pawn.NegotiatePowerFactor());
                    float EnemyLeaveOdds = Mathf.Clamp01(PES_Settings.BaseBeguilementFrightenChance_Hostile * pawn.NegotiatePowerFactor());

                    sb = new StringBuilder(string.Format(@"<b>[{0}]</b>  ", "PES_beguile_noun".Translate().CapitalizeFirst()));
                    if (incident.faction_revealed)
                    {
                        sb.AppendLine(string.Format(@"<i>{0}</i>", "PES_TryContact_Beguilement_FactionConfirmed".Translate(pawn.Name.ToStringShort, incident.SourceFaction.Name, Faction.OfPlayer.Name)));
                        sb.AppendLine(OddsIndicator(0, EnemyContactOdds, "PES_TryOutcome_Positive", EnemyLeaveOdds, "PES_TryOutcome_Leave"));
                    }
                    else
                    {
                        sb.AppendLine(string.Format(@"<i>{0}</i>", "PES_TryContact_Beguilement".Translate(pawn.Name.ToStringShort, Faction.OfPlayer.Name)));
                        if (incident.IntelLevel == IncidentIntelLevel.Danger)
                            sb.AppendLine(OddsIndicator(0, EnemyContactOdds, "PES_TryOutcome_Positive", EnemyLeaveOdds, "PES_TryOutcome_Scared"));
                        else
                        {
                            sb.AppendLine(OddsIndicator(1, friendlyFleeOdds, "PES_TryOutcome_Scared"));
                            sb.AppendLine(OddsIndicator(2, EnemyContactOdds, "PES_TryOutcome_Positive", EnemyLeaveOdds, "PES_TryOutcome_Scared"));
                        }
                    }
                    option = new DiaOption(sb.ToString());
                    if (incident.IsHostileToPlayer)
                        option.action = DialogUtilities.ResolveActionByOdds(EnemyLeaveOdds * MessageOdds, LeaveAction, BeguilementSuccessNode(incident.faction_revealed), EnemyContactOdds * MessageOdds, CommEstablishAction, PersuasionSuccessNode(), FailAction, ContactFailNode());
                    else
                        option.action = DialogUtilities.ResolveActionByOdds(friendlyFleeOdds * MessageOdds, LeaveAction, BeguilementSuccessNode(incident.faction_revealed), FailAction, ContactFailNode());
                    diaNode.options.Add(option);
                }
                #endregion
            }

            diaNode.options.Add(DialogUtilities.NormalCancelOption());
            return diaNode;
        }

        public static DiaNode ContactFailNode()
        {
            DiaNode diaNode = new DiaNode("PES_Contact_Fail".Translate(DialogUtilities.tempPawn.Name.ToStringShort));
            diaNode.options.Add(DialogUtilities.CurtOption("PES_ASHAME", null, null, true));
            return diaNode;
        }

        public static DiaNode PersuasionSuccessNode()
        {
            DiaNode diaNode;
            InterceptedIncident_HumanCrowd incident = DialogUtilities.tempCaravan.incident as InterceptedIncident_HumanCrowd;
            if (incident.IsHostileToPlayer)
            {
                diaNode = new DiaNode("PES_Persuasion_Success_Hostile".Translate(incident.SourceFaction.Name));
                diaNode.options.Add(DialogUtilities.CurtOption("PES_RaiseAlarm", null, null, true));
            }
            else
            {
                diaNode = new DiaNode("PES_Persuation_Success_Friendly".Translate(incident.SourceFaction.Name));
                diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, null, true));
            }
            return diaNode;
        }

        public static DiaNode IntimidationSuccessNode()
        {
            DiaNode diaNode;
            InterceptedIncident_HumanCrowd incident = DialogUtilities.tempCaravan.incident as InterceptedIncident_HumanCrowd;
            if (incident.IsHostileToPlayer)
            {
                diaNode = new DiaNode("PES_Intimidation_SuccessFlee_Hostile".Translate(DialogUtilities.tempCaravan.CaravanTitle, incident.SourceFaction.Name));
                diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, null, true));
            }
            else
            {
                diaNode = new DiaNode("PES_Intimidation_SuccessFlee_Friendly".Translate(DialogUtilities.tempCaravan.CaravanTitle, incident.SourceFaction.Name));
                diaNode.options.Add(DialogUtilities.CurtOption("PES_ASHAME", null, null, true));
            }
            return diaNode;
        }

        public static DiaNode BeguilementSuccessNode(bool factionKnown)
        {
            DiaNode diaNode;
            InterceptedIncident_HumanCrowd incident = DialogUtilities.tempCaravan.incident as InterceptedIncident_HumanCrowd;
            if (incident.IsHostileToPlayer)
            {
                if (factionKnown)
                {
                    diaNode = new DiaNode("PES_Beguilement_SuccessLeave_Hostile".Translate(DialogUtilities.tempCaravan.CaravanTitle));
                    diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, null, true));
                }
                else
                {
                    diaNode = new DiaNode("PES_Intimidation_SuccessFlee_Hostile".Translate(DialogUtilities.tempCaravan.CaravanTitle, incident.SourceFaction.Name));
                    diaNode.options.Add(DialogUtilities.CurtOption("PES_Reassuring", null, null, true));
                }
            }
            else
            {
                diaNode = new DiaNode("PES_Beguilement_SuccessFlee_Friendly".Translate(DialogUtilities.tempCaravan.CaravanTitle, incident.SourceFaction.Name));
                diaNode.options.Add(DialogUtilities.CurtOption("PES_ASHAME", null, null, true));
            }
            return diaNode;
        }

        public static DiaNode MechanoidAnswers()
        {
            int no = new IntRange(1, 2).RandomInRange;
            DiaNode diaNode1 = new DiaNode(("PES_Mechanoid_Outcome_" + no.ToString()).Translate());

            DiaNode diaNode2 = new DiaNode(("PES_MechanoidAnswer_" + no.ToString()).Translate());
            DiaOption option2 = new DiaOption("PES_trembling".Translate());
            option2.resolveTree = true;
            option2.action = () => { DialogUtilities.tempCaravan.Communicable = false; };
            diaNode2.options.Add(option2);

            DiaOption option1 = new DiaOption("PES_Mechanoid_Choice".Translate());
            option1.link = diaNode2;
            diaNode1.options.Add(option1);

            return diaNode1;
        }

        private static string OddsIndicator(int obj, float odds, string verb, float odds2 = 0f, string verb2 = null)
        {
            StringBuilder sb = new StringBuilder("(");
            sb.Append("PES_Condition_Received".Translate().CapitalizeFirst());
            sb.Append(": ");
            if (obj == 1)
            {
                sb.Append("PES_Condition_Neutral".Translate());
                sb.Append(", ");
            }
            if (obj == 2)
            {
                sb.Append("PES_Condition_Hostile".Translate());
                sb.Append(", ");
            }
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
