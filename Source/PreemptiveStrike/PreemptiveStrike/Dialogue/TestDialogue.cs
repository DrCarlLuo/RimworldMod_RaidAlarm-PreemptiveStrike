using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Dialogue
{
    class TestDialogue : ICommunicable
    {
        public FloatMenuOption CommFloatMenuOption(Building_CommsConsole console, Pawn negotiator)
        {
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Call 23333", delegate ()
            {
                console.GiveUseCommsJob(negotiator, this);
            }, MenuOptionPriority.InitiateSocial), negotiator, console);
        }

        public string GetCallLabel()
        {
            return "Carl";
        }

        public Faction GetFaction()
        {
            return Faction.OfMechanoids;
        }

        public string GetInfoText()
        {
            return "2333";
        }

        public void TryOpenComms(Pawn negotiator)
        {
            Dialog_Negotiation dialog_Negotiation = new Dialog_Negotiation(negotiator, this, TestDiaNode(), true);
            dialog_Negotiation.soundAmbient = SoundDefOf.RadioComms_Ambience;
            Find.WindowStack.Add(dialog_Negotiation);
        }

        public static DiaNode TestDiaNode()
        {
            DiaNode diaNode = new DiaNode("Hell YEAH!!!!");
            DiaOption op1 = new DiaOption("It's too late, organics");
            op1.Disable("Determined exterminator");
            DiaOption op2 = new DiaOption("Yesterday is but a dream, tomorrow is only a vision, but today well lived, makes every yesterday a dream of happiness and every tomorrow a vision of hope; To achieve great things, two things are needed, a plan and not quite enough time");
            op2.resolveTree = true;
            DiaOption op3 = new DiaOption("2333");
            op3.action = () => { Messages.Message("1234", MessageTypeDefOf.NeutralEvent); };
            DiaNode d1 = new DiaNode("good day");
            DiaOption op4 = new DiaOption("how do you do?");
            op4.link = d1;
            diaNode.options.Add(op1);
            diaNode.options.Add(op2);
            diaNode.options.Add(op3);
            diaNode.options.Add(op4);
            return diaNode;
        }
    }

}
