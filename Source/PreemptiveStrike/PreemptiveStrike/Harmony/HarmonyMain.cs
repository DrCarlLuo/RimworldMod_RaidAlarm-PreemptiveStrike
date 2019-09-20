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
        }
    }
}
