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

        public virtual bool CanUseNow
        {
            get
            {
                if (!(this.parent.Spawned && this.parent.Faction == Faction.OfPlayer))
                    return false;
                if(Props.NotUsableUnderDarkness && this.parent.Map.skyManager.CurSkyGlow <= 0.3)
                {
                    if (!useSearchlight || !PowerOnOrDontNeedPower)
                        return false;
                }
                if (!Props.UsableWithoutPower && !PowerOnOrDontNeedPower)
                    return false;
                return true;
            }
        }

        public event Action<Pawn> UseAction;

        private bool useTelescope = false;
        private bool useSearchlight = false;

        public virtual void Use(Pawn worker)
        {
            if (!this.CanUseNow)
                Log.Error("Used while CanUseNow is false");
            float vision = Props.visionRangeProvide;
            float detection = Props.detectionRangeProvide;

            //Light Level
            if (Props.NotUsableUnderDarkness && this.parent.Map.skyManager.CurSkyGlow <= 0.3)
            {
                if (useSearchlight && PowerOnOrDontNeedPower)
                {
                    vision *= 0.6f;
                    detection *= 0.6f;
                }
                else
                    vision = detection = 0f;
            }
            //Pawn Sight
            if (Props.AffectedByOperatorSightAbility)
            {
                float sightFactor = worker.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
                vision *= sightFactor;
                detection *= sightFactor;
            }
            //Power
            if(!PowerOnOrDontNeedPower)
            {
                vision *= 0.5f;
                detection *= 0.5f;
            }
            //Telescope
            if(useTelescope)
            {
                vision *= 1.5f;
                detection *= 1.5f;
            }
            //Weather
            var weather = parent.Map.weatherManager.curWeather;
            if(Props.AffectedByWeather)
            {
                float factor = 1f;
                if (weather == PESDefOf.Fog)
                    factor = 0.5f;
                if (weather == PESDefOf.Rain)
                    factor = 0.8f;
                if (weather == PESDefOf.FoggyRain)
                    factor = 0.4f;
                if (weather == PESDefOf.RainyThunderstorm)
                    factor = 0.7f;
                if (weather == PESDefOf.SnowHard)
                    factor = 0.9f;
                vision *= factor;
                detection *= factor;
            }
            UseAction?.Invoke(worker);
            this.UpdateDetectionAbility(Mathf.RoundToInt(vision), Mathf.RoundToInt(detection));
        }

        public override void CompTick()
        {
            base.CompTick();
            if(useSearchlight)
            {
                if (parent.Map.skyManager.CurSkyGlow <= 0.3)
                    compPowerTrader.PowerOutput = -1f * compPowerTrader.Props.basePowerConsumption - 500f;
                else
                    compPowerTrader.PowerOutput = -1f * compPowerTrader.Props.basePowerConsumption;
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            if(signal == CompUpgrade.CompleteEventNameRoot + "_Telescope")
                useTelescope = true;
            if (signal == CompUpgrade.CompleteEventNameRoot + "_NightVision")
                useSearchlight = true;
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder sb = new StringBuilder("");
            bool nothing = true;
            if (!useSearchlight && Props.NotUsableUnderDarkness && this.parent.Map.skyManager.CurSkyGlow <= 0.3)
            {
                nothing = false;
                sb.Append("PES_Building_UnUsableDark".Translate());
            }
            string baseStr = base.CompInspectStringExtra();
            if (baseStr != null)
            {
                if(!nothing)
                    sb.AppendLine();
                nothing = false;
                sb.Append(baseStr);
            }
            if (nothing) return null;
            return sb.ToString();
        }
    }
}
