using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using RimWorld;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.UI;
using RimWorld.Planet;
using Verse;

namespace PreemptiveStrike.Harmony
{
    //Do these patches because the worldobjects are stored in save along with worldobjectsholder

    [HarmonyPatch(typeof(WorldObjectsHolder), "AddToCache")]
    static class Patch_WorldObjectsHolder_AddToCache
    {
        [HarmonyPostfix]
        static void Postfix(WorldObjectsHolder __instance, WorldObject o)
        {
            if (o is TravelingIncidentCaravan)
            {
                IncidentCaravanUtility.IncidentCaravans.Add((TravelingIncidentCaravan)o);
                EventManger.NotifyCaravanListChange?.Invoke();
            }
        }
    }

    [HarmonyPatch(typeof(WorldObjectsHolder), "RemoveFromCache")]
    static class Patch_WorldObjectsHolder_RemoveFromCache
    {
        [HarmonyPostfix]
        static void Postfix(WorldObjectsHolder __instance, WorldObject o)
        {
            if (o is TravelingIncidentCaravan)
            {
                IncidentCaravanUtility.IncidentCaravans.Remove((TravelingIncidentCaravan)o);
                EventManger.NotifyCaravanListChange?.Invoke();
            }
        }
    }

    [HarmonyPatch(typeof(WorldObjectsHolder), "Recache")]
    static class Patch_WorldObjectsHolder_Recache
    {
        [HarmonyPrefix]
        static void Prefix(WorldObjectsHolder __instance)
        {
            IncidentCaravanUtility.IncidentCaravans.Clear();
            EventManger.NotifyCaravanListChange?.Invoke();
        }
    }

}
