using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using PreemptiveStrike.Interceptor;
using Verse;

namespace PreemptiveStrike.IncidentCaravan
{
    static class IncidentCaravanUtility
    {
        public static bool AddNewIncidentCaravan(InterceptedIncident incident)
        {
            TravelingRaidingCaravan travalingCaravan = (TravelingRaidingCaravan)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("PES_RaidingCaravan", true));
            if (!TryFindTile(out int sourceTile))
                return false;
            travalingCaravan.Tile = sourceTile;
            travalingCaravan.SetFaction(incident.parms.faction);
            travalingCaravan.destinationTile = incident.parms.target.Tile;
            travalingCaravan.remainingTick = 2500;
            travalingCaravan.incident = incident;
            Find.WorldObjects.Add(travalingCaravan);
            return true;
        }

        private static bool TryFindTile(out int tile)
        {
            //IntRange banditCampQuestSiteDistanceRange = SiteTuning.BanditCampQuestSiteDistanceRange;
            IntRange banditCampQuestSiteDistanceRange = new IntRange(7, 8);
            return TileFinder.TryFindNewSiteTile(out tile, banditCampQuestSiteDistanceRange.min, banditCampQuestSiteDistanceRange.max, false, true, -1);
        }
    }
}
