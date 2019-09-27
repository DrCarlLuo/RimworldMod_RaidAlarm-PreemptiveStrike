using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Things
{
    class CompProperties_BuildingAlarm_Primitive : CompProperties
    {
        public CompProperties_BuildingAlarm_Primitive()
        {
            compClass = typeof(CompBuildingAlarm_Primitive);
        }

        public Vector3 DrawOffset = Vector3.zero;
    }
}
