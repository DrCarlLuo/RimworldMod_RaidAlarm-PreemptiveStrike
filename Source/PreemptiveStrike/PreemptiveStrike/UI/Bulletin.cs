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
    abstract class Bulletin : IComparable<Bulletin>
    {
        public string MainLabel;
        public Texture2D MainIcon;
        public TravelingIncidentCaravan Caravan;

        public InterceptedIncident incident => Caravan.incident;

        public virtual IncidentIntelLevel bulletinIntelLevel => incident.IntelLevel;

        public static Bulletin Create(TravelingIncidentCaravan caravan)
        {
            InterceptedIncident incident = caravan.incident;
            if (incident is InterceptedIncident_HumanCrowd)
                return new Bulletin_Human(caravan);
            if (incident is InterceptedIncident_AnimalHerd)
                return new Bulletin_Animal(caravan);
            if (incident is InterceptedIncident_SkyFaller)
                return new Bulletin_SkyFaller(caravan);
            if (incident is InterceptedIncident_Infestation)
                return new Bulletin_Infestation(caravan);
            if (incident is InterceptedIncident_SolarFlare)
                return new Bulletin_Flare(caravan);
            return null;
        }

        protected virtual void DrawIcon(float x,float y)
        {
            if(Widgets.ButtonImage(new Rect(x, y, UIConstants.BulletinIconSize, UIConstants.BulletinIconSize), MainIcon) && Caravan!=null && (incident is InterceptedIncident_AnimalHerd || incident is InterceptedIncident_HumanCrowd))
            {
                CameraJumper.TryJumpAndSelect(Caravan);
            }
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
            if (RemainSparkCnt > 0)
                DoSpark(x, y);
            GUI.BeginGroup(new Rect(x,y,UIConstants.BulletinWidth,UIConstants.BulletinHeight));
            DrawIcon(0, UIConstants.BulletinIconIntend + 5f);
            float labelX = UIConstants.BulletinIconSize + UIConstants.BulletinIconIntend;
            DrawMainLabel(labelX, 0f);
            DrawFirstLine(labelX, UIConstants.MainLabelHeight+UIConstants.MainLabelIntend);
            DrawSecondLine(labelX, UIConstants.MainLabelHeight + UIConstants.MainLabelIntend + UIConstants.TinyLabelHeight+UIConstants.TinyLabelIntend);
            DrawThirdLine(labelX, UIConstants.MainLabelHeight + UIConstants.MainLabelIntend + UIConstants.TinyLabelHeight*2 + UIConstants.TinyLabelIntend*2);

            GUI.EndGroup();
        }

        private int RemainSparkCnt;
        private readonly static Color sparkColorA = new Color(0f, 0f, 0f, 0f);
        private readonly static Color sparkColorB = new Color(0.788f,0.259f,0.365f);//201,66,93
        private readonly static float sparkInterval = 0.25f;
        private float RemainSparkTime;
        private void DoSpark(float x, float y)
        {
            Color curColor;
            if (RemainSparkCnt % 2 == 0)
                curColor = Color.Lerp(sparkColorB, sparkColorA, RemainSparkTime);
            else
                curColor = Color.Lerp(sparkColorA, sparkColorB, RemainSparkTime);
            GUI.DrawTexture(new Rect(x, y, UIConstants.BulletinWidth, UIConstants.BulletinHeight), SolidColorMaterials.NewSolidColorTexture(curColor));
            RemainSparkTime -= Time.deltaTime;
            if (RemainSparkTime <= 0)
            {
                --RemainSparkCnt;
                RemainSparkTime = sparkInterval;
            }
        }

        public void BeginSpark(int times)
        {
            RemainSparkCnt = times * 2;
            RemainSparkTime = sparkInterval;
        }

        public int CompareTo(Bulletin other)
        {
            if (bulletinIntelLevel != other.bulletinIntelLevel)
                return bulletinIntelLevel.CompareTo(other.bulletinIntelLevel);
            if (Caravan == null) return 1;
            if (other.Caravan == null) return -1;
            if (Caravan.remainingTick != other.Caravan.remainingTick)
                return Caravan.remainingTick.CompareTo(other.Caravan.remainingTick);
            return Caravan.ID.CompareTo(other.Caravan.ID);
        }
    }
}
