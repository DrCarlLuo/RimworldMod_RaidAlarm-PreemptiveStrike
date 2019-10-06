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

        //Make contact chances
        public static float BasePersuadeChance_Friendly = 0.3f;
        public static float BasePersuadeChance_Hostile = 0.1f;

        public static float BaseIntimidationFrightenChance_Friendly = 0.21f;
        public static float BaseIntimidationContactChance_Friendly = 0.45f;
        public static float BaseIntimidationFrightenChance_Hostile = 0.11f;
        public static float BaseIntimidationContactChance_Hostile = 0.4f;

        public static float BaseBeguilementFrightenChance_Friendly = 0.9f;
        public static float BaseBeguilementFrightenChance_Hostile = 0.25f;
        public static float BaseBeguilementContactChance_Hostile = 0.25f;

        //Delay raid chances
        public static float BaseDelayPersuasionChance = 0.2f;
        public static float BaseDelayIntimidationSuccessChance = 0.3f;
        public static float BaseDelayIntimidationSmiteChance = 0.3f;
        public static float BaseDelayBeguilementChance = 0.4f;

        public static float BaseBargainPersuasionChance = 0.3f;
        public static float BaseBargainIntimidationSuccessChance = 0.4f;
        public static float BaseBargainIntimidationSmiteChance = 0.4f;
        public static float BaseBargainBeguilementChance = 0.5f;

        public static float BaseRemedyPersuasionChance = 0.3f;
        public static float BaseRemedyIntimidationSuccessChance = 0.4f;
        public static float BaseRemedyIntimidationSmiteChance = 0.4f;
        public static float BaseRemedyBeguilementChance = 0.5f;

        public static bool DebugModeOn = true;
    }
}
