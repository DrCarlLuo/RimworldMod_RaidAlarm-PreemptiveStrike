using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Reflection;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_SkyFaller_ShipChunk : InterceptedIncident_SkyFaller
    {
        private IntVec3 DropSpot;

        public override SkyFallerType FallerType => SkyFallerType.Big;

        public override string IncidentTitle_Confirmed => "PES_Skyfaller_ShipChunk".Translate();

        public override bool IsHostileToPlayer => false;

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_ShipChunk = WorkerPatchType.ExecuteOrigin;
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.ReturnTempList;
            IncidentInterceptorUtility.tempSkyfallerCellLoose = DropSpot;
            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.Generate;
            IncidentInterceptorUtility.IsIntercepting_ShipChunk = WorkerPatchType.Forestall;
        }

        public override bool PreCalculateDroppingSpot()
        {
            Map map = parms.target as Map;
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.Generate;

            object[] invokeParms = new object[] { map.Center, map, 999999, null};
            var method = typeof(IncidentWorker_ShipChunkDrop).GetMethod("TryFindShipChunkDropCell", BindingFlags.NonPublic | BindingFlags.Instance);
            if (!(bool)method.Invoke(new IncidentWorker_ShipChunkDrop(), invokeParms))
                return false;
            DropSpot = (IntVec3)invokeParms[3];
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
            Messages.Message("PES_Notify_ShipChunk".Translate(), MessageTypeDefOf.NeutralEvent);
        }
    }
}
