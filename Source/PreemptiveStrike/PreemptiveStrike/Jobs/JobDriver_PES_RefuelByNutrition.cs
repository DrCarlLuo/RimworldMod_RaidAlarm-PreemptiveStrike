using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PreemptiveStrike.Jobs
{
    class JobDriver_PES_RefuelByNutrition : JobDriver_Refuel
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            base.AddEndCondition(() => (!this.RefuelableComp.IsFull) ? JobCondition.Ongoing : JobCondition.Succeeded);
            base.AddFailCondition(() => !this.job.playerForced && !this.RefuelableComp.ShouldAutoRefuelNowIgnoringFuelPct);
            yield return Toils_General.DoAtomic(delegate
            {
                //Calc Nutrition
                float remains = (RefuelableComp.TargetFuelLevel - RefuelableComp.Fuel) / RefuelableComp.Props.FuelMultiplierCurrentDifficulty;
                float nutrition = FoodUtility.GetNutrition(TargetB.Thing, TargetB.Thing.def);
                int curNum = 1;
                if (nutrition < float.Epsilon)
                    curNum = 1;
                else
                    curNum = Mathf.CeilToInt(remains / nutrition);
                Log.Message(curNum.ToString());
                this.job.count = curNum;
            });
            Toil reserveFuel = Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return reserveFuel;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveFuel, TargetIndex.B, TargetIndex.None, true, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(240, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return Toils_Refuel.FinalizeRefueling(TargetIndex.A, TargetIndex.B);
            yield break;
        }
    }
}
