using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Harmony
{
    [StaticConstructorOnStartup]
    class HarmonyMain
    {
        static HarmonyInstance instance;

        static HarmonyMain()
        {
            instance = HarmonyInstance.Create("DrCarlLuo.Rimworld.PreemptiveStrike");
            instance.PatchAll(Assembly.GetExecutingAssembly());
            ManualPatchings();
        }

        static void ManualPatchings()
        {
            //This alphabeaver one is f**king special
            //Why it has to be an INTERNAL CLASS, WHYYYYYYYYYYYYYYYYYYYYYY?????
            var prefix = typeof(Patch_IncidentWorker_Alphabeavers_TryExecuteWorker).GetMethod("Prefix");
            instance.Patch(AccessTools.Method(AccessTools.TypeByName("RimWorld.IncidentWorker_Alphabeavers"), "TryExecuteWorker"), new HarmonyMethod(prefix));

        }
    }
}
