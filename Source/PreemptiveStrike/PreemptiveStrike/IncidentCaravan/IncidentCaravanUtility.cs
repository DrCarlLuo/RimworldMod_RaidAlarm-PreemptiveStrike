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
        public static List<TravelingIncidentCaravan> IncidentCaravans = new List<TravelingIncidentCaravan>();

        public static IEnumerable<ICommunicable> GetAllCommunicableCaravan()
        {
            foreach(var x in IncidentCaravans)
            {
                if (x.Communicable)
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
            incident.parentCaravan = travalingCaravan;
            if (InitialDetected) travalingCaravan.detected = true;
            Find.WorldObjects.Add(travalingCaravan);
            return true;
        }

        private static bool TryFindTile(int targetTile, out int tile)
        {
            //IntRange banditCampQuestSiteDistanceRange = SiteTuning.BanditCampQuestSiteDistanceRange;
            IntRange banditCampQuestSiteDistanceRange = new IntRange(10,11);
            return TileFinder.TryFindNewSiteTile(out tile, banditCampQuestSiteDistanceRange.min, banditCampQuestSiteDistanceRange.max, false, true, -1);
        }
    }
}
