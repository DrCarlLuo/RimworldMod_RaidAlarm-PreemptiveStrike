using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_SolarFlare : InterceptedIncident
    {
        public override string IncidentTitle_Confirmed => "PES_Warning_Flare_Early".Translate();

        public override string IncidentTitle_Unknow => "PES_Warning_Flare_Early".Translate();

        public override string IntentionStr => "";

        public override IncidentIntelLevel IntelLevel => IncidentIntelLevel.Danger;

        public override bool IsHostileToPlayer => true;

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_SolarFlare = WorkerPatchType.ExecuteOrigin;
            parms.target = Find.World;//This is a must, because world referece cannot be saved!
            if (this.incidentDef != null && this.parms != null)
                //this.incidentDef.Worker.TryExecute(this.parms);
                IncidentDefOf.SolarFlare.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");
            IncidentInterceptorUtility.IsIntercepting_SolarFlare = WorkerPatchType.Forestall;
        }

        public override void RevealAllInformation()
        {

        }

        public override void RevealInformationWhenCommunicationEstablished()
        {
            throw new Exception("Should not be called with infestation");
        }

        public override void RevealRandomInformation()
        {
            throw new Exception("Should not be called with infestation");
        }
    }
}
