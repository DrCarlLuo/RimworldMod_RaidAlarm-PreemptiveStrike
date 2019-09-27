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
    class CompBuildingAlarm_Primitive : CompBuildingAlarm
    {
        public CompProperties_BuildingAlarm_Primitive Props => props as CompProperties_BuildingAlarm_Primitive;

        public override void PostDraw()
        {
            base.PostDraw();
            if(Alarming)
            {
                Vector3 drawPos = this.parent.DrawPos;
                drawPos.x += Props.DrawOffset.x;
                drawPos.y += Props.DrawOffset.y;
                drawPos.z += Props.DrawOffset.z;
                CompFireOverlay.FireGraphic.Draw(drawPos, Rot4.North, this.parent, 0f);
            }
        }
    }
}
