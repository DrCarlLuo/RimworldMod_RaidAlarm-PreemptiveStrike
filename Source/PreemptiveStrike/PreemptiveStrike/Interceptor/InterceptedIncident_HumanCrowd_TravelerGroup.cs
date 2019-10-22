using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;
using System.Reflection;

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

        public override bool ManualDeterminParams()
        {
            MethodInfo vanillaParmsResolver = typeof(IncidentWorker_NeutralGroup).GetMethod("TryResolveParms", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)vanillaParmsResolver.Invoke(new IncidentWorker_TravelerGroup(), new object[] { parms });
            if (!result)
            {
                return false;
            }
            return base.ManualDeterminParams();
        }

        protected override void SetInterceptFlag(bool value)
        {
            IncidentInterceptorUtility.isIntercepting_TravelerGroup = value;
        }

    }
}
