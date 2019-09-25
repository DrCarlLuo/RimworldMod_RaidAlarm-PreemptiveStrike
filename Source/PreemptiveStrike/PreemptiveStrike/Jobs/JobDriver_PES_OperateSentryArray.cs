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
    class JobDriver_PES_OperateSentryArray : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOn(delegate ()
            {
                var comp = this.job.targetA.Thing.TryGetComp<CompDetection_ManualDevice>();
                return !comp.CanUseNow;
            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil work = new Toil();
            work.tickAction = delegate ()
            {
                Pawn actor = work.actor;
                Building building = (Building)actor.CurJob.targetA.Thing;

                actor.skills.Learn(SkillDefOf.Intellectual, 0.035f, false);
                actor.GainComfortFromCellIfPossible();

                var comp = this.job.targetA.Thing.TryGetComp<CompDetection_ManualDevice>();
                comp.Use(actor);
            };
            work.defaultCompleteMode = ToilCompleteMode.Never;
            work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            //work.activeSkill = (() => SkillDefOf.Intellectual);
            yield return work;
        }
    }
}
