using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.RaidGoal;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.Things
{
    class ModernWatchTower : Building
    {
        public override void Draw()
        {
            base.Draw();
            Vector3 drawPos = DrawPos;
            drawPos.y += 5.905f;
            //drawPos.z += 1.1f;
            //drawPos.x += 0.5f;
            Quaternion quaternion = Quaternion.identity;
            Color white = Color.white;
            Material material = Textures.FenceGraphics.MatAt(base.Rotation, null);
            if (white != material.color)
            {
                material = MaterialPool.MatFrom((Texture2D)material.mainTexture, ShaderDatabase.Transparent, white);
            }
            //Graphic coloredVersion = GraphicsCache.BathOver.GetColoredVersion(base.Graphic.Shader, this.DrawColor, this.DrawColorTwo);
            //Graphics.DrawMesh(coloredVersion.MeshAt(base.Rotation), drawPos, quaternion, coloredVersion.MatAt(base.Rotation, null), 0);
            //drawPos.y -= 0.01f;
            Graphics.DrawMesh(Textures.FenceGraphics.MeshAt(base.Rotation), drawPos, quaternion, material, 0);
        }
    }
}
