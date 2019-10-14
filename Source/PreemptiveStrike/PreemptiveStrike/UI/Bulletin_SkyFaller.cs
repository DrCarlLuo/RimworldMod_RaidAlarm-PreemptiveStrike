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
    class Bulletin_SkyFaller : Bulletin
    {
        public Bulletin_SkyFaller() { }
        public Bulletin_SkyFaller(TravelingIncidentCaravan travelingIncidentCaravan)
        {
            Caravan = travelingIncidentCaravan;
            simpleCaravan = travelingIncidentCaravan as TravelingIncidentCaravan_Simple;
        }

        private TravelingIncidentCaravan_Simple simpleCaravan;
        private InterceptedIncident_SkyFaller Incident_SkyFaller => incident as InterceptedIncident_SkyFaller;

        protected override void DrawIcon(float x, float y)
        {
            if(Incident_SkyFaller.FallerType == SkyFallerType.Big)
            {
                if (incident.IntelLevel == IncidentIntelLevel.Danger)
                    MainIcon = Textures.IconLargeSkyObj_Hostile;
                else
                    MainIcon = Textures.IconLargeSkyObj_Neutral;
            }
            else
            {
                if (incident.IntelLevel == IncidentIntelLevel.Danger)
                    MainIcon = Textures.IconSmallSkyObj_Hostile;
                else
                    MainIcon = Textures.IconSmallSkyObj_Neutral;

            }
            base.DrawIcon(x, y);
        }

        protected override void DrawFirstLine(float x, float y)
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

        protected override void DrawSecondLine(float x, float y)
        {
            Rect directionRect = new Rect(x, y, 290f, UIConstants.TinyLabelHeight);

            if(Incident_SkyFaller.lookTargets == null)
            {
                Widgets.Label(directionRect, "PES_UI_SkyFaller_Spot_Unknown".Translate());
                return;
            }

            if (Widgets.ButtonText(directionRect, Incident_SkyFaller.FallerType == SkyFallerType.Big ? "PES_UI_SkyFaller_Spot_Big".Translate() : "PES_UI_SkyFaller_Spot_Small".Translate(), false))
            {
                CameraJumper.TryJump(Incident_SkyFaller.lookTargets.TryGetPrimaryTarget());
            }
            if (Mouse.IsOver(directionRect))
            {
                Incident_SkyFaller.lookTargets.TryHighlight();
            }
        }

        protected override void DrawThirdLine(float x, float y)
        {
            //Nothing to display here
        }
    }
}
