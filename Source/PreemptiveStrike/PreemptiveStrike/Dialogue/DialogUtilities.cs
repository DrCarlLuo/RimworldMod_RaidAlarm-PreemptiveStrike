using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.Dialogue
{
    static class DialogUtilities
    {
        public static Pawn tempPawn;
        public static TravelingIncidentCaravan tempCaravan;

        public static void BeginCaravanDialog(Pawn pawn, TravelingIncidentCaravan caravan)
        {
            tempCaravan = caravan;
            tempPawn = pawn;
            if(!caravan.CommunicationEstablished)
                OpenDialog(DialogMaker_TryToContact.PrologueNode());
            else
            {
                if (caravan.incident.IntelLevel == Interceptor.IncidentIntelLevel.Danger)
                    OpenDialog(DialogMaker_RaidNegotiation.PrologueNode());
                else if (caravan.incident.IntelLevel == Interceptor.IncidentIntelLevel.Neutral)
                    OpenDialog(DialogMaker_Friendly.FriendlyNode());
            }
        }

        public static void OpenDialog(DiaNode node)
        {
            Dialog_Negotiation dialog_Negotiation = new Dialog_Negotiation(tempPawn, tempCaravan, node, true);
            dialog_Negotiation.soundAmbient = SoundDefOf.RadioComms_Ambience;
            Find.WindowStack.Add(dialog_Negotiation);
        }

        public static DiaOption NormalDisconnectOption()
        {
            string disconnect = string.Format("({0})", "PES_Disconnect".Translate());
            DiaOption diaOption = new DiaOption(disconnect);
            diaOption.resolveTree = true;
            return diaOption;
        }

        public static DiaOption NormalCancelOption()
        {
            string disconnect = "PES_Cancel".Translate();
            DiaOption diaOption = new DiaOption(disconnect);
            diaOption.resolveTree = true;
            return diaOption;
        }

        public static DiaOption CurtOption(string label, DiaNode nxtNode, Action action, bool end)
        {
            DiaOption diaOption = new DiaOption(label.Translate().CapitalizeFirst());
            diaOption.link = nxtNode;
            diaOption.action = action;
            diaOption.resolveTree = end;
            return diaOption;
        }

        public static Action ResolveActionByOdds(float odds, Action SuccessAction, DiaNode SuccessNode, Action FailAction, DiaNode FailNode)
        {
            float dice = new FloatRange(0f, 1f).RandomInRange;
            if (PES_Settings.DebugModeOn) Messages.Message(string.Format("odds: {0} dice:{1}", odds, dice), MessageTypeDefOf.NeutralEvent);

            if (dice <= odds)
                return () => { SuccessAction?.Invoke(); OpenDialog(SuccessNode); };
            else
                return () => { FailAction?.Invoke(); OpenDialog(FailNode); };
        }

        public static Action ResolveActionByOdds(float odds1, Action action1, DiaNode diaNode1, float odds2, Action action2, DiaNode diaNode2, Action FailAction, DiaNode FailNode)
        {
            float dice = new FloatRange(0f, 1f).RandomInRange;
            if (PES_Settings.DebugModeOn) Messages.Message(string.Format("odds: {0}-{1} dice:{2}", odds1, odds2, dice), MessageTypeDefOf.NeutralEvent);
            if (dice <= odds1)
                return () => { action1?.Invoke(); OpenDialog(diaNode1); };
            if (dice <= odds1 + odds2)
                return () => { action2?.Invoke(); OpenDialog(diaNode2); };
            return () => { FailAction?.Invoke(); OpenDialog(FailNode); };
        }

        public static string GetOddsString(float odds)
        {
            if (odds >= 0.8f)
                return "PES_Odds_VeryHigh".Translate();
            if (odds >= 0.6f)
                return "PES_Odds_High".Translate();
            if (odds >= 0.4f)
                return "PES_Odds_Medium".Translate();
            if (odds >= 0.2f)
                return "PES_Odds_Low".Translate();
            return "PES_Odds_VeryLow".Translate();
        }

        public static float MessageReceiveChance => Mathf.Clamp01(1f - 0.2f * Find.Storyteller.difficulty.difficulty + PES_Settings.MessageRecieveOffset);

        public static float NegotiatePowerFactor(this Pawn pawn)
        {
            return pawn.GetStatValue(StatDefOf.NegotiationAbility);
        }

        public static float NegotiatePowerFactorNeg(this Pawn pawn)
        {
            if (pawn.GetStatValue(StatDefOf.NegotiationAbility) == 0)
                return 2.0f;
            return 1f/ pawn.GetStatValue(StatDefOf.NegotiationAbility);
        }

        public static bool MapHasCommsConsole(Map map)
        {
            return map.listerThings.ThingsOfDef(ThingDefOf.CommsConsole).Any(thing => thing.Faction == Faction.OfPlayer);
        }

        public static void NegotiatorLearnSocial(bool success)
        {
            if (success)
                tempPawn.skills.Learn(SkillDefOf.Social, 2000f, true);
            else
                tempPawn.skills.Learn(SkillDefOf.Social, 500f, true);
        }
    }
}
