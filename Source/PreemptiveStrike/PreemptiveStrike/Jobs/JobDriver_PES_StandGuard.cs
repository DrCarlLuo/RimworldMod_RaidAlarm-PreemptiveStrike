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
        private TowerBuildingBase tower;
        private int lastPatrolTick;
        private int lastRotateTick;
        int curPatrolPos;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            //Reserve Interaction Cell, so that this job wouldn't affect refueling job or updation job
            return pawn.Reserve(job.targetA.Thing.InteractionCell, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            tower = TargetA.Thing as TowerBuildingBase;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOn(delegate ()
            {
                var comp = this.job.targetA.Thing.TryGetComp<CompDetection_ManualDevice>();
                return !comp.CanUseNow || tower == null;
            });

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            Toil work = new Toil();
            lastPatrolTick = Find.TickManager.TicksGame;
            lastRotateTick = Find.TickManager.TicksGame;
            curPatrolPos = 0;


            work.tickAction = delegate ()
            {
                Pawn actor = work.actor;
                //actor.skills.Learn(SkillDefOf.Shooting, 0.035f, false);

                if (Find.TickManager.TicksGame - lastPatrolTick >= tower.PatrolIntervalTick)
                {
                    ++curPatrolPos;
                    if (curPatrolPos == tower.PatrolPointCnt)
                        curPatrolPos = 0;
                    actor.pather.StartPath(tower.GetPatrolPos(curPatrolPos), PathEndMode.OnCell);
                    lastPatrolTick = Find.TickManager.TicksGame;
                    lastRotateTick = Find.TickManager.TicksGame;
                }
                else if (Find.TickManager.TicksGame - lastRotateTick >= tower.RotateIntervalTick)
                {
                    rotateToFace = TargetIndex.B;
                    actor.Rotation = tower.GetPatrolRot(curPatrolPos);
                    lastRotateTick = Find.TickManager.TicksGame;
                }
                var comp = this.job.targetA.Thing.TryGetComp<CompDetection_ManualDevice>();
                comp.Use(actor);
            };
            work.defaultCompleteMode = ToilCompleteMode.Never;
            //work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            //work.activeSkill = (() => SkillDefOf.Shooting);
            yield return work;
        }

    }
}
