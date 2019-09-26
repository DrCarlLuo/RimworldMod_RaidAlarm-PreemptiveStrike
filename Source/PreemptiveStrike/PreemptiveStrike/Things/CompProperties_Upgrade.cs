using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

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
        public string description;

        public string upgradeTypeName;
        public UpgradeType upgradeType => (UpgradeType)Enum.Parse(typeof(UpgradeType), upgradeTypeName);

        public int workAmount;
        public List<ThingDefCountClass> costList = new List<ThingDefCountClass>();

        public ResearchProjectDef needResearch;
        public int needConstructionSkill;

        public string needUpgradeType;
        public UpgradeType NeedUpgradeType => (UpgradeType)Enum.Parse(typeof(UpgradeType), needUpgradeType);

        public List<CompProperties> upgradeCompProp;

        public string GizmoTexPath;

        public CompProperties_Upgrade()
        {
            compClass = typeof(CompUpgrade);
        }

        private string descriptionOnGizmo;
        public string DescriptionOnGizmo
        {
            get
            {
                if (descriptionOnGizmo == null)
                {
                    string.Format("<b>{0}</b>\n{1}\n\n{2}{3}",
                        name,
                        description,
                        MaterialDescription,
                        needConstructionSkill == 0 ? "" : skillRequirementDescription
                        );
                }
                return descriptionOnGizmo;
            }
        }

        private string materialDescription;
        public string MaterialDescription
        {
            get
            {
                if (materialDescription == null)
                {
                    StringBuilder sb = new StringBuilder("");
                    foreach (var tc in costList)
                    {
                        sb.Append(tc.thingDef.label);
                        sb.Append(" x");
                        sb.Append(tc.count);
                    }
                    materialDescription = sb.ToString();
                }
                return materialDescription;
            }
        }

        private string skillRequirementDescription;
        public string SkillRequirementDescription
        {
            get
            {
                if (skillRequirementDescription == null)
                {
                    skillRequirementDescription = string.Format("Construction Needed:{0}", needConstructionSkill);
                }
                return skillRequirementDescription;
            }
        }
    }
}
