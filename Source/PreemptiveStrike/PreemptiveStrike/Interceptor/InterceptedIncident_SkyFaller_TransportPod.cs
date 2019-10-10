using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Reflection;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_SkyFaller_TransportPod : InterceptedIncident_SkyFaller
    {
        private IntVec3 DropSpot;

        public override SkyFallerType FallerType => SkyFallerType.Small;

        public override string IncidentTitle_Confirmed => "PES_Skyfaller_RefugeePod".Translate();

        public override bool IsHostileToPlayer => false;

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_TransportPod = WorkerPatchType.ExecuteOrigin;
            IncidentInterceptorUtility.IsIntercepting_RandomDropSpot = GeneratorPatchFlag.ReturnTempList;
            IncidentInterceptorUtility.tempRandomDropCell = DropSpot;
            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");
            IncidentInterceptorUtility.IsIntercepting_RandomDropSpot = GeneratorPatchFlag.Generate;
            IncidentInterceptorUtility.IsIntercepting_TransportPod = WorkerPatchType.Forestall;
        }

        public override bool PreCalculateDroppingSpot()
        {
            Map map = parms.target as Map;
            IncidentInterceptorUtility.IsIntercepting_RandomDropSpot = GeneratorPatchFlag.Generate;
            DropSpot = DropCellFinder.RandomDropSpot(map);
            lookTargets = new TargetInfo(DropSpot, map, false);
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DropSpot, "DropSpot", IntVec3.Zero);
        }

        public override void confirmMessage()
        {
            Messages.Message("PES_Notify_RefugeePod".Translate(), MessageTypeDefOf.NeutralEvent);
        }
    }
}
