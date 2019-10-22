using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.DetectionSystem;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.UI
{
    class TinyReportWindow : Window
    {
        public static TinyReportWindow Instance { get; } = new TinyReportWindow();

        private TinyReportWindow()
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
            //windowRect = new Rect(Verse.UI.screenWidth - UIConstants.DefualtWindowPin2RightIntend - UIConstants.DefaultWindowWidth, 100f, UIConstants.DefaultWindowWidth, UIConstants.TitleHeight + UIConstants.TitleIntend + 35f);
            //windowRect = new Rect(Verse.UI.screenWidth - UIConstants.TinyWindowSize - UIConstants.TinyWindowPinIntend, 100f, UIConstants.TinyWindowSize, UIConstants.TinyWindowSize);
            SetInitialSizeAndPosition();
        }

        protected override void SetInitialSizeAndPosition()
        {
            if(PES_Settings.TinyUIRectSaved)
            {
                windowRect = PES_Settings.TinyUIRect;
            }
            else
            {
                windowRect = PES_Settings.TinyUIRect = new Rect(Verse.UI.screenWidth - UIConstants.TinyWindowSize - UIConstants.TinyWindowPinIntend, 100f, UIConstants.TinyWindowSize, UIConstants.TinyWindowSize);
            }
            windowRect.width = UIConstants.TinyWindowSize;
            windowRect.height = UIConstants.TinyWindowSize;
        }

        protected override float Margin => 0f;

        public override void DoWindowContents(Rect inRect)
        {
            //float Offsetx = (UIConstants.TinyWindowSize - inRect.width) / 2;
            //float Offsety = (UIConstants.TinyWindowSize - inRect.height) / 2;
            //Rect rect = new Rect(inRect.x - Offsetx, inRect.y - Offsety,UIConstants.TinyWindowSize,UIConstants.TinyWindowSize);
            //windowRect = new Rect(Verse.UI.screenWidth - UIConstants.TinyWindowSize - UIConstants.TinyWindowPinIntend, windowRect.y, windowRect.width, windowRect.height);
            MakeItInScreen();
            PES_Settings.TinyUIRect = windowRect;

            float picx = 46 * 82 / 110f;
            float picy = 46;
            Rect rect = new Rect((UIConstants.TinyWindowSize - picx) / 2, (UIConstants.TinyWindowSize - picy) / 2, picx, picy);
            Texture2D tex;
            if (ColonySecurityDashBoard_Window.Instance.dangerNum > 0)
                tex = Textures.IconReportDanger;
            else if (ColonySecurityDashBoard_Window.Instance.uidNum > 0)
                tex = Textures.IconReportWarning;
            else
                tex = Textures.IconReportNormal;
            if(Widgets.ButtonImage(rect, tex))
            {
                if (ColonySecurityDashBoard_Window.IsOpening)
                    ColonySecurityDashBoard_Window.CloseIt();
                else
                    ColonySecurityDashBoard_Window.OpenIt();
            }
        }

        private void MakeItInScreen()
        {
            float curWidth = Instance.windowRect.width;
            float curHeight = Instance.windowRect.height;
            if (Instance.windowRect.x + curWidth >= Verse.UI.screenWidth)
                Instance.windowRect.x = Verse.UI.screenWidth - curWidth;
            if (Instance.windowRect.x < 0) Instance.windowRect.x = 0;
            if (Instance.windowRect.y + curHeight >= Verse.UI.screenHeight)
                Instance.windowRect.y = Verse.UI.screenHeight - curHeight;
            if (Instance.windowRect.y < 0) Instance.windowRect.y = 0;
        }
    }
}
