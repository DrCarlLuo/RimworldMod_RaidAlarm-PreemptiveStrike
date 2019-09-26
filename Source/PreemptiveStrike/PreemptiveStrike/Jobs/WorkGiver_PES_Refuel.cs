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
    class WorkGiver_PES_Refuel : WorkGiver_Refuel
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Undefined);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            foreach (var building in pawn.Map.listerBuildings.allBuildingsColonist)
            {
                if (building.TryGetComp<CompUpgrade>() != null && building.TryGetComp<CompRefuelable>() != null)
                    yield return building;
            }
            yield break;
        }

    }
}
