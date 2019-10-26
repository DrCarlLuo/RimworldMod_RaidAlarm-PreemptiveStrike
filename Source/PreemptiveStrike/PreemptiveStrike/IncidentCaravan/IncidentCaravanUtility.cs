using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using PreemptiveStrike.Interceptor;
using Verse;
using PreemptiveStrike.Mod;
using PreemptiveStrike.DetectionSystem;

namespace PreemptiveStrike.IncidentCaravan
{
    static class IncidentCaravanUtility
    {
        public static List<TravelingIncidentCaravan> IncidentCaravans = new List<TravelingIncidentCaravan>();

        public static IEnumerable<ICommunicable> GetAllCommunicableCaravan()
        {
            foreach(var x in IncidentCaravans)
            {
                if (x.Communicable && x.detected)
                    yield return x;
            }
            yield break;
        }

        public static bool AddNewIncidentCaravan(InterceptedIncident incident)
        {
            TravelingIncidentCaravan travalingCaravan = (TravelingIncidentCaravan)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("PES_RaidingCaravan", true));
            if (!TryFindTile(incident.parms.target.Tile, out int sourceTile))
            {
                Log.Error("Fail to create incident caravan: no available tile found");
                return false;
            }
            travalingCaravan.Tile = sourceTile;
            //travalingCaravan.SetFaction(incident.parms.faction);
            travalingCaravan.destinationTile = incident.parms.target.Tile;

            int approxTileDist = Mathf.CeilToInt(Find.WorldGrid.ApproxDistanceInTiles(travalingCaravan.Tile, travalingCaravan.destinationTile));
            travalingCaravan.remainingTick = Mod.PES_Settings.TickForIncidentCaravanCoverOneTile * approxTileDist;
            travalingCaravan.incident = incident;
            incident.parentCaravan = travalingCaravan;
            Find.WorldObjects.Add(travalingCaravan);
            return true;
        }

        public static bool AddSimpleIncidentCaravan(InterceptedIncident incident, int DelayTick, int revealTick, bool InitialDetected = false)
        {
            TravelingIncidentCaravan_Simple travalingCaravan = (TravelingIncidentCaravan_Simple)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("PES_RaidingCaravan_Simple", true));
            travalingCaravan.Tile = 0;
            travalingCaravan.remainingTick = DelayTick;
            travalingCaravan.RemainingRevealTick = revealTick;
            travalingCaravan.incident = incident;
            travalingCaravan.detected = false;
            incident.parentCaravan = travalingCaravan;
            if (InitialDetected) travalingCaravan.detected = true;
            Find.WorldObjects.Add(travalingCaravan);
            return true;
        }

        private static bool TryFindTile(int targetTile, out int tile)
        {
            //IntRange banditCampQuestSiteDistanceRange = SiteTuning.BanditCampQuestSiteDistanceRange;
            int detectionRange = DetectDangerUtilities.GetDetectionRangeOfMap(targetTile);
            detectionRange = Math.Max(detectionRange, DetectDangerUtilities.GetVisionRangeOfMap(targetTile));
            detectionRange = Math.Max(detectionRange, 6);
            IntRange banditCampQuestSiteDistanceRange = new IntRange(detectionRange,detectionRange + 5);
            if(!TileFinder.TryFindNewSiteTile(out tile, banditCampQuestSiteDistanceRange.min, banditCampQuestSiteDistanceRange.max, false, true, -1))
            {
                return ForceFindTile_Dfs(targetTile, targetTile, detectionRange + 1, out tile);
            }
            return true;
        }

        private static bool ForceFindTile_Dfs(int curTile, int orgTile, int wantDist, out int res)
        {
            int thisDist = Mathf.CeilToInt(Find.WorldGrid.ApproxDistanceInTiles(curTile, orgTile));
            if(thisDist >= wantDist)
            {
                res = curTile;
                return true;
            }
            List<int> choices = new List<int>();
            List<int> neighbors = new List<int>();
            Find.WorldGrid.GetTileNeighbors(curTile, neighbors);
            foreach(var x in neighbors)
            {
                int newDist = Mathf.CeilToInt(Find.WorldGrid.ApproxDistanceInTiles(x, orgTile));
                if (newDist > thisDist)
                    choices.Add(x);
            }
            if (choices.Count <= 0)
            {
                res = -1;
                return false;
            }
            return ForceFindTile_Dfs(choices.RandomElement(), orgTile, wantDist, out res);
        }
    }
}
