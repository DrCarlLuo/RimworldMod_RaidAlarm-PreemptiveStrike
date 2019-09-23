using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.UI
{
    class PES_UIComponent : GameComponent
    {
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Find.WindowStack.Add(ColonySecurityDashBoard_Window.Instance);
        }

        public PES_UIComponent(Game game)
        {

        }
    }
}
