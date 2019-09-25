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
    class CompUpgrade : ThingComp, IThingHolder
    {
        public CompProperties_Upgrade Props => this.props as CompProperties_Upgrade;

        public static readonly string CompleteEventNameRoot = "CompleteUpgrade";

        public bool complete = false;
        public int workAccum = 0;
        public bool beginUpgrade = false;
        public ThingOwner ingredients;

        public CompUpgrade()
        {
            this.ingredients = new ThingOwner<Thing>(this);
        }

        private bool AllPrerequisitesMeet 
            => (Props.needResearch == null || Props.needResearch.IsFinished) &&
               (Props.needUpgradeType == null)

        public float FinishPercentage => Mathf.Clamp01(this.workAccum * 1f / Math.Max(Props.workAmount, 1));

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            ingredients.TryDropAll(parent.Position, map, ThingPlaceMode.Near);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings() => ingredients;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if(!complete && AllPrerequisitesMeet)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = Props.name,
                    defaultDesc = Props.describe,
                    toggleAction = delegate ()
                    {
                        Log.Message("Pretend to be upgrading");
                    },
                    isActive = () => beginUpgrade
                };
            }
            yield break;
        }

    }
}
