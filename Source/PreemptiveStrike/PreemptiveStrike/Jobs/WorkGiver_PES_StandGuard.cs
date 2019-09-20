using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace PreemptiveStrike.Jobs
{
    class WorkGiver_PES_StandGuard : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
            => ThingRequest.ForDef(PESDefOf.PES_watchtower);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(PESDefOf.PES_watchtower).Cast<Thing>();
        }

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return false;
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
            return pawn.CanReserve(target, 1, -1, null, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(PESDefOf.PES_StandGuard, t, 1500, true);
        }

    }
}
