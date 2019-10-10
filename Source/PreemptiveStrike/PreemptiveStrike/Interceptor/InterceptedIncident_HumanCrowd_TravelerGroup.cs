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
        public override string IntentionStr => "PES_Intention_Traveler".Translate();

        public override string IncidentTitle_Confirmed
        {
            get
            {
                if (incidentTitle_Confirmed == null)
                {
                    incidentTitle_Confirmed = PES_NameGenerator.TravelerName(SourceFaction.Name);
                }
                return incidentTitle_Confirmed;
            }
        }

        protected override void SetInterceptFlag(bool value)
        {
            IncidentInterceptorUtility.isIntercepting_TravelerGroup = value;
        }

    }
}
