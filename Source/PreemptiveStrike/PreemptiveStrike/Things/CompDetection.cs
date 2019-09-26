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
    abstract class CompDetection : ThingComp
    {
        protected virtual void UpdateDetectionAbility(int visionRange, int detectionRange)
        {
            int tile = parent.Map.Tile;
            int curTick = Find.TickManager.TicksGame;
            var dic = DetectDangerUtilities.DetectionAbilityInMapTile;
            if (!dic.TryGetValue(tile, out DetectionEffect oldEffect) || oldEffect.LastTick < curTick)
            {
                dic[tile] = new DetectionEffect(curTick, visionRange, detectionRange);
                return;
            }
            oldEffect.Vision = Math.Max(oldEffect.Vision, visionRange);
            oldEffect.Detection = Math.Max(oldEffect.Detection, detectionRange);
        }

        public override string CompInspectStringExtra()
        {
            var upgradeList = parent.AllComps.OfType<CompUpgrade>();
            bool none = true;
            StringBuilder sb = new StringBuilder("Installed Upgrade: ");
            foreach (var comp in upgradeList)
            {
                if(comp.complete)
                {
                    if (none)
                        none = false;
                    else
                        sb.Append(", ");
                    sb.Append(comp.Props.name);
                }
            }
            if (none) return null;
            return sb.ToString();
        }
    }
}
