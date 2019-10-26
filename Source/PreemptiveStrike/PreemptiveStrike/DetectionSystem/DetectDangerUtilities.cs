using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Mod;


namespace PreemptiveStrike.DetectionSystem
{
    struct DetectionEffect
    {
        public int LastTick;
        public int Vision;
        public int Detection;

        public DetectionEffect(int lastTick, int vision, int detection)
        {
            LastTick = lastTick;
            Vision = vision;
            Detection = detection;
        }
    }

    [StaticConstructorOnStartup]
    static class DetectDangerUtilities
    {
        public static Dictionary<int, DetectionEffect> DetectionAbilityInMapTile;
        public static int LastSolarFlareDetectorTick = -1;

        public static bool DitectionOddsOfCaravan(TravelingIncidentCaravan caravan, out float odds)
        {
            int targetTile = caravan.incident.parms.target.Tile;
            int remainingTiles = Mathf.CeilToInt(ApproxTileNumBetweenCaravanTarget(caravan));
            int curDetectionRange = GetDetectionRangeOfMap(targetTile);
            int curVisionRange = GetVisionRangeOfMap(targetTile);
            if(remainingTiles > curDetectionRange || curDetectionRange == curVisionRange)
            {
                odds = 0;
                return false;
            }
            float C = curDetectionRange - curVisionRange;
            float y = remainingTiles - curVisionRange;
            odds = (C - y) / C;
            odds *= PES_Settings.DetectionCoefficient;
            return true;
        }

        public static bool TryDetectIncidentCaravan(TravelingIncidentCaravan caravan)
        {
            float odds = 0;
            if (!DitectionOddsOfCaravan(caravan, out odds))
                return false;
            odds = Mathf.Clamp(odds, 0.1f, 0.8f);
            if (new FloatRange(0f,1f).RandomInRange <= odds)
            {
                if (PES_Settings.DebugModeOn)
                    Log.Message("Try Detect with odds "+ odds +" : Success");
                return true;
            }
            if (PES_Settings.DebugModeOn)
                Log.Message("Try Detect with odds " + odds + " : Fail");
            return false;
        }

        public static bool TryDetectIncidentCaravanDetail(TravelingIncidentCaravan caravan)
        {
            float odds = 0;
            if (!DitectionOddsOfCaravan(caravan, out odds))
                return false;
            odds *= 2;
            odds = Mathf.Clamp(odds, 0.2f, 0.95f);
            if (new FloatRange(0f, 1f).RandomInRange <= odds)
            {
                if (PES_Settings.DebugModeOn)
                    Log.Message("Try Detect Detail with odds " + odds + " : Success");
                return true;
            }
            if (PES_Settings.DebugModeOn)
                Log.Message("Try Detect Detail with odds " + odds + " : Fail");
            return false;
        }

        public static bool TryConfirmCaravanWithinVision(TravelingIncidentCaravan caravan)
        {
            int targetTile = caravan.incident.parms.target.Tile;
            int remainingTiles = Mathf.CeilToInt(ApproxTileNumBetweenCaravanTarget(caravan));
            int visionRange = GetVisionRangeOfMap(targetTile);
            if (visionRange == 0)
                return false; //if the colony has no vision, then dont do it at all
            if (remainingTiles <= visionRange)
            {
                if (PES_Settings.DebugModeOn)
                    Log.Message("Caravan enter vision range");
                return true;
            }
            return false;
        }

        public static int GetDetectionRangeOfMap(int MapTile)
        {
            //Log.Message("a" + DetectionAbilityInMapTile[MapTile].LastTick + "??" + Find.TickManager.TicksGame);
            if (DetectionAbilityInMapTile.TryGetValue(MapTile, out DetectionEffect effect) && effect.LastTick == Find.TickManager.TicksGame)
                return effect.Detection;
            return 0;
        }

        public static int GetVisionRangeOfMap(int MapTile)
        {
            if (DetectionAbilityInMapTile.TryGetValue(MapTile, out DetectionEffect effect) && effect.LastTick == Find.TickManager.TicksGame)
                return effect.Vision;
            return 0;
        }

        public static float ApproxTileNumBetweenCaravanTarget(TravelingIncidentCaravan caravan)
        {
            //TODO: Is this a little costly???
            return Find.WorldGrid.ApproxDistanceInTiles(caravan.Tile, caravan.incident.parms.target.Tile);
            //Vector3 TargetCenter = Find.WorldGrid.GetTileCenter(caravan.incident.parms.target.Tile);
            //return Find.WorldGrid.ApproxDistanceInTiles(GenMath.SphericalDistance(caravan.curPos.normalized, TargetCenter.normalized));
        }

        static DetectDangerUtilities()
        {
            DetectionAbilityInMapTile = new Dictionary<int, DetectionEffect>();
        }
    }
}
