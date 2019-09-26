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
    class JobDriver_PES_InstallUpgrade : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        private CompUpgrade FindOneComp()
        {
            if (TargetThingA is ThingWithComps thing)
            {
                foreach (var c in thing.AllComps.OfType<CompUpgrade>())
                {
                    if (c.beginUpgrade)
                        return c;
                }
            }
            return null;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CompUpgrade comp = FindOneComp();
            if (comp == null)
            {
                EndJobWith(JobCondition.Incompletable);
                yield break;
            }
            AddFailCondition(() => comp == null || !comp.parent.Spawned || !comp.beginUpgrade);
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);

            Toil gotoBuilding = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil findMaterial = new Toil
            {
                initAction = delegate ()
                {
                    ThingDefCount thingDefCount = comp.TryGetOneMissingMaterial();
                    job.count = thingDefCount.Count;
                    if (thingDefCount.Count > 0)
                    {
                        Thing foundRes = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(thingDefCount.ThingDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly), 9999f, (Thing r) => { return r.def == thingDefCount.ThingDef && !r.IsForbidden(pawn) && pawn.CanReserve(r); });
                        if (foundRes == null)
                            EndJobWith(JobCondition.Incompletable);
                        else
                            job.SetTarget(TargetIndex.B, foundRes);
                    }
                    else
                        JumpToToil(gotoBuilding);
                }
            };

            //Haul things to building
            yield return findMaterial;
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, job.count);
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.Touch).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    var tracker = pawn.carryTracker;
                    if (tracker.CarriedThing != null)
                    {
                        tracker.innerContainer.TryTransferToContainer(tracker.CarriedThing, comp.GetDirectlyHeldThings(), tracker.CarriedThing.stackCount, true);
                        pawn.Map.reservationManager.ReleaseAllForTarget(TargetThingB);
                        job.SetTarget(TargetIndex.B, null);
                        JumpToToil(findMaterial);
                    }
                }
            };
            //Do upgrade
            yield return gotoBuilding;
            yield return new Toil
            {
                tickAction = delegate ()
                {
                    Pawn actor = GetActor();
                    comp.Work(actor, actor.GetStatValue(StatDefOf.ConstructionSpeed));
                    if (comp.complete)
                        EndJobWith(JobCondition.Succeeded);
                },
                defaultCompleteMode = ToilCompleteMode.Never
            }.WithEffect(EffecterDefOf.ConstructMetal, TargetIndex.A).WithProgressBar(TargetIndex.A, () => comp.FinishPercentage);
            yield break;
        }
    }
}
