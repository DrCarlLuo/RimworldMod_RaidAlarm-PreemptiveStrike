using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Reflection;
using PreemptiveStrike.Dialogue;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_SkyFaller_ShipPartCrash : InterceptedIncident_SkyFaller
    {
        private IntVec3 DropSpot;

        public override SkyFallerType FallerType => SkyFallerType.Big;

        public override string IncidentTitle_Confirmed => "PES_Skyfaller_ShipPart".Translate();

        public override bool IsHostileToPlayer => true;

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_ShipPart = WorkerPatchType.ExecuteOrigin;
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.ReturnTempList;
            IncidentInterceptorUtility.tempSkyfallerCellLoose = DropSpot;
            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.Generate;
            IncidentInterceptorUtility.IsIntercepting_ShipPart = WorkerPatchType.Forestall;
        }

        public override bool PreCalculateDroppingSpot()
        {
            Map map = parms.target as Map;
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.Generate;
            if (!CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.CrashedShipPartIncoming, map, out DropSpot, 14, default, -1, false, true, true, true, true, false, null))
                return false;
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
            SparkUILetter.Make("PES_Warning_ShipPart".Translate(), "PES_Warning_Shippart_Text".Translate(), LetterDefOf.NeutralEvent, parentCaravan);
        }
    }
}
