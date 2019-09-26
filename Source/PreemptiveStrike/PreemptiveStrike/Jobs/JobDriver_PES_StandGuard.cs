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
    class JobDriver_PES_StandGuard : JobDriver
    {
        private int lastRotateTick;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
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
