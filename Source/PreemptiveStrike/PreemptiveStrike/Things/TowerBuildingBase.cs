using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Things
{
    class TowerBuildingBase : Building
    {
        public virtual int PatrolPointCnt => 1;
        public virtual int PatrolIntervalTick => 10000000;
        public virtual int RotateIntervalTick => 300;

        public virtual IntVec3 GetPatrolPos(int x)
        {
            return InteractionCell;
        }

        public virtual Rot4 GetPatrolRot(int x)
        {
            return Rot4.Random;
        }
    }
}
