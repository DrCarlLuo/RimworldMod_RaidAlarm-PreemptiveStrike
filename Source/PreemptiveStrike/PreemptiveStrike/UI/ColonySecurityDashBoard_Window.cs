using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.DetectionSystem;

namespace PreemptiveStrike.UI
{
    class ColonySecurityDashBoard_Window : Window
    {
        public static ColonySecurityDashBoard_Window Instance { get; } = new ColonySecurityDashBoard_Window();

        public static List<Bulletin> BulletinCache = new List<Bulletin>();

        public static void Recache()
        {
            BulletinCache.Clear();
            foreach (var x in IncidentCaravanUtility.IncidentCaravans)
            {
                if(x.detected)
                    BulletinCache.Add(Bulletin.Create(x));
            }
            ReCalulateSize();
        }

        //private WindowResizer Resizer = new WindowResizer();

        public static void ReCalulateSize()
        {
            float height = (UIConstants.BulletinHeight + UIConstants.BulletinIntend) * Math.Min(BulletinCache.Count, 3);
            Instance.windowRect = new Rect(Verse.UI.screenWidth - UIConstants.DefualtWindowPin2RightIntend - UIConstants.DefaultWindowWidth, 100f, UIConstants.DefaultWindowWidth, UIConstants.TitleHeight + UIConstants.TitleIntend + height + 30f);
        }

        private ColonySecurityDashBoard_Window()
        {
            this.preventCameraMotion = false;
            this.closeOnCancel = false;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = false;
            this.focusWhenOpened = false;
            this.draggable = true;
            this.resizeable = false;
            this.layer = WindowLayer.GameUI;
            this.soundAppear = null;
            this.soundClose = null;
            this.doCloseX = false;
            this.doCloseButton = false;
            //this.Resizer.minWindowSize.x = 200.0f;
            //this.Resizer.minWindowSize.y = 100.0f;
            //this.windowRect = new Rect(Verse.UI.screenWidth - 435f, 100f, 420f, 420f);
        }

        protected override void SetInitialSizeAndPosition()
        {
            ReCalulateSize();
        }

        private Vector2 scrollViewVec = Vector2.zero;

        public override void DoWindowContents(Rect inRect)
        {
            //GUI.Label(new Rect(10f, 10f, 400f, 30f), "Colony Security Report");
            GUI.Label(new Rect(10f, 10f, 400f, 30f), DetectionAbility);

            scrollViewVec = GUI.BeginScrollView(new Rect(5f, 50f, UIConstants.BulletinWidth + 20f, UIConstants.BulletinHeight * 3 + UIConstants.BulletinIntend * 3), scrollViewVec, new Rect(0f, 0f, UIConstants.BulletinWidth, BulletinCache.Count * (UIConstants.BulletinHeight + UIConstants.BulletinIntend)));

            float bulletinY = 0f;
            foreach (var bulletin in BulletinCache)
            {
                bulletin.OnDraw(0f, bulletinY);
                bulletinY += UIConstants.BulletinHeight + 5f;
            }

            GUI.EndScrollView();
            //Bulletin test = new Bulletin();
            //test.OnDraw(0f,50f);
            //Widgets.ButtonImage(new Rect(10f, 40f, 30f, 30f), Textures.IconSword);
        }

        private string DetectionAbility
        {
            get
            {
                if(Find.CurrentMap != null)
                {
                    if (!DetectDangerUtilities.DetectionAbilityInMapTile.TryGetValue(Find.CurrentMap.Tile, out DetectionEffect res))
                        return "nothing to see";
                    if (res.LastTick != Find.TickManager.TicksGame)
                        return "Vision: 0    Detection:0";
                    else
                        return string.Format("Vision: {0}    Detection:{1}", res.Vision, res.Detection);
                }
                return "nothing to see";
            }
        }
    }
}
