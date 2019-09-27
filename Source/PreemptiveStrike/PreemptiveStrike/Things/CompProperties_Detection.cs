using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Things
{
    class CompProperties_Detection : CompProperties
    {
        public int visionRangeProvide = 0;
        public int detectionRangeProvide = 0;
        public bool NotUsableUnderDarkness = false;
        public bool AffectedByOperatorSightAbility = false;
        public bool UsableWithoutPower = true;
        public bool AffectedByWeather = true;
    }
}
