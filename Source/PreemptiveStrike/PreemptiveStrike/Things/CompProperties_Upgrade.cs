using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.DetectionSystem;

namespace PreemptiveStrike.Things
{
    enum UpgradeType
    {
        RemovePowerNeed,
        FoodSustain,
        RecreationSustain,
        Telescope,
        NightVision
    }


    class CompProperties_Upgrade : CompProperties
    {
        public string name;
        public string describe;

        public string upgradeTypeName;
        public UpgradeType upgrade => (UpgradeType)Enum.Parse(typeof(UpgradeType), upgradeTypeName);

        public int workAmount;
        public List<ThingDefCountClass> costList = new List<ThingDefCountClass>();

        public ResearchProjectDef needResearch;
        public int needConstructionSkill;

        public string needUpgradeType;
        public UpgradeType NeedUpgradeType => (UpgradeType)Enum.Parse(typeof(UpgradeType), needUpgradeType);

        public CompProperties_Upgrade()
        {
            compClass = typeof(CompUpgrade);
        }
    }
}
