using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Interceptor;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.UI
{
    class Bulletin_Human : Bulletin
    {
        public Bulletin_Human() { }
        public Bulletin_Human(TravelingIncidentCaravan travelingIncidentCaravan)
        {
            Caravan = travelingIncidentCaravan;
        }

        private InterceptedIncident_HumanCrowd Incident_Human => incident as InterceptedIncident_HumanCrowd;

        protected override void DrawIcon(float x, float y)
        {
            if (bulletinIntelLevel == IncidentIntelLevel.Unknown)
                MainIcon = Textures.IconUnknown;
            else if (bulletinIntelLevel == IncidentIntelLevel.Danger)
                MainIcon = Textures.IconRaidHuman;
            else if (bulletinIntelLevel == IncidentIntelLevel.Neutral)
            {
                if (Incident_Human is InterceptedIncident_HumanCrowd_Neutral neutralIncident)
                {
                    if (!neutralIncident.intention_revealed)
                        MainIcon = Textures.IconFriendly_Unknown;
                    else
                    {
                        if (Incident_Human is InterceptedIncident_HumanCrowd_TraderCaravan)
                            MainIcon = Textures.IconTrader;
                        else
                            MainIcon = Textures.IconTraveler;
                    }
                }
            }
            base.DrawIcon(x, y);
        }

        protected override void DrawFirstLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Color orgColor = GUI.color;
            string intentionStr = "Error";

            if (!Incident_Human.intention_revealed)
            {
                intentionStr = "PES_UI_UnknownIntention".Translate();
                Widgets.Label(new Rect(x, y, 300f, UIConstants.TinyLabelHeight), intentionStr);
            }
            else
            {
                if (Incident_Human is InterceptedIncident_HumanCrowd_RaidEnemy)
                {
                    var incident1 = Incident_Human as InterceptedIncident_HumanCrowd_RaidEnemy;

                    GUI.color = Color.red;
                    intentionStr = "PES_Intention_Raid".Translate();
                    if (Caravan.CommunicationEstablished || Caravan.confirmed)
                        intentionStr += string.Format(@"({0})", incident1.goal.Lable);
                    Widgets.Label(new Rect(x, y, 150f, UIConstants.TinyLabelHeight), intentionStr);

                    string strategyString = "Error";
                    if (incident1.raidStrategy_revealed)
                    {
                        strategyString = incident1.StrategyString;
                        if (Incident_Human is InterceptedIncident_HumanCrowd_RaidEnemy_Groups)
                            strategyString += "PES_Intention_RaidInGroups".Translate();
                    }
                    else
                        strategyString = "PES_UI_Unknown".Translate();

                    GUI.color = Color.white;
                    Widgets.Label(new Rect(x + 160f, y, 240f, UIConstants.TinyLabelHeight), "PES_UI_Strategy".Translate(strategyString));
                }
                else if (Incident_Human is InterceptedIncident_HumanCrowd_TraderCaravan)
                {
                    var incident1 = Incident_Human as InterceptedIncident_HumanCrowd_TraderCaravan;

                    GUI.color = Color.cyan;
                    intentionStr = "PES_Intention_TradeCaravan".Translate();
                    Widgets.Label(new Rect(x, y, 150f, UIConstants.TinyLabelHeight), intentionStr);

                    if (incident1.caravanType_revealed && incident1.traderKind != null)
                    {
                        GUI.color = Color.white;
                        Widgets.Label(new Rect(x + 160f, y, 240f, UIConstants.TinyLabelHeight), incident1.traderKind.label);
                    }
                }
                else if (Incident_Human is InterceptedIncident_HumanCrowd_TravelerGroup)
                {
                    intentionStr = "PES_Intention_Traveler".Translate();
                    Widgets.Label(new Rect(x, y, 300f, UIConstants.TinyLabelHeight), intentionStr);
                }
                else if (Incident_Human is InterceptedIncident_HumanCrowd_VisitorGroup)
                {
                    intentionStr = "PES_Intention_Visitor".Translate();
                    Widgets.Label(new Rect(x, y, 300f, UIConstants.TinyLabelHeight), intentionStr);
                }
            }
            GUI.color = orgColor;
        }

        protected override void DrawSecondLine(float x, float y)
        {
            StringBuilder sb = new StringBuilder("PES_UI_Faction".Translate());
            if (!Incident_Human.faction_revealed)
                sb.Append("PES_UI_Unknown".Translate());
            else
            {
                sb.Append(Incident_Human.SourceFaction.Name);
                if (Incident_Human.SourceFaction.PlayerRelationKind == FactionRelationKind.Hostile)
                    sb.Append(string.Format(@"(<color=red>{0}</color>)", "Hostile".Translate()));
                else if (Incident_Human.SourceFaction.PlayerRelationKind == FactionRelationKind.Ally)
                    sb.Append(string.Format(@"(<color=blue>{0}</color>)", "Ally".Translate()));
                else
                    sb.Append(string.Format(@"(<color=white>{0}</color>)", "Neutral".Translate()));
            }
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(x, y, 200f, UIConstants.TinyLabelHeight), sb.ToString());

            string numberStr = "PES_UI_Corps".Translate();
            if (Incident_Human.crowdSize_revealed)
                numberStr += Incident_Human.CrowdSize.ToString();
            else
                numberStr += "PES_UI_Unknown".Translate();
            Widgets.Label(new Rect(x + 210f, y, 190f, UIConstants.TinyLabelHeight), numberStr);
        }

        protected override void DrawThirdLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            string timeStr = "";
            if (PES_Settings.DebugModeOn)
                timeStr = "PES_UI_ETA".Translate() + Caravan.remainingTick;
            else
                timeStr = "PES_UI_ETA".Translate() + GenDate.ToStringTicksToPeriod(Caravan.remainingTick);
            float timeWidth = Text.CurFontStyle.CalcSize(new GUIContent(timeStr)).x;
            Widgets.Label(new Rect(x, y, timeWidth + 5f, UIConstants.TinyLabelHeight), timeStr);

            string directStr = Incident_Human.spawnPosition_revealed ? "PES_UI_Direction_known".Translate() : "PES_UI_Direction_unknown".Translate();
            float dirWidth = Text.CurFontStyle.CalcSize(new GUIContent(directStr)).x + 5f;
            Rect directionRect = new Rect(Mathf.Max(x + timeWidth + 5f, UIConstants.BulletinWidth - dirWidth), y, dirWidth, UIConstants.TinyLabelHeight);
            if (Widgets.ButtonText(directionRect, directStr, false) && Incident_Human.spawnPosition_revealed)
            {
                CameraJumper.TryJump(Incident_Human.lookTargets.TryGetPrimaryTarget());
            }
            if (Incident_Human.spawnPosition_revealed && Mouse.IsOver(directionRect))
            {
                Incident_Human.lookTargets.TryHighlight();
            }
        }

    }
}
