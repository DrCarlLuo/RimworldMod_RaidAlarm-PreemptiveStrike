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
    class WorkGiver_PES_InstallUpgrade : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
            => ThingRequest.ForGroup(ThingRequestGroup.Undefined);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            IEnumerable<Designation> buildings = pawn.Map.designationManager.SpawnedDesignationsOfDef(PESDefOf.PES_InstallUpgrade);
            foreach (Designation des in buildings)
            {
                Thing designatedThing = des.target.Thing;
                if (designatedThing is ThingWithComps thing && thing.AllComps.OfType<CompUpgrade>().Any((CompUpgrade comp) => comp.beginUpgrade))
                {
                    yield return designatedThing;
                }
                designatedThing = null;
            }
            yield break;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is ThingWithComps thing)
            {
                if (!pawn.Dead && !pawn.Downed && !pawn.IsBurning() && !thing.Destroyed && !thing.IsBurning() && pawn.CanReserveAndReach(thing, PathEndMode.InteractionCell, Danger.Deadly))
                {
                    CompUpgrade comp = null;
                    foreach (var c in thing.AllComps.OfType<CompUpgrade>())
                    {
                        if (c.beginUpgrade)
                        {
                            comp = c;
                            break;
                        }
                    }
                    if (comp == null) return null;

                    ThingDefCount needThingCount = comp.TryGetOneMissingMaterial();

                    if (!comp.PawnMeetsSkillRequirement(pawn))
                    {
                        JobFailReason.Is("ConstructionSkillTooLow");
                        return null;
                    }
                    if (needThingCount.Count > 0)
                    {
                        Thing foundRes = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(needThingCount.ThingDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly), 9999f, (Thing r) => { return r.def == needThingCount.ThingDef && !r.IsForbidden(pawn) && pawn.CanReserve(r); });
                        if (foundRes == null)
                        {
                            JobFailReason.Is("Upgrade_missingMaterial");
                            return null;
                        }
                    }
                    return new Job(PESDefOf.PES_Job_InstallUpgrade, t);
                }
            }
            return null;
        }

    }
}
