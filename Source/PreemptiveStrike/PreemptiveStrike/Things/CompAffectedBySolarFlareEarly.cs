using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

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
                var queue = Find.Storyteller.incidentQueue;
                bool org = found;
                found = false;
                foreach (QueuedIncident qi in queue)
                {
                    if (qi.FiringIncident.def == IncidentDefOf.SolarFlare && qi.FireTick - Find.TickManager.TicksGame <= 2500 * 12)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    parent.BroadcastCompSignal("DisableDetection");
                else
                    parent.BroadcastCompSignal("EnableDetection");
                if (org != found)
                    EventManger.NotifyCaravanListChange?.Invoke();
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
