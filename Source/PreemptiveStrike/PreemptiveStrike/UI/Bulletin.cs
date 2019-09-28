using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.Interceptor;

namespace PreemptiveStrike.UI
{
    enum BulletinCategory
    {
        Danger,
        Unknown,
        Neutral
    }

    abstract class Bulletin
    {
        public string MainLabel;
        public Texture2D MainIcon;
        public TravelingIncidentCaravan Caravan;

        public InterceptedIncident incident => Caravan.incident;

        public IncidentIntelLevel bulletinIntelLevel => incident.IntelLevel;

        public static Bulletin Create(TravelingIncidentCaravan caravan)
        {
            InterceptedIncident incident = caravan.incident;
            if (incident is InterceptedIncident_HumanCrowd)
                return new Bulletin_Human(caravan);
            if (incident is InterceptedIncident_AnimalHerd)
                return new Bulletin_Animal(caravan);
            return null;
        }

        protected virtual void DrawIcon(float x,float y)
        {
            Widgets.ButtonImage(new Rect(x, y, UIConstants.BulletinIconSize, UIConstants.BulletinIconSize), MainIcon);
        }

        protected virtual void DrawMainLabel(float x, float y)
        {
            MainLabel = Caravan.CaravanTitle;
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(x, y, 350f, UIConstants.MainLabelHeight), MainLabel);
        }

        protected virtual void DrawFirstLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(x, y, 150f, UIConstants.TinyLabelHeight), "Raid(Concentrate invade)");
            Widgets.Label(new Rect(x + 160f, y, 250f, UIConstants.TinyLabelHeight), "Strategy:Prepare then attack");
        }

        protected virtual void DrawSecondLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(x, y, 350f, UIConstants.TinyLabelHeight), "Faction: Holy Roman Empire 2333333");
        }
        protected virtual void DrawThirdLine(float x, float y)
        {
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(x, y, 350f, UIConstants.TinyLabelHeight), "Incoming Direction: known ETA:10-20h");
        }


        public void OnDraw(float x, float y)
        {
            GUI.BeginGroup(new Rect(x,y,UIConstants.BulletinWidth,UIConstants.BulletinHeight));

            DrawIcon(0, UIConstants.BulletinIconIntend);
            float labelX = UIConstants.BulletinIconSize + UIConstants.BulletinIconIntend;
            DrawMainLabel(labelX, 0f);
            DrawFirstLine(labelX, UIConstants.MainLabelHeight+UIConstants.MainLabelIntend);
            DrawSecondLine(labelX, UIConstants.MainLabelHeight + UIConstants.MainLabelIntend + UIConstants.TinyLabelHeight+UIConstants.TinyLabelIntend);
            DrawThirdLine(labelX, UIConstants.MainLabelHeight + UIConstants.MainLabelIntend + UIConstants.TinyLabelHeight*2 + UIConstants.TinyLabelIntend*2);

            GUI.EndGroup();
        }
    }
}
