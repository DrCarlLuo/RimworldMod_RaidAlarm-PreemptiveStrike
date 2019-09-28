using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Interceptor;

namespace PreemptiveStrike.Dialogue
{
    static class DialogMaker_TryToContact
    {
        public static DiaNode PrologueNode(Pawn pawn, TravelingIncidentCaravan caravan)
        {
            if (!(caravan.incident is InterceptedIncident_HumanCrowd incident))
                return null;
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

            if (incident.faction_revealed && incident.SourceFaction == Faction.OfMechanoids)
            {
                option = new DiaOption("PES_TryContactPrologue_Mechanoid".Translate());
                option.resolveTree = true;
                dianode.options.Add(option);
            }
            else
            {
                //Persuade
                if (incident.IntelLevel == IncidentIntelLevel.Unknown)
                    key = "PES_TryContact_Persuasion_Unknown";
                else if (incident.IntelLevel == IncidentIntelLevel.Danger)
                    key = "PES_TryContact_Persuasion_Hostile";
                else
                    key = "PES_TryContact_Persuasion_Neutral";
                string persuasion = string.Format(@"<b>[{0}]</b>  ", "PES_Persuade_noun".Translate().CapitalizeFirst());
                persuasion += key.Translate(pawn.Name.ToStringShort, Faction.OfPlayer.Name);
                option = new DiaOption(persuasion);
                option.resolveTree = true;
                dianode.options.Add(option);
                //Intimidate
                string intimidation = string.Format(@"<b>[{0}]</b>  ", "PES_intimidate_noun".Translate()).CapitalizeFirst();
                intimidation += "PES_TryContact_Intimidation_have_mortar".Translate();
                option = new DiaOption(intimidation);
                option.resolveTree = true;
                dianode.options.Add(option);
                //Beguild
                string beguile = string.Format(@"<b>[{0}]</b>  ", "PES_beguile_noun".Translate().CapitalizeFirst());
                if (incident.faction_revealed)
                    beguile += "PES_TryContact_Beguilement_FactionConfirmed".Translate(pawn.Name.ToStringShort, Faction.OfPlayer.Name, incident.SourceFaction.Name);
                else
                    beguile += "PES_TryContact_Beguilement".Translate(pawn.Name.ToStringShort, Faction.OfPlayer.Name);
                option = new DiaOption(beguile);
                option.resolveTree = true;
                dianode.options.Add(option);
            }
            return dianode;
        }
    }
}
