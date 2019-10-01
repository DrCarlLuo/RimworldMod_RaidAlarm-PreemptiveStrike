using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_HumanCrowd_VisitorGroup : InterceptedIncident_HumanCrowd_Neutral
    {
        public override string IntentionStr => "PES_Intention_Visitor".Translate();

        protected override void SetInterceptFlag(bool value)
        {
            IncidentInterceptorUtility.isIntercepting_VisitorGroup = value;
        }

    }
}
