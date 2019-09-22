using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_HumanCrowd_TravelerGroup : InterceptedIncident_HumanCrowd_Neutral
    {
        protected override void SetInterceptFlag(bool value)
        {
            IncidentInterceptorUtility.isIntercepting_TravelerGroup = value;
        }

    }
}
