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
    class JobDriver_PES_StandGuard_Primitive : JobDriver
    {
        private int lastRotateTick;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA.Thing.InteractionCell;
            Job job = this.job;
            //Reserve Interaction Cell, so that this job wouldn't affect refueling job or updation job
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            this.FailOn(delegate ()
            {
                var comp = this.job.targetA.Thing.TryGetComp<CompDetection_ManualDevice>();
                return !comp.CanUseNow;
            });

            Toil work = new Toil();
            lastRotateTick = Find.TickManager.TicksGame;
            work.tickAction = delegate ()
            {
                Pawn actor = work.actor;
                actor.skills.Learn(SkillDefOf.Shooting, 0.035f, false);
                
                if (Find.TickManager.TicksGame - lastRotateTick >= 300)
                {
                    rotateToFace = TargetIndex.B;
                    actor.Rotation = Rot4.Random;
                    lastRotateTick = Find.TickManager.TicksGame;
                }

                var comp = this.job.targetA.Thing.TryGetComp<CompDetection_ManualDevice>();
                comp.Use(actor);
            };
            work.defaultCompleteMode = ToilCompleteMode.Never;
            work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            //work.activeSkill = (() => SkillDefOf.Shooting);
            yield return work;
        }
    }
}
