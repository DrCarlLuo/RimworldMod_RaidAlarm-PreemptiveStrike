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
    class Bulletin_Animal : Bulletin
    {
        public Bulletin_Animal() { }
        public Bulletin_Animal(TravelingIncidentCaravan travelingIncidentCaravan)
        {
            Caravan = travelingIncidentCaravan;
        }

        private InterceptedIncident_AnimalHerd Incident_Animal => incident as InterceptedIncident_AnimalHerd;

        protected override void DrawIcon(float x, float y)
        {
            if (bulletinIntelLevel == IncidentIntelLevel.Unknown)
                MainIcon = Textures.IconUnknown;
            else if (bulletinIntelLevel == IncidentIntelLevel.Danger)
                MainIcon = Textures.IconRaidAnimal;
            else if (bulletinIntelLevel == IncidentIntelLevel.Neutral)
                MainIcon = Textures.IconHerd;
            base.DrawIcon(x, y);
        }

        protected override void DrawFirstLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Color orgColor = GUI.color;

            string intentionStr = "Error";
            Color intentionColor = Color.white;

            if(!Incident_Animal.intention_revealed)
            {
                intentionStr = "PES_UI_UnknownIntention".Translate();
                intentionColor = Color.white;
            }
            else
            {
                if(Incident_Animal is InterceptedIncident_AnimalHerd_ManhunterPack)
                {
                    intentionColor = Color.red;
                    intentionStr = "PES_Intention_ManHunterPack".Translate();
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_Alphabeavers)
                {
                    intentionStr = "PES_Intention_AlphaBeavers".Translate();
                    intentionColor = Color.red;
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_FarmAnimalsWanderIn)
                {
                    intentionStr = "PES_Intention_FarmAnimalsWanderIn".Translate();
                    intentionColor = Color.cyan;
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_HerdMigration)
                {
                    intentionStr = "PES_Intention_HerdMigration".Translate();
                    intentionColor = Color.cyan;
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_ThrumboPasses)
                {
                    intentionStr = "PES_Intention_ThrumboPass".Translate();
                    intentionColor = Color.cyan;
                }
            }

            GUI.color = intentionColor;
            Widgets.Label(new Rect(x, y, 300f, UIConstants.TinyLabelHeight), intentionStr);

            GUI.color = orgColor;
        }

        protected override void DrawSecondLine(float x, float y)
        {
            string kindStr = "Error";
            if (Incident_Animal.animalType_revealed)
                kindStr = Incident_Animal.AnimalType.label;
            else
                kindStr = "PES_AnimalType_Unknown".Translate();

            Widgets.Label(new Rect(x, y, 200f, UIConstants.TinyLabelHeight), kindStr);

            string numberStr = "PES_UI_Herd".Translate();
            if (Incident_Animal.animalNum_revealed)
                numberStr += Incident_Animal.AnimalNum.ToString();
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

            string directStr = Incident_Animal.spawnPosition_revealed ? "PES_UI_Direction_known".Translate() : "PES_UI_Direction_unknown".Translate();
            float dirWidth = Text.CurFontStyle.CalcSize(new GUIContent(directStr)).x + 5f;
            Rect directionRect = new Rect(Mathf.Max(x + timeWidth + 5f, UIConstants.BulletinWidth - dirWidth), y, dirWidth, UIConstants.TinyLabelHeight);
            if (Widgets.ButtonText(directionRect, directStr, false) && Incident_Animal.spawnPosition_revealed)
            {
                CameraJumper.TryJump(Incident_Animal.lookTargets.TryGetPrimaryTarget());
            }
            if(Incident_Animal.spawnPosition_revealed && Mouse.IsOver(directionRect))
            {
                Incident_Animal.lookTargets.TryHighlight();
            }
        }
    }
}
