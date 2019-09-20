using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using PreemptiveStrike.Interceptor;
using Verse;
using PreemptiveStrike.DetectionSystem;

namespace PreemptiveStrike.IncidentCaravan
{
    static class IncidentCaravanUtility
    {
        public static bool AddNewIncidentCaravan(InterceptedIncident incident)
        {
            TravelingRaidingCaravan travalingCaravan = (TravelingRaidingCaravan)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("PES_RaidingCaravan", true));
            if (!TryFindTile(incident.parms.target.Tile, out int sourceTile))
            {
                Log.Error("Fail to create incident caravan: no available tile found");
                return false;
            }
            travalingCaravan.Tile = sourceTile;
            travalingCaravan.SetFaction(incident.parms.faction);
            travalingCaravan.destinationTile = incident.parms.target.Tile;

            int approxTileDist = Mathf.CeilToInt(Find.WorldGrid.ApproxDistanceInTiles(travalingCaravan.Tile, travalingCaravan.destinationTile));
            travalingCaravan.remainingTick = Mod.PES_Settings.TickForIncidentCaravanCoverOneTile * approxTileDist;
            travalingCaravan.incident = incident;
            Find.WorldObjects.Add(travalingCaravan);
            return true;
        }

        private static bool TryFindTile(int targetTile, out int tile)
        {
            //IntRange banditCampQuestSiteDistanceRange = SiteTuning.BanditCampQuestSiteDistanceRange;
            IntRange banditCampQuestSiteDistanceRange = new IntRange(6,7);
            return TileFinder.TryFindNewSiteTile(out tile, banditCampQuestSiteDistanceRange.min, banditCampQuestSiteDistanceRange.max, false, true, -1);
        }
    }
}
