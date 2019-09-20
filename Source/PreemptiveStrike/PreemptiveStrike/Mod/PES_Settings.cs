using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Mod
{
    static class PES_Settings
    {
        public static int WatchTowerVisionRange = 2;
        public static int WatchTowerDetectRange = 5;

        public static int DetectionChance = 50;

        public static int TickForIncidentCaravanCoverOneTile = 500; //default: 2500 for one hour

        public static bool DebugModeOn = true;
    }
}
