using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using PreemptiveStrike.Interceptor;
using PreemptiveStrike.DetectionSystem;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.IncidentCaravan
{
    class TravelingIncidentCaravan : WorldObject
    {
        public int initialTile = -1;
        public Vector3 curPos;
        public int destinationTile = -1;

        private Vector3 DestinationPos { get { return Find.WorldGrid.GetTileCenter(destinationTile); } }

        private bool arrived;
        public int remainingTick = 0;
        //private float traveledPct;

        public InterceptedIncident incident;
        public bool detected;
        public bool confirmed;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref initialTile, "initialTile", 0, false);
            Scribe_Values.Look<Vector3>(ref curPos, "curPos", Vector3.zero, false);
            Scribe_Values.Look<int>(ref destinationTile, "destinationTile", 0, false);
            Scribe_Values.Look<bool>(ref arrived, "arrived", false, false);
            Scribe_Values.Look<int>(ref remainingTick, "remainingTick", 0, false);
            Scribe_Deep.Look<InterceptedIncident>(ref incident, "incident");
            Scribe_Values.Look<bool>(ref detected, "detected", false, false);
            Scribe_Values.Look<bool>(ref confirmed, "confirmed", false, false);
        }

        public override void PostAdd()
        {
            base.PostAdd();
            initialTile = base.Tile;
            curPos = Find.WorldGrid.GetTileCenter(initialTile);
            arrived = false;
            detected = false;
            confirmed = false;
            //traveledPct += (1f - traveledPct) / remainingTick;
        }

        public override void Tick()
        {
            base.Tick();
            if (remainingTick > 0)
                curPos = Vector3.Slerp(curPos, DestinationPos, 1f / remainingTick);
            else
                curPos = DestinationPos;
            --remainingTick;
            if (remainingTick <= 0)
            {
                remainingTick = 0;
                Arrive();
                return;
            }
            TileChangingTick();
        }

        private void TileChangingTick()
        {
            List<int> neighbors = new List<int>();
            Find.WorldGrid.GetTileNeighbors(Tile, neighbors);
            float minDist = Vector3.Distance(curPos, Find.WorldGrid.GetTileCenter(Tile));
            int p = -1;
            foreach(int x in neighbors)
            {
                float curDist = Vector3.Distance(curPos, Find.WorldGrid.GetTileCenter(x));
                if(curDist<minDist)
                {
                    minDist = curDist;
                    p = x;
                }
            }
            //Tile Changed!
            if (p != -1)
            {
                Tile = p;
                TileChangingAction();
            }
        }

        protected void TileChangingAction()
        {
            if (!confirmed)
            {
                bool newDetected = detected;
                if (!detected)
                    newDetected = DetectDangerUtilities.TryDetectIncidentCaravan(this);
                else
                {
                    if (DetectDangerUtilities.TryDetectCaravanDetail(this))
                        incident.RevealRandomInformation();
                }

                confirmed = DetectDangerUtilities.TryConfirmCaravanWithinVision(this);
                if (confirmed)
                    incident.RevealAllInformation();
                newDetected = newDetected || confirmed;

                if (newDetected != detected)
                {
                    detected = newDetected;
                    PreemptiveStrike.UI.ColonySecurityDashBoard_Window.Recache();
                }
                
            }
        }

        //I dont know how to conceal a world object(to make it unselectable), so I just hide it in the core of the planet...XD
        public override Vector3 DrawPos
        {
            get
            {
                if (PES_Settings.DebugModeOn)
                    return curPos;
                return detected? curPos : Vector3.zero;
            }
        }

        public override void Draw()
        {
            if(PES_Settings.DebugModeOn)
            {
                if (confirmed)
                    Material.color = Color.white;
                else if (detected)
                    Material.color = Color.cyan;
                else
                    Material.color = Color.black;
                base.Draw();
                return;
            }

            if (detected)
                base.Draw();
        }

        private void Arrive()
        {
            if (arrived) return;
            arrived = true;
            Messages.Message("Enemy Caravan Arrived!!!", MessageTypeDefOf.NeutralEvent);
            incident.ExecuteNow();
            Find.WorldObjects.Remove(this);
        }
    }
}
