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
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return CanRefuelThing(t) && CanRefuel(pawn, t, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return RefuelJob(pawn, t, forced, JobStandard, JobAtomic);
        }

        public static bool CanRefuel(Pawn pawn, Thing t, bool forced = false)
        {
            CompTowerFoodRefuelable compRefuelable = t.TryGetComp<CompTowerFoodRefuelable>();
            if (compRefuelable == null || compRefuelable.IsFull)
            {
                return false;
            }
            bool flag = !forced;
            if (flag && !compRefuelable.ShouldAutoRefuelNow)
            {
                return false;
            }
            if (!t.IsForbidden(pawn))
            {
                LocalTargetInfo target = t;
                if (pawn.CanReserve(target, 1, -1, null, forced))
                {
                    if (t.Faction != pawn.Faction)
                    {
                        return false;
                    }
                    if (FindBestFuel(pawn, t) == null)
                    {
                        ThingFilter fuelFilter = compRefuelable.FuelFilter;
                        JobFailReason.Is("NoFuelToRefuel".Translate(fuelFilter.Summary), null);
                        return false;
                    }
                    if (compRefuelable.Props.atomicFueling)
                    {
                        Log.Error("Refuel in Tower shouldn't be atomic");
                    }
                    return true;
                }
            }
            return false;
        }

        private static Thing FindBestFuel(Pawn pawn, Thing refuelable)
        {
            ThingFilter filter = refuelable.TryGetComp<CompTowerFoodRefuelable>().FuelFilter;
            IntVec3 position = pawn.Position;
            Map map = pawn.Map;
            ThingRequest bestThingRequest = filter.BestThingRequest;
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);

            Predicate<Thing> validator = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) && filter.Allows(x);
            return GenClosest.ClosestThingReachable(position, map, bestThingRequest, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }

        public override JobDef JobStandard => PESDefOf.PES_Job_RefuelByNutrition;

        public static Job RefuelJob(Pawn pawn, Thing t, bool forced = false, JobDef customRefuelJob = null, JobDef customAtomicRefuelJob = null)
        {
            if (!t.TryGetComp<CompTowerFoodRefuelable>().Props.atomicFueling)
            {
                Thing t2 = FindBestFuel(pawn, t);
                return new Job(customRefuelJob ?? JobDefOf.Refuel, t, t2);
            }
            Log.Error("Refuel in Tower shouldn't be atomic");
            return null;
        }
    }
}
