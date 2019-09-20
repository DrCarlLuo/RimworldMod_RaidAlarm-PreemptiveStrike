using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    abstract class InterceptedIncident : IExposable
    {
        public IncidentDef incidentDef;
        public IncidentParms parms;

        public abstract void ExecuteNow();

        public virtual void ExposeData()
        {
            Scribe_Defs.Look<IncidentDef>(ref incidentDef, "incidentDef");
            Scribe_Deep.Look<IncidentParms>(ref parms, "parms");
        }

        public abstract void RevealRandomInformation();

        public abstract void RevealAllInformation();

        public InterceptedIncident() { }

        public InterceptedIncident(IncidentDef incidentDef, IncidentParms incidentParms)
        {
            this.incidentDef = incidentDef;
            this.parms = incidentParms;
        }


    }
}
