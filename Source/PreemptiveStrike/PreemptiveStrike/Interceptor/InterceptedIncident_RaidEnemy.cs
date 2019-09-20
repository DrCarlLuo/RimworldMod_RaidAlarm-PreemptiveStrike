using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_RaidEnemy : InterceptedIncident
    {
        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_IncidentExcecution = false;

            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");

            IncidentInterceptorUtility.IsIntercepting_IncidentExcecution = true;
        }

        public InterceptedIncident_RaidEnemy() { }

        public InterceptedIncident_RaidEnemy(IncidentParms parms) : base(IncidentDefOf.RaidEnemy, parms) { }
    }
}
