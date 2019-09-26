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
    static class UpgradeUtilities
    {
        public static void ToggleDesignation(this Thing thing, DesignationDef def, bool enable)
        {
            if (thing.Map == null || thing.Map.designationManager == null) throw new Exception("Thing must belong to a map to toggle designations on it");
            var des = thing.Map.designationManager.DesignationOn(thing, def);
            if (enable && des == null)
            {
                thing.Map.designationManager.AddDesignation(new Designation(thing, def));
            }
            else if (!enable && des != null)
            {
                thing.Map.designationManager.RemoveDesignation(des);
            }
        }

        public static void ForceAddComp(this ThingWithComps thing, ThingComp comp, CompProperties prop)
        {
            var field = typeof(ThingWithComps).GetField("comps", BindingFlags.NonPublic | BindingFlags.Instance);
            List<ThingComp> list = field.GetValue(thing) as List<ThingComp>;
            list.Add(comp);
            field.SetValue(thing, list);
            comp.parent = thing;

            //Need to manually resolve references (Any better solution here???)
            if(prop is CompProperties_Refuelable fuck)
            {
                var filter = fuck.fuelFilter;
                filter.ResolveReferences();
            }

            comp.Initialize(prop);
        }
    }
}
