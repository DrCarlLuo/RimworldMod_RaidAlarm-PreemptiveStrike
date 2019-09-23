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

    [StaticConstructorOnStartup]
    static class DetectDangerUtilities
    {
        public static Dictionary<int, int> LastWatchTowerUsedTickInMapTile;

        public static bool TryDetectIncidentCaravan(TravelingIncidentCaravan caravan)
        {
            int targetTile = caravan.incident.parms.target.Tile;
            int remainingTiles = Mathf.CeilToInt(ApproxTileNumBetweenCaravanTarget(caravan));
            if (remainingTiles <= GetDetectionRangeOfMap(targetTile))
            {
                if (new IntRange(1, 100).RandomInRange <= PES_Settings.DetectionChance)
                {
                    Messages.Message("Caravan Detected", MessageTypeDefOf.NegativeEvent);
                    return true;
                }
            }
            return false;
        }

        public static bool TryDetectCaravanDetail(TravelingIncidentCaravan caravan)
        {
            if(caravan.detected)
            {
                if (new IntRange(1, 100).RandomInRange <= PES_Settings.DetectionChance)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TryConfirmCaravanWithinVision(TravelingIncidentCaravan caravan)
        {
            int targetTile = caravan.incident.parms.target.Tile;
            int remainingTiles = Mathf.CeilToInt(ApproxTileNumBetweenCaravanTarget(caravan));
            if (remainingTiles <= GetVisionRangeOfMap(targetTile))
            {
                Messages.Message("Caravan Spotted!", MessageTypeDefOf.NegativeEvent);
                return true;
            }
            return false;
        }

        public static int GetDetectionRangeOfMap(int MapTile)
        {
            int res = -1;
            if (LastWatchTowerUsedTickInMapTile.TryGetValue(MapTile, out int lastTick))
            {
                if (lastTick == Find.TickManager.TicksGame)
                    res = Math.Max(res, PES_Settings.WatchTowerDetectRange);
            }
            return res;
        }

        public static int GetVisionRangeOfMap(int MapTile)
        {
            int res = -1;
            if (LastWatchTowerUsedTickInMapTile.TryGetValue(MapTile, out int lastTick))
            {
                if (lastTick == Find.TickManager.TicksGame)
                    res = Math.Max(res, PES_Settings.WatchTowerVisionRange);
            }
            return res;
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
            LastWatchTowerUsedTickInMapTile = new Dictionary<int, int>();
        }
    }
}
