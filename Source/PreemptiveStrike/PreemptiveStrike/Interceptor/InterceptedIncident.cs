using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    abstract class InterceptedIncident
    {
        public IncidentDef incidentDef;
        public IncidentParms parms;

        public abstract void ExecuteNow();

        public InterceptedIncident(IncidentDef incidentDef, IncidentParms incidentParms)
        {
            this.incidentDef = incidentDef;
            this.parms = incidentParms;
        }
    }
}
