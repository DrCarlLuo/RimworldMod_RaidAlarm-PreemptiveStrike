using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    enum IncidentIntelLevel
    {
        Danger,
        Unknown,
        Neutral
    }

    abstract class InterceptedIncident : IExposable
    {
        public IncidentDef incidentDef;
        public IncidentParms parms;

        private string incidentTitle_Confirmed = null;
        public abstract string IncidentTitle_Confirmed { get; }

        private string incidentTitle_Unknow = null;
        public abstract string IncidentTitle_Unknow { get; }
        

        public abstract IncidentIntelLevel IntelLevel { get; }

        public abstract bool IsHostileToPlayer { get; }

        public abstract void ExecuteNow();

        public virtual void ExposeData()
        {
            Scribe_Defs.Look<IncidentDef>(ref incidentDef, "incidentDef");
            Scribe_Deep.Look<IncidentParms>(ref parms, "parms");
        }

        public virtual bool SubstituionWorkerExecution()
        {
            Log.Error("Substitution Worker not implemented!!!");
            return false;
        }

        public virtual bool ManualDeterminParams()
        {
            Log.Error("Manual Params Determination not implemented!!!");
            return false;
        }

        public abstract void RevealRandomInformation();

        public abstract void RevealAllInformation();

        public abstract void RevealInformationWhenCommunicationEstablished();

    }
}
