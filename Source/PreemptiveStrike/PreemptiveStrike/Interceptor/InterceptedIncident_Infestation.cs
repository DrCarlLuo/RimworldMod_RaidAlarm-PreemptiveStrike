using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PreemptiveStrike.Mod;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_Infestation : InterceptedIncident
    {
        private IntVec3 loc;

        public override string IncidentTitle_Confirmed => "PES_Infestation_Title".Translate();

        public override string IncidentTitle_Unknow => "";

        public override string IntentionStr => "";

        public override IncidentIntelLevel IntelLevel => IncidentIntelLevel.Danger;

        public override bool IsHostileToPlayer => true;

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_Infestation = WorkerPatchType.ExecuteOrigin;
            IncidentInterceptorUtility.IsIntercepting_InfestationCell = GeneratorPatchFlag.ReturnTempList;
            IncidentInterceptorUtility.tempInfestationCell = loc;
            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");
            IncidentInterceptorUtility.IsIntercepting_InfestationCell = GeneratorPatchFlag.Generate;
            IncidentInterceptorUtility.IsIntercepting_Infestation = WorkerPatchType.Forestall;
        }

        public bool DeterminSpot()
        {
            Map map = parms.target as Map;
            IncidentInterceptorUtility.IsIntercepting_InfestationCell = GeneratorPatchFlag.Generate;
            if (!InfestationCellFinder.TryFindCell(out loc, parms.target as Map))
                return false;
            lookTargets = new TargetInfo(loc, map, false);
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref loc, "loc");
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
