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

        protected Vector3 DestinationPos { get { return Find.WorldGrid.GetTileCenter(destinationTile); } }

        protected bool arrived;
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
        protected bool relationInformed = false;

        public virtual string CaravanTitle
        {
            get
            {
                if (confirmed || CommunicationEstablished)
                    return incident.IncidentTitle_Confirmed;
                else
                    return incident.IncidentTitle_Unknow;
            }
        }

        public override string Label => CaravanTitle;

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
            Scribe_Values.Look(ref relationInformed, "relationInformed", false, false);
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

        protected virtual void TileChangingTick()
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
                    {
                        incident.RevealRandomInformation();
                        TryNotifyCaravanIntel();
                    }
                }

                confirmed = DetectDangerUtilities.TryConfirmCaravanWithinVision(this);
                if (confirmed)
                {
                    incident.RevealAllInformation();
                    if (Communicable) EstablishCommunication();
                    TryNotifyCaravanIntel();
                    NotifyConfirmed();
                }
                newDetected = newDetected || confirmed;

                if (newDetected != detected)
                {
                    detected = newDetected;
                    if (!confirmed) NotifyDetected();
                    EventManger.NotifyCaravanListChange?.Invoke();
                }

            }
        }

        public void NotifyDetected()
        {
            if (incident is InterceptedIncident_AnimalHerd)
                OpenUILetter.Make("PES_Detected_Title_Animal".Translate(), "PES_Detected_Text_Animal".Translate(), LetterDefOf.NeutralEvent);
            else
                OpenUILetter.Make("PES_Detected_Title_Human".Translate(), "PES_Detected_Text_Human".Translate(), LetterDefOf.NeutralEvent);
        }

        public void NotifyConfirmed()
        {
            Messages.Message("PES_Caravan_Confirmed".Translate(CaravanTitle), MessageTypeDefOf.NeutralEvent);
        }

        public virtual void TryNotifyCaravanIntel()
        {
            if (incident.IntelLevel == IncidentIntelLevel.Unknown)
                return;
            if (relationInformed)
                return;
            if (incident.IntelLevel == IncidentIntelLevel.Neutral)
            {
                if (incident is InterceptedIncident_AnimalHerd)
                    Messages.Message("PES_Neutral_Message_Animal".Translate(), MessageTypeDefOf.NeutralEvent);
                else
                    Messages.Message("PES_Neutral_Message".Translate(), MessageTypeDefOf.NeutralEvent);
            }
            else
            {
                if (incident is InterceptedIncident_AnimalHerd)
                    SparkUILetter.Make("PES_Hostile_Confirmed".Translate(), "PES_Hostile_Text_Animal".Translate(), LetterDefOf.ThreatBig, this);
                else
                    SparkUILetter.Make("PES_Hostile_Confirmed".Translate(), "PES_Hostile_Text_Human".Translate(), LetterDefOf.ThreatBig, this);
            }
            relationInformed = true;
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

        private static Material hostileMat = MaterialPool.MatFrom(WorldObjectDefOf.Caravan.texture, ShaderDatabase.WorldOverlayTransparentLit, Color.red, WorldMaterials.DynamicObjectRenderQueue);
        private static Material AllyMat = MaterialPool.MatFrom(WorldObjectDefOf.Caravan.texture, ShaderDatabase.WorldOverlayTransparentLit, Color.cyan, WorldMaterials.DynamicObjectRenderQueue);
        private static Material UnknownMat = MaterialPool.MatFrom(WorldObjectDefOf.Caravan.texture, ShaderDatabase.WorldOverlayTransparentLit, Color.white, WorldMaterials.DynamicObjectRenderQueue);
        public override Material Material
        {
            get
            {
                if (incident.IntelLevel == IncidentIntelLevel.Danger)
                    return hostileMat;
                else if (incident.IntelLevel == IncidentIntelLevel.Neutral)
                    return AllyMat;
                else
                    return UnknownMat;
            }
        }

        public override string GetInspectString()
        {
            if (stageRemainingTick > 0)
                return "PES_UI_CaravanStaging".Translate();
            return "PES_UI_CaravanMoving".Translate((incident.parms.target as Map).Parent.Label);
        }

        protected void Arrive()
        {
            if (PES_Settings.DebugModeOn)
                Log.Message("Carravan try arrive");
            if (arrived) return;
            arrived = true;
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
            if (!Communicable)
                throw new Exception("Try to establish communication with non-commnicable!");
            if (!CommunicationEstablished)
            {
                CommunicationEstablished = true;
                incident.RevealInformationWhenCommunicationEstablished();
                if (!incident.IsHostileToPlayer)
                {
                    confirmed = detected = true;
                    incident.RevealAllInformation();
                }
                TryNotifyCaravanIntel();
                if (DialogUtilities.MapHasCommsConsole(incident.parms.target as Map))
                    Messages.Message("PES_CommunicationEstablished".Translate(CaravanTitle), MessageTypeDefOf.NeutralEvent);
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
