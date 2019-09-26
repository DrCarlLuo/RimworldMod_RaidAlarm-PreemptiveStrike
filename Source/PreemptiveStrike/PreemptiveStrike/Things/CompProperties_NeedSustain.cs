using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection;

namespace PreemptiveStrike.Things
{
    class CompProperties_NeedSustain : CompProperties
    {
        public string needType;
        public float maxLevel;
        public bool instant;
        public bool consumeFuel = false;
        public float converseRatio = 0.05f;

        private NeedDef needDefInt = null;
        public NeedDef NeedType
        {
            get
            {
                if (needDefInt == null)
                    needDefInt = typeof(NeedDefOf).GetField(needType, BindingFlags.Static | BindingFlags.Public).GetValue(null) as NeedDef;
                return needDefInt;
            }
        }

        public CompProperties_NeedSustain()
        {
            compClass = typeof(CompNeedSustain);
        }
    }
}
