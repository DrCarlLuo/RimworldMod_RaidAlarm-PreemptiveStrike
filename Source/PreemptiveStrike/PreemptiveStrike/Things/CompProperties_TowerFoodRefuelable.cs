using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Things
{
    class CompProperties_TowerFoodRefuelable : CompProperties_Refuelable
    {
        public StorageSettings DefaultStorageSettings;
        public StorageSettings FixedStorageSettings;
        public StorageSettings UpdatedStorageSettings;

        public CompProperties_TowerFoodRefuelable()
        {
            compClass = typeof(CompTowerFoodRefuelable);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (this.FixedStorageSettings != null)
            {
                this.FixedStorageSettings.filter.ResolveReferences();
            }
            if (this.DefaultStorageSettings == null && this.FixedStorageSettings != null)
            {
                this.DefaultStorageSettings = new StorageSettings();
                this.DefaultStorageSettings.CopyFrom(this.FixedStorageSettings);
            }
            if (this.DefaultStorageSettings != null)
            {
                this.DefaultStorageSettings.filter.ResolveReferences();
            }
            if (this.UpdatedStorageSettings != null)
                this.UpdatedStorageSettings.filter.ResolveReferences();
        }
    }
}
