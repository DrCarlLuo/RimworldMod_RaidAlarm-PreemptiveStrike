using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Harmony.Compatibility
{
    class OtherModPatchMain
    {
        public static void ModCompatibilityPatches()
        {
            if(RimQuest.IsModLoaded())
            {
                Log.Message("PES: Try to patch RimQuest");
                RimQuest.DoPatch();
            }
        }
    }
}
