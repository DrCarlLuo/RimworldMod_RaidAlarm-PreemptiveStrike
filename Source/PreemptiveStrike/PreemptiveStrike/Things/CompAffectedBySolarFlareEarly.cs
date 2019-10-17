using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Interceptor;

namespace PreemptiveStrike.Things
{
    class CompAffectedBySolarFlareEarly : ThingComp
    {
        private bool found = false;
        private int counter = 0;

        public override void CompTick()
        {
            base.CompTick();
            --counter;
            if (counter <= 0)
            {
                found = false;
                foreach(var c in IncidentCaravanUtility.IncidentCaravans)
                {
                    if (c.incident is InterceptedIncident_SolarFlare)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    parent.BroadcastCompSignal("DisableDetection");
                else
                    parent.BroadcastCompSignal("EnableDetection");
                counter = 250;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (found)
                return "PES_Building_UnUsableSolarFlare".Translate();
            else
                return null;
        }
    }
}
