using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Things
{
    class CompProperties_AffectedBySolarFlareEarly : CompProperties
    {
       public CompProperties_AffectedBySolarFlareEarly()
        {
            compClass = typeof(CompAffectedBySolarFlareEarly);
        }
    }
}
