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
    class CompUpgrade : ThingComp, IThingHolder
    {
        public CompProperties_Upgrade Props => this.props as CompProperties_Upgrade;

        public static readonly string CompleteEventNameRoot = "CompleteUpgrade";

        public bool complete = false;
        public float workAccum = 0;
        public bool beginUpgrade = false;
        public ThingOwner ingredients;

        public CompUpgrade()
        {
            this.ingredients = new ThingOwner<Thing>(this);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe.EnterNode("CompUpgrade_" + Props.upgradeTypeName);
            Scribe_Values.Look(ref complete, "complete", false, false);
            if (!complete)
            {
                Scribe_Values.Look(ref workAccum, "workAccum", 0f, false);
                Scribe_Values.Look(ref beginUpgrade, "beginUpgrade", false, false);
                Scribe_Deep.Look(ref this.ingredients, "ingredients");
            }
            if (ingredients == null)
                ingredients = new ThingOwner<Thing>(this);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                //Use this to apply update in save-loading
                //so that other comps does not need to save any data
                if (complete)
                    PostUpgrade(true);
            }
            Scribe.ExitNode();
        }

        private bool AllPrerequisitesMeet
            => (Props.needResearch == null || Props.needResearch.IsFinished) &&
               (Props.needUpgradeType == null);

        public float FinishPercentage => Mathf.Clamp01(this.workAccum / Math.Max(Props.workAmount, 1));

        private void UpdateDesignation()
        {
            if (this.parent.Map != null)
            {
                parent.ToggleDesignation(PESDefOf.PES_InstallUpgrade, parent.AllComps.OfType<CompUpgrade>().Any((CompUpgrade comp) => comp.beginUpgrade));
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            UpdateDesignation();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            ingredients.TryDropAll(parent.Position, map, ThingPlaceMode.Near);
        }

        public void Work(Pawn pawn, float workAmount)
        {
            workAccum += workAmount;
            if (workAccum >= Props.workAmount)
                CompleteUpgrade();

        }

        private void CompleteUpgrade()
        {
            beginUpgrade = false;
            if (!complete)
            {
                complete = true;
                ingredients.ClearAndDestroyContents(DestroyMode.Vanish);
                UpdateDesignation();
                PostUpgrade(false);
            }
        }

        private void PostUpgrade(bool exposeData)
        {
            parent.BroadcastCompSignal(CompleteEventNameRoot + "_" + Props.upgradeTypeName);
            if (Props.upgradeCompProp != null && Props.upgradeCompProp.Count > 0)
            {
                foreach (var prop in Props.upgradeCompProp)
                {
                    var comp = Activator.CreateInstance(prop.compClass) as ThingComp;
                    prop.ResolveReferences(parent.def);
                    parent.ForceAddComp(comp, prop);
                    if (exposeData)
                        comp.PostExposeData();
                }
            }
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings() => ingredients;

        public ThingDefCount TryGetOneMissingMaterial()
        {
            if (beginUpgrade)
            {
                foreach (var needThing in Props.costList)
                {
                    int remainingNum = needThing.count;
                    foreach (var existThing in ingredients)
                    {
                        if (existThing.def == needThing.thingDef)
                            remainingNum -= existThing.stackCount;
                    }
                    if (remainingNum > 0)
                        return new ThingDefCount(needThing.thingDef, remainingNum);
                }
            }
            return default;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!complete && AllPrerequisitesMeet)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "PES_Building_ToInstall".Translate(Props.TruncatedName),
                    defaultDesc = Props.DescriptionOnGizmo,
                    toggleAction = delegate ()
                    {
                        if (!beginUpgrade && !AllPawnInColonyMeetSkillRequirement)
                        {
                            Messages.Message("PES_Update_NeedSkill".Translate(Props.needConstructionSkill), this.parent, MessageTypeDefOf.RejectInput, false);
                        }
                        else
                        {
                            beginUpgrade = !beginUpgrade;
                            if (!beginUpgrade)
                                ingredients.TryDropAll(parent.Position, parent.Map, ThingPlaceMode.Near);
                            UpdateDesignation();
                        }
                    },
                    isActive = () => beginUpgrade,
                    icon = ContentFinder<Texture2D>.Get(Props.GizmoTexPath)
                };
            }
            yield break;
        }

        public bool PawnMeetsSkillRequirement(Pawn pawn) => pawn.skills.GetSkill(SkillDefOf.Construction).Level >= Props.needConstructionSkill;

        public bool AllPawnInColonyMeetSkillRequirement => !parent.Map.mapPawns.FreeColonists.All((Pawn pawn) => !PawnMeetsSkillRequirement(pawn));

        public override string CompInspectStringExtra()
        {
            StringBuilder sb = new StringBuilder("");
            if (beginUpgrade)
            {
                sb.AppendLine("PES_Building_UpdateInstalling".Translate(Props.name, Mathf.RoundToInt(FinishPercentage * 100f)));
                sb.Append("PES_Building_Delivered".Translate(ingredients.ContentsString));
            }
            return sb.ToString();
        }

    }
}
