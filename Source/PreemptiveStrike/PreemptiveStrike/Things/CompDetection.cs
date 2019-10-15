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
        public bool EnableDetection = true;

        public virtual CompProperties_Detection Props => props as CompProperties_Detection;

        protected CompPowerTrader compPowerTraderInt = null;
        protected CompPowerTrader compPowerTrader
        {
            get
            {
                if (compPowerTraderInt == null)
                    compPowerTraderInt = parent.GetComp<CompPowerTrader>();
                return compPowerTraderInt;
            }
        }

        public bool PowerOnOrDontNeedPower => compPowerTrader == null || compPowerTrader.PowerOn;

        protected virtual void UpdateDetectionAbility(int visionRange, int detectionRange)
        {
            if (!EnableDetection) return;
            int tile = parent.Map.Tile;
            int curTick = Find.TickManager.TicksGame;
            var dic = DetectDangerUtilities.DetectionAbilityInMapTile;
            if (!dic.TryGetValue(tile, out DetectionEffect oldEffect) || oldEffect.LastTick != curTick)
            {
                dic[tile] = new DetectionEffect(curTick, visionRange, detectionRange);
                return;
            }
            oldEffect.Vision = Math.Max(oldEffect.Vision, visionRange);
            oldEffect.Detection = Math.Max(oldEffect.Detection, detectionRange);
            dic[tile] = oldEffect;
        }

        public override string CompInspectStringExtra()
        {
            var upgradeList = parent.AllComps.OfType<CompUpgrade>();
            bool none = true;
            StringBuilder sb = new StringBuilder("PES_Building_Installed".Translate());
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

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (signal == "DisableDetection")
                EnableDetection = false;
            else if (signal == "EnableDetection")
                EnableDetection = true;
        }
    }
}
