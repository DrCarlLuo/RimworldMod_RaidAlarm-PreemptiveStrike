using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Interceptor;

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
                MainIcon = Textures.IconSword;
            else if (bulletinIntelLevel == IncidentIntelLevel.Neutral)
                MainIcon = Textures.IconMerchant;
            base.DrawIcon(x, y);
        }

        protected override void DrawFirstLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Color orgColor = GUI.color;
            string intentionStr = "Error";

            if (!Incident_Human.intention_revealed)
            {
                intentionStr = "Unknown Intention";
                Widgets.Label(new Rect(x, y, 300f, UIConstants.TinyLabelHeight), intentionStr);
            }
            else
            {
                if (Incident_Human is InterceptedIncident_HumanCrowd_RaidEnemy)
                {
                    var incident1 = Incident_Human as InterceptedIncident_HumanCrowd_RaidEnemy;

                    GUI.color = Color.red;
                    intentionStr = "Raid(Concentrate Invade)";
                    Widgets.Label(new Rect(x, y, 150f, UIConstants.TinyLabelHeight), intentionStr);

                    string strategyString = "Error";
                    if (incident1.raidStrategy_revealed)
                        strategyString = incident1.RaidStrategy.label;
                    else
                        strategyString = "Unknown";

                    GUI.color = Color.white;
                    Widgets.Label(new Rect(x + 160f, y, 240f, UIConstants.TinyLabelHeight), "Strategy: " + strategyString);
                }
                else if (Incident_Human is InterceptedIncident_HumanCrowd_TraderCaravan)
                {
                    var incident1 = Incident_Human as InterceptedIncident_HumanCrowd_TraderCaravan;

                    GUI.color = Color.cyan;
                    intentionStr = "Trade Caravan";
                    Widgets.Label(new Rect(x, y, 150f, UIConstants.TinyLabelHeight), intentionStr);

                    if (incident1.caravanType_revealed && incident1.traderKind != null)
                    {
                        GUI.color = Color.white;
                        Widgets.Label(new Rect(x + 160f, y, 240f, UIConstants.TinyLabelHeight), incident1.traderKind.label);
                    }
                }
                else if (Incident_Human is InterceptedIncident_HumanCrowd_TravelerGroup)
                {
                    intentionStr = "Travelers";
                    Widgets.Label(new Rect(x, y, 300f, UIConstants.TinyLabelHeight), intentionStr);
                }
                else if (Incident_Human is InterceptedIncident_HumanCrowd_VisitorGroup)
                {
                    intentionStr = "Visitors";
                    Widgets.Label(new Rect(x, y, 300f, UIConstants.TinyLabelHeight), intentionStr);
                }
            }
            GUI.color = orgColor;
        }

        protected override void DrawSecondLine(float x, float y)
        {
            StringBuilder sb = new StringBuilder("Faction: ");
            if (!Incident_Human.faction_revealed)
                sb.Append("unknown");
            else
            {
                sb.Append(Incident_Human.SourceFaction.Name);
                if (Incident_Human.SourceFaction.PlayerRelationKind == FactionRelationKind.Hostile)
                    sb.Append(@"(<color=red>Hostile</color>)");
                else if (Incident_Human.SourceFaction.PlayerRelationKind == FactionRelationKind.Ally)
                    sb.Append(@"(<color=blue>Ally</color>)");
                else
                    sb.Append(@"(<color=white>Neutral</color>)");
            }
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(x, y, 200f, UIConstants.TinyLabelHeight), sb.ToString());

            string numberStr = "Corps Size: ";
            if (Incident_Human.crowdSize_revealed)
                numberStr += Incident_Human.CrowdSize.ToString();
            else
                numberStr += "unknown";
            Widgets.Label(new Rect(x + 210f, y, 190f, UIConstants.TinyLabelHeight), numberStr);
        }

        protected override void DrawThirdLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(x, y, 100f, UIConstants.TinyLabelHeight), "ETA: " + Caravan.remainingTick);
            Widgets.ButtonText(new Rect(x + 110f, y, 290f, UIConstants.TinyLabelHeight), "Incoming Direction: unknown", false);
        }

    }
}
