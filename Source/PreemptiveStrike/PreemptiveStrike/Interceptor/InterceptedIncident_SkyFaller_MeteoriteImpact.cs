using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PreemptiveStrike.Mod;
using RimWorld;
using Verse;
using System.Reflection;
using PreemptiveStrike.Dialogue;

namespace PreemptiveStrike.Interceptor
{
    class InterceptedIncident_SkyFaller_MeteoriteImpact : InterceptedIncident_SkyFaller
    {
        private IntVec3 DropSpot;

        public override SkyFallerType FallerType => SkyFallerType.Big;

        public override string IncidentTitle_Confirmed => "PES_Skyfaller_Meteorite".Translate();

        public override bool IsHostileToPlayer => false;

        public override void ExecuteNow()
        {
            IncidentInterceptorUtility.IsIntercepting_Meteorite = WorkerPatchType.ExecuteOrigin;
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.ReturnTempList;
            IncidentInterceptorUtility.tempSkyfallerCellLoose = DropSpot;
            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.Generate;
            IncidentInterceptorUtility.IsIntercepting_Meteorite = WorkerPatchType.Forestall;
        }

        public override bool PreCalculateDroppingSpot()
        {
            object[] invokeParms = new object[] { null, parms.target as Map };
            IncidentInterceptorUtility.IsIntercepting_SkyfallerCell_Loose = GeneratorPatchFlag.Generate;

            var method = typeof(IncidentWorker_MeteoriteImpact).GetMethod("TryFindCell", BindingFlags.NonPublic | BindingFlags.Instance);
            if (!(bool)method.Invoke(new IncidentWorker_MeteoriteImpact(), invokeParms))
                return false;
            DropSpot = (IntVec3)invokeParms[0];
            lookTargets = new TargetInfo(DropSpot, parms.target as Map, false);
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DropSpot, "DropSpot", IntVec3.Zero);
        }

        public override void confirmMessage()
        {
            SparkUILetter.Make("PES_Warning_Meteorite".Translate(), "PES_Warning_Meteorite_Text".Translate(), LetterDefOf.NeutralEvent, parentCaravan);
        }
    }
}
