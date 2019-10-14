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
    class Bulletin_Infestation : Bulletin
    {
        public Bulletin_Infestation() { }
        public Bulletin_Infestation(TravelingIncidentCaravan travelingIncidentCaravan)
        {
            Caravan = travelingIncidentCaravan;
            simpleCaravan = travelingIncidentCaravan as TravelingIncidentCaravan_Simple;
        }

        private TravelingIncidentCaravan_Simple simpleCaravan;
        private InterceptedIncident_Infestation Incident_Infestation => incident as InterceptedIncident_Infestation;

        protected override void DrawIcon(float x, float y)
        {
            MainIcon = Textures.IconInfestation;
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

            if (Widgets.ButtonText(directionRect, "PES_UI_Infestation_Spot".Translate(),false))
            {
                CameraJumper.TryJump(Incident_Infestation.lookTargets.TryGetPrimaryTarget());
            }
            if (Mouse.IsOver(directionRect))
            {
                Incident_Infestation.lookTargets.TryHighlight();
            }
        }

        protected override void DrawThirdLine(float x, float y)
        {
            //Nothing to display here
        }
    }
}
