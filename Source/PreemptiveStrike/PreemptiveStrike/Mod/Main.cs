using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Mod
{
    [StaticConstructorOnStartup]
    class PES : Verse.Mod
    {
        public static PES Instance { get; private set; }

        public PES(ModContentPack content): base(content)
        {
            GetSettings<PES_Settings>();
            Instance = this;
        }

        public override string SettingsCategory()
        {
            return "Pre-emptive Strike";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            PES_Settings.DoSettingsWindowContents(inRect);
        }
    }
}
