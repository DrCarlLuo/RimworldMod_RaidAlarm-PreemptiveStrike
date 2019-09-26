using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.DetectionSystem;

namespace PreemptiveStrike.Things
{
    class CompDetection_ManualDevice : CompDetection
    {
        public CompProperties_Detection Props => this.props as CompProperties_Detection;

        public virtual bool CanUseNow => this.parent.Spawned && this.parent.Faction == Faction.OfPlayer && (!Props.NotUsableUnderDarkness || this.parent.Map.skyManager.CurSkyGlow > 0.3);

        public event Action<Pawn> UseAction;

        public virtual void Use(Pawn worker)
        {
            if (!this.CanUseNow)
                Log.Error("Used while CanUseNow is false");
            int vision = Props.visionRangeProvide;
            int detection = Props.detectionRangeProvide;
            if (Props.AffectedByOperatorSightAbility && this.parent.Map.skyManager.CurSkyGlow <= 0.3)
                vision = detection = 0;
            if(Props.AffectedByOperatorSightAbility)
            {
                float sightFactor = worker.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
                vision = Mathf.RoundToInt(vision * sightFactor);
                detection = Mathf.RoundToInt(detection * sightFactor);
            }
            UseAction?.Invoke(worker);
            this.UpdateDetectionAbility(vision, detection);
        }

    }
}
