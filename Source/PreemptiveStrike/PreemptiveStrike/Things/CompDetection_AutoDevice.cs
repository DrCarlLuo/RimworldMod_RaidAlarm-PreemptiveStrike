using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.DetectionSystem;

namespace PreemptiveStrike.Things
{
    class CompDetection_AutoDevice : CompDetection
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
        public override void CompTick()
        {
            base.CompTick();
            if (PowerOnOrDontNeedPower)
            {
                UpdateDetectionAbility(Props.visionRangeProvide, Props.detectionRangeProvide);
                DetectDangerUtilities.LastSolarFlareDetectorTick = Find.TickManager.TicksGame;
            }
        }
    }
}
