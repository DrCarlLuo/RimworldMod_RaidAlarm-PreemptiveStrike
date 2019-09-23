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
    class Bulletin_Animal : Bulletin
    {
        public Bulletin_Animal() { }
        public Bulletin_Animal(TravelingIncidentCaravan travelingIncidentCaravan)
        {
            Caravan = travelingIncidentCaravan;
            DetermineCategory();
        }

        private InterceptedIncident_AnimalHerd Incident_Animal => incident as InterceptedIncident_AnimalHerd;

        protected override void DetermineCategory()
        {
            if (Incident_Animal.intention_revealed)
            {
                if (Incident_Animal is InterceptedIncident_AnimalHerd_ManhunterPack || Incident_Animal is InterceptedIncident_AnimalHerd_Alphabeavers)
                    bulletinCategory = BulletinCategory.Danger;
                else
                    bulletinCategory = BulletinCategory.Neutral;
            }
            else
                bulletinCategory = BulletinCategory.Unknown;
        }

        protected override void DrawIcon(float x, float y)
        {
            if (bulletinCategory == BulletinCategory.Unknown)
                MainIcon = Textures.IconUnknown;
            else if (bulletinCategory == BulletinCategory.Danger)
                MainIcon = Textures.IconAnimal;
            else if (bulletinCategory == BulletinCategory.Neutral)
                MainIcon = Textures.IconAnimal;
            base.DrawIcon(x, y);
        }

        protected override void DrawMainLabel(float x, float y)
        {
            MainLabel = "Unidentified Animal Herd";
            if (Caravan.confirmed)
            {
                if (bulletinCategory == BulletinCategory.Danger)
                    MainLabel = "Devouring Swarms";
                else if (bulletinCategory == BulletinCategory.Neutral)
                    MainLabel = "Puppy and Kittens";
            }
            base.DrawMainLabel(x, y);
        }

        protected override void DrawFirstLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Color orgColor = GUI.color;

            string intentionStr = "Error";
            Color intentionColor = Color.white;

            if(!Incident_Animal.intention_revealed)
            {
                intentionStr = "Unknown Intention";
                intentionColor = Color.white;
            }
            else
            {
                if(Incident_Animal is InterceptedIncident_AnimalHerd_ManhunterPack)
                {
                    intentionColor = Color.red;
                    intentionStr = "Manhunting Pack";
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_Alphabeavers)
                {
                    intentionStr = "Wood Eating";
                    intentionColor = Color.red;
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_FarmAnimalsWanderIn)
                {
                    intentionStr = "Join Colony";
                    intentionColor = Color.cyan;
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_HerdMigration)
                {
                    intentionStr = "Herd Migration";
                    intentionColor = Color.cyan;
                }
                else if(Incident_Animal is InterceptedIncident_AnimalHerd_ThrumboPasses)
                {
                    intentionStr = "Wandering";
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
                kindStr = "Unidentified Animal Type";

            Widgets.Label(new Rect(x, y, 200f, UIConstants.TinyLabelHeight), kindStr);

            string numberStr = "Herd Size: ";
            if (Incident_Animal.animalNum_revealed)
                numberStr += Incident_Animal.AnimalNum.ToString();
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
