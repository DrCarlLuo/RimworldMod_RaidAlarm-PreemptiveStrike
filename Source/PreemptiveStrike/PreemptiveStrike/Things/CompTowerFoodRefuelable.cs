using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Harmony;
using UnityEngine;
using System.Reflection;
using Verse;

namespace PreemptiveStrike.Things
{
    class CompTowerFoodRefuelable : CompRefuelable, IStoreSettingsParent
    {
        public new CompProperties_TowerFoodRefuelable Props => props as CompProperties_TowerFoodRefuelable;

        public bool Updated = false;

        public StorageSettings storageSettings;
        public ThingFilter FuelFilter => storageSettings.filter;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (storageSettings == null)
            {
                storageSettings = new StorageSettings(this);
                if (Props.DefaultStorageSettings != null)
                    storageSettings.CopyFrom(Props.DefaultStorageSettings);
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            if (signal == CompUpgrade.CompleteEventNameRoot + "_RawFoodAllow")
                Updated = true;
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (storageSettings == null)
            {
                storageSettings = new StorageSettings(this);
                if (Props.DefaultStorageSettings != null)
                    storageSettings.CopyFrom(Props.DefaultStorageSettings);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref storageSettings, "storageSettings");
        }

        public bool StorageTabVisible => true;

        public StorageSettings GetParentStoreSettings()
        {
            if (Updated)
                return Props.UpdatedStorageSettings;
            return Props.FixedStorageSettings;
        }

        public StorageSettings GetStoreSettings() => storageSettings;
    }

    [HarmonyPatch(typeof(CompRefuelable), "Refuel", new Type[] { typeof(List<Thing>) })]
    class CompRefuelable_Refule_p1
    {
        [HarmonyPrefix]
        public static bool Prefix(CompRefuelable __instance, List<Thing> fuelThings)
        {
            if (__instance is CompTowerFoodRefuelable)
            {
                if (__instance.Props.atomicFueling)
                {
                    Log.Error("Refuel in Tower shouldn't be atomic");
                }
                float remains = (__instance.TargetFuelLevel - __instance.Fuel) / __instance.Props.FuelMultiplierCurrentDifficulty;
                while (remains > float.Epsilon && fuelThings.Count > 0)
                {
                    Thing thing = fuelThings.Pop<Thing>();
                    float nutrition = FoodUtility.GetNutrition(thing, thing.def);
                    if (nutrition < float.Epsilon) continue;
                    int curNum = Mathf.CeilToInt(remains / nutrition);
                    curNum = Math.Min(curNum, thing.stackCount);
                    ForceAddFuel(__instance, curNum * nutrition);
                    thing.SplitOff(curNum).Destroy(DestroyMode.Vanish);
                    remains -= curNum * nutrition;
                }
                return false;
            }
            return true;
        }

        public static void ForceAddFuel(CompRefuelable __instance, float value)
        {
            var fuelField = typeof(CompRefuelable).GetField("fuel", BindingFlags.NonPublic | BindingFlags.Instance);
            fuelField.SetValue(__instance, (float)fuelField.GetValue(__instance) + value * __instance.Props.FuelMultiplierCurrentDifficulty);
            __instance.parent.BroadcastCompSignal("Refueled");
        }
    }
}
