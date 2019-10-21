using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;
using PreemptiveStrike.Interceptor;

namespace PreemptiveStrike.Harmony.Compatibility
{
    class RimQuest
    {
        static Type RimQuest_HarmonyPatches = null;
        static MethodInfo RimQuest_HarmonyPatches_AddQuestGiverTwo = null;
        static bool Execute_RimQuest_HarmonyPatches_AddQuestGiverTwo = false;

        public static bool IsModLoaded()
        {
            RimQuest_HarmonyPatches = AccessTools.TypeByName("RimQuest.HarmonyPatches");
            return RimQuest_HarmonyPatches != null;
        }

        public static void DoPatch()
        {
            //Trade Caravans
            RimQuest_HarmonyPatches_AddQuestGiverTwo = AccessTools.Method(RimQuest_HarmonyPatches, "AddQuestGiverTwo");
            HarmonyMain.instance.Patch(RimQuest_HarmonyPatches_AddQuestGiverTwo, new HarmonyMethod(typeof(RimQuest).GetMethod("RimQuest_HarmonyPatches_AddQuestGiverTwo_PostFix")));
            HarmonyMain.instance.Patch(typeof(InterceptedIncident_HumanCrowd_Neutral).GetMethod("ExecuteNow"), null, new HarmonyMethod(typeof(RimQuest).GetMethod("PES_TradeCaravanPostFix")));
        }

        public static bool RimQuest_HarmonyPatches_AddQuestGiverTwo_PreFix()
        {
            return Execute_RimQuest_HarmonyPatches_AddQuestGiverTwo;
        }

        public static void PES_TradeCaravanPostFix(InterceptedIncident_HumanCrowd_Neutral __instance)
        {
            Log.Message("Doing this");
            Execute_RimQuest_HarmonyPatches_AddQuestGiverTwo = true;
            RimQuest_HarmonyPatches_AddQuestGiverTwo.Invoke(null, new object[] {null,null,null,null,__instance.pawnList});
            Execute_RimQuest_HarmonyPatches_AddQuestGiverTwo = false;
        }
    }
}
