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
    static class DialogMaker_Friendly
    {
        public static DiaNode FriendlyNode()
        {
            TravelingIncidentCaravan caravan = DialogUtilities.tempCaravan;
            Pawn pawn = DialogUtilities.tempPawn;

            if (!(caravan.incident is InterceptedIncident_HumanCrowd_Neutral incident))
                return null;

            DiaNode diaNode = new DiaNode("PES_Friendly_Greeting".Translate(caravan.CaravanTitle, pawn.Name.ToStringShort) + "\n");

            DiaNode diaNodeLeaving = new DiaNode("PES_Friendly_Dismissed".Translate());
            diaNodeLeaving.options.Add(DialogUtilities.NormalDisconnectOption());

            DiaOption option = new DiaOption("PES_Friendly_Dismiss".Translate()+"\n");
            option.link = diaNodeLeaving;
            option.action = () => { caravan.Dismiss(); };

            diaNode.options.Add(option);
            diaNode.options.Add(DialogUtilities.NormalDisconnectOption());

            return diaNode;
        }
    }
}
