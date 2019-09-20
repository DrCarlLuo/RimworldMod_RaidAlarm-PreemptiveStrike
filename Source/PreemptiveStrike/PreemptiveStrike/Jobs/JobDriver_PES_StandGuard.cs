using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace PreemptiveStrike.Jobs
{
    class JobDriver_PES_StandGuard : JobDriver
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
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            Toil work = new Toil();
            work.tickAction = delegate ()
            {
                Pawn actor = work.actor;
                actor.skills.Learn(SkillDefOf.Shooting, 0.035f, false);
                var dic = DetectionSystem.DetectDangerUtilities.LastWatchTowerUsedTickInMapTile;
                if (dic != null && actor.Map != null)
                    dic[actor.Map.Tile] = Find.TickManager.TicksGame;
            };
            work.defaultCompleteMode = ToilCompleteMode.Never;
            work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            work.activeSkill = (() => SkillDefOf.Shooting);
            yield return work;
        }
    }
}
