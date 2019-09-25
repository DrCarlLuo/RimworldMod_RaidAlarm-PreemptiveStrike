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
        public static int PrimitiveWatchTowerVisionRange = 1;
        public static int PrimitiveWatchTowerDetectRange = 3;

        public static int WatchTowerVisionRange = 2;
        public static int WatchTowerDetectionRange = 6;

        public static int SentryArrayVisionRange = 6;
        public static int SentryArrayDetectionRange = 0;

        public static int RadarVisionRange = 0;
        public static int RadarDetectionRange = 10;

        public static int SpySatelliteVisionRange = 0;
        public static int SpySatelliteDetectionRange = 18;

        public static int DetectionChance = 50;

        public static int TickForIncidentCaravanCoverOneTile = 500; //default: 2500 for one hour

        public static bool DebugModeOn = true;
    }
}
