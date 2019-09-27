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
    abstract class CompBuildingAlarm : ThingComp
    {
        public bool Alarming = true;
    }
}
