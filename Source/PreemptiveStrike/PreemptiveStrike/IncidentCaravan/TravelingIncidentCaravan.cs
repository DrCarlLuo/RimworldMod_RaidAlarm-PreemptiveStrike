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
using PreemptiveStrike.Dialogue;

namespace PreemptiveStrike.IncidentCaravan
{
    class TravelingIncidentCaravan : WorldObject, ICommunicable
    {
        public int initialTile = -1;
        public Vector3 curPos;
        public int destinationTile = -1;

        private Vector3 DestinationPos { get { return Find.WorldGrid.GetTileCenter(destinationTile); } }

        private bool arrived;
        public int remainingTick = 0;
        public int broadcastMessageCoolDownTick = 0;
        public int negotiateCoolDownTick = 0;
        public int delayCoolDownTick = 0;
        public int stageRemainingTick = 0;
        public bool StagedBefore = false;
        //private float traveledPct;

        public InterceptedIncident incident;
        public bool detected;
        public bool confirmed;

        public string CaravanTitle
        {
            get
            {
                if (confirmed || CommunicationEstablished)
                    return incident.IncidentTitle_Confirmed;
                else
                    return incident.IncidentTitle_Unknow;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref initialTile, "initialTile", 0, false);
            Scribe_Values.Look(ref curPos, "curPos", Vector3.zero, false);
            Scribe_Values.Look(ref destinationTile, "destinationTile", 0, false);
            Scribe_Values.Look(ref arrived, "arrived", false, false);
            Scribe_Values.Look(ref remainingTick, "remainingTick", 0, false);
            Scribe_Values.Look(ref stageRemainingTick, "stageRemainingTick", 0, false);
            Scribe_Values.Look(ref broadcastMessageCoolDownTick, "broadcastMessageCoolDownTick", 0, false);
            Scribe_Values.Look(ref negotiateCoolDownTick, "negotiateCoolDownTick", 0, false);
            Scribe_Values.Look(ref delayCoolDownTick, "delayCoolDownTick", 0, false);
            Scribe_Values.Look(ref StagedBefore, "StagedBefore", false, false);
            Scribe_Deep.Look(ref incident, "incident");
            Scribe_Values.Look(ref detected, "detected", false, false);
            Scribe_Values.Look(ref confirmed, "confirmed", false, false);
            Scribe_Values.Look(ref CommunicationEstablished, "CommunicationEstablished", false, false);
            Scribe_Values.Look(ref Communicable, "Communicable", false, false);
        }

        public override void PostAdd()
        {
            base.PostAdd();
            initialTile = base.Tile;
            curPos = Find.WorldGrid.GetTileCenter(initialTile);
            arrived = false;
            detected = false;
            confirmed = false;
            Communicable = incident is InterceptedIncident_HumanCrowd;
            //traveledPct += (1f - traveledPct) / remainingTick;
        }

        public override void Tick()
        {
            base.Tick();

            //This is a little hard-coding, need fixes
            if (incident is InterceptedIncident_HumanCrowd_RaidEnemy raidIncident) raidIncident.goal.GoalTick();

            if (broadcastMessageCoolDownTick > 0) --broadcastMessageCoolDownTick;
            if (negotiateCoolDownTick > 0) --negotiateCoolDownTick;
            if (delayCoolDownTick > 0) --delayCoolDownTick;
            if (stageRemainingTick > 0)
            {
                --stageRemainingTick;
                return;
            }

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
            foreach (int x in neighbors)
            {
                float curDist = Vector3.Distance(curPos, Find.WorldGrid.GetTileCenter(x));
                if (curDist < minDist)
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
                    EventManger.NotifyCaravanListChange?.Invoke();
                }

            }
        }

        public void StageForThreeHours()
        {
            stageRemainingTick = 7500;
            StagedBefore = true;
        }

        public void ApplyNegotiationCoolDown() { negotiateCoolDownTick = 2500; }
        public void ApplyBroadCastCoolDown() { broadcastMessageCoolDownTick = 2500; }
        public void ApplyDelayCoolDown() { delayCoolDownTick = 2500; }

        //I dont know how to conceal a world object(to make it unselectable), so I just hide it in the core of the planet...XD
        public override Vector3 DrawPos
        {
            get
            {
                if (PES_Settings.DebugModeOn)
                    return curPos;
                return detected ? curPos : Vector3.zero;
            }
        }

        public override void Draw()
        {
            if (PES_Settings.DebugModeOn)
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

        public void Dismiss()
        {
            Find.WorldObjects.Remove(this);
            Messages.Message("PES_CaravanDismiss".Translate(CaravanTitle), MessageTypeDefOf.NeutralEvent);

        }

        #region Communication
        public bool Communicable = false;
        public InterceptedIncident_HumanCrowd CommunicableIncident => incident as InterceptedIncident_HumanCrowd;

        public bool CommunicationEstablished = false;

        public void EstablishCommunication()
        {
            if (!CommunicationEstablished)
            {
                CommunicationEstablished = true;
                incident.RevealInformationWhenCommunicationEstablished();
                if (!incident.IsHostileToPlayer)
                    confirmed = detected = true;
                EventManger.NotifyCaravanListChange?.Invoke();
            }
        }

        public string GetCallLabel()
        {
            return CaravanTitle;
        }

        public string GetInfoText()
        {
            if (CommunicableIncident.faction_revealed)
                return CommunicableIncident.SourceFaction.GetInfoText();
            else
                return "PES_UnknownFaction".Translate();
        }

        public void TryOpenComms(Pawn negotiator)
        {
            DialogUtilities.BeginCaravanDialog(negotiator, this);
        }

        public Faction GetFaction()
        {
            if (CommunicableIncident.faction_revealed)
                return CommunicableIncident.SourceFaction;
            else
                return null;
        }

        public FloatMenuOption CommFloatMenuOption(Building_CommsConsole console, Pawn negotiator)
        {
            StringBuilder sb = new StringBuilder("");
            if (CommunicationEstablished)
                sb.Append("CallOnRadio".Translate(this.GetCallLabel()));
            else
                sb.Append("PES_TryToCallOnRadio".Translate(GetCallLabel()));
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(sb.ToString(), delegate ()
            {
                console.GiveUseCommsJob(negotiator, this);
            }, MenuOptionPriority.InitiateSocial), negotiator, console);
        }

        #endregion
    }
}
