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
    class Bulletin_Flare : Bulletin
    {
        public Bulletin_Flare() { }
        public Bulletin_Flare(TravelingIncidentCaravan travelingIncidentCaravan)
        {
            Caravan = travelingIncidentCaravan;
            simpleCaravan = travelingIncidentCaravan as TravelingIncidentCaravan_Simple;
        }

        private TravelingIncidentCaravan_Simple simpleCaravan;
        private InterceptedIncident_SolarFlare Incident_Flare => incident as InterceptedIncident_SolarFlare;

        public override IncidentIntelLevel bulletinIntelLevel => IncidentIntelLevel.Danger;

        protected override void DrawIcon(float x, float y)
        {
            MainIcon = Textures.IconSolarFlare;
            base.DrawIcon(x, y);
        }

        protected override void DrawMainLabel(float x, float y)
        {
            MainLabel = "PES_Warning_Flare_Early".Translate();
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(x, y, 350f, UIConstants.MainLabelHeight), MainLabel);
        }

        protected override void DrawFirstLine(float x, float y)
        {

        }

        protected override void DrawSecondLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            string timeStr = "";
            if (PES_Settings.DebugModeOn)
                timeStr = "PES_UI_ETA".Translate() + Caravan.remainingTick;
            else
                timeStr = "PES_UI_ETA".Translate() + GenDate.ToStringTicksToPeriod(Caravan.remainingTick);
            float timeWidth = Text.CurFontStyle.CalcSize(new GUIContent(timeStr)).x;
            Widgets.Label(new Rect(x, y, timeWidth + 5f, UIConstants.TinyLabelHeight), timeStr);
        }

        protected override void DrawThirdLine(float x, float y)
        {
            
        }


    }
}
