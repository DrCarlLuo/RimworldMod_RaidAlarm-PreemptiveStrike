using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using PreemptiveStrike.Things;

namespace PreemptiveStrike.Jobs
{
    class WorkGiver_PES_OperateSentryArray : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(PESDefOf.PES_SentryDroneArray);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(PESDefOf.PES_SentryDroneArray).Cast<Thing>();
        }

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < allBuildingsColonist.Count; i++)
            {
                if (allBuildingsColonist[i].def == PESDefOf.PES_SentryDroneArray)
                {
                    CompDetection_ManualDevice comp = allBuildingsColonist[i].GetComp<CompDetection_ManualDevice>();
                    if (comp != null && comp.CanUseNow)
                        return false;
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Faction != pawn.Faction)
                return false;
            Building building = t as Building;
            if (building == null)
                return false;
            if (building.IsForbidden(pawn))
                return false;
            if (building.IsBurning())
                return false;
            LocalTargetInfo target = building;
            if (!pawn.CanReserve(target, 1, -1, null, forced))
            {
                return false;
            }
            CompDetection_ManualDevice comp = building.TryGetComp<CompDetection_ManualDevice>();
            return comp.CanUseNow;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(PESDefOf.PES_OperateSentryArray, t, 1500, true);
        }
    }
}
