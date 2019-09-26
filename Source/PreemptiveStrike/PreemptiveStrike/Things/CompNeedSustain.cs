using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Things
{
    class CompNeedSustain : ThingComp
    {
        public CompProperties_NeedSustain Props => props as CompProperties_NeedSustain;

        private CompRefuelable compRefuelableInt = null;
        public CompRefuelable compRefuelable
        {
            get
            {
                if (compRefuelableInt == null)
                {
                    compRefuelableInt = parent.TryGetComp<CompRefuelable>();
                }
                return compRefuelableInt;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            var comp = parent.TryGetComp<CompDetection_ManualDevice>();
            if (comp != null)
            {
                comp.UseAction += Sustains;
            }
        }

        public void Sustains(Pawn pawn)
        {
            var need = pawn.needs.TryGetNeed(Props.NeedType);
            if (need != null && need.CurLevel < Props.maxLevel)
            {
                float changeAmount;
                if (Props.instant)
                    changeAmount = Mathf.Min(Props.maxLevel, need.MaxLevel) - need.CurLevel;
                else
                    changeAmount = 0.02f;

                if (Mathf.FloorToInt(changeAmount * 100) == 0)
                    return;

                if (changeAmount > float.Epsilon && Props.consumeFuel && compRefuelable != null)
                {
                    if (Props.converseRatio > float.Epsilon)
                    {
                        changeAmount = Mathf.Min(changeAmount, compRefuelable.Fuel * Props.converseRatio);
                        compRefuelable.ConsumeFuel(changeAmount / Props.converseRatio);
                    }
                }
                need.CurLevel += changeAmount;
            }
        }
    }
}
