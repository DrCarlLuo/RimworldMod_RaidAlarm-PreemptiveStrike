using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using PreemptiveStrike.IncidentCaravan;
using PreemptiveStrike.DetectionSystem;
using System.Reflection;

namespace PreemptiveStrike.UI
{
    [StaticConstructorOnStartup]
    class ColonySecurityDashBoard_Window : Window
    {
        public static ColonySecurityDashBoard_Window Instance { get; } = new ColonySecurityDashBoard_Window();
        public static bool IsOpening = false;
        public static void OpenIt()
        {
            if (IsOpening) return;
            Find.WindowStack.Add(Instance);
            IsOpening = true;
            ReCalulateSize();
        }

        public static void CloseIt()
        {
            if (!IsOpening) return;
            Instance.Close();
            IsOpening = false;
        }

        public static List<Bulletin> BulletinCache = new List<Bulletin>();

        public static void Recache()
        {
            BulletinCache.Clear();
            foreach (var x in IncidentCaravanUtility.IncidentCaravans)
            {
                if (x.detected)
                {
                    BulletinCache.Add(Bulletin.Create(x));
                }
            }

            //----- This code is to help player remove some corrupted information in the save caused by bugs in the last version
            QueuedIncident toremove = null;
            foreach (QueuedIncident qi in Find.Storyteller.incidentQueue)
            {
                if (qi.FiringIncident.def == IncidentDefOf.SolarFlare)
                {
                    toremove = qi;
                    break;                    
                }
            }
            if(toremove != null)
            {
                List<QueuedIncident> tmpQue = typeof(IncidentQueue).GetField("queuedIncidents", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Find.Storyteller.incidentQueue) as List<QueuedIncident>;
                if (tmpQue != null)
                    tmpQue.Remove(toremove);
            }
            //-------

            BulletinCache.Sort();

            int danger = 0, neutral = 0, uid = 0;
            foreach (var x in BulletinCache)
            {
                if (x.bulletinIntelLevel == Interceptor.IncidentIntelLevel.Danger)
                    ++danger;
                else if (x.bulletinIntelLevel == Interceptor.IncidentIntelLevel.Neutral)
                    ++neutral;
                else
                    ++uid;
            }
            if (danger > Instance.dangerNum)
                OpenIt();
            Instance.dangerNum = danger;
            Instance.neutralNum = neutral;
            Instance.uidNum = uid;
            ReCalulateSize();
        }

        public static void DoSparkWithBulletin(TravelingIncidentCaravan caravan)
        {
            foreach (var b in BulletinCache)
            {
                if (b.Caravan == caravan)
                {
                    b.BeginSpark(3);
                    return;
                }
            }
        }

        //private bool minified = true;
        //public bool Minified
        //{
        //    get { return minified; }
        //    set
        //    {
        //        bool changed = minified != value;
        //        minified = value;
        //        ReCalulateSize(changed);
        //    }
        //}

        public int dangerNum;
        public int neutralNum;
        public int uidNum;
        //private WindowResizer Resizer = new WindowResizer();

        public static void ReCalulateSize(bool changeMinified = false)
        {
            float x = TinyReportWindow.Instance.windowRect.x - UIConstants.DefaultWindowWidth;
            float y = TinyReportWindow.Instance.windowRect.y;
            float height = (UIConstants.BulletinHeight + UIConstants.BulletinIntend) * Math.Min(BulletinCache.Count, 3);
            Instance.windowRect = new Rect(x, y, UIConstants.DefaultWindowWidth, UIConstants.TitleHeight + UIConstants.TitleIntend + height + 35f);
            //if (Instance.Minified)
            //{
            //    if(changeMinified)
            //        x += UIConstants.DiffOfOrgAndMinify;
            //    Instance.windowRect = new Rect(x, y, UIConstants.MinifiedWindowWidth, 105f);
            //}
            //else
            //{
            //    if (changeMinified)
            //        x -= UIConstants.DiffOfOrgAndMinify;
            //    float height = (UIConstants.BulletinHeight + UIConstants.BulletinIntend) * Math.Min(BulletinCache.Count, 3);
            //    Instance.windowRect = new Rect(x, y, UIConstants.DefaultWindowWidth, UIConstants.TitleHeight + UIConstants.TitleIntend + height + 35f);
            //}
            //float curWidth = Instance.windowRect.width;
            //float curHeight = Instance.windowRect.height;
            //if (Instance.windowRect.x + curWidth >= Verse.UI.screenWidth)
            //    Instance.windowRect.x = Verse.UI.screenWidth - curWidth;
            //if (Instance.windowRect.x < 0) Instance.windowRect.x = 0;
            //if (Instance.windowRect.y + curHeight >= Verse.UI.screenHeight)
            //    Instance.windowRect.y = Verse.UI.screenHeight - curHeight;
            //if (Instance.windowRect.y < 0) Instance.windowRect.y = 0;
        }

        private ColonySecurityDashBoard_Window()
        {
            this.preventCameraMotion = false;
            this.closeOnCancel = false;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = false;
            this.focusWhenOpened = false;
            this.draggable = false;
            this.resizeable = false;
            this.layer = WindowLayer.GameUI;
            this.soundAppear = null;
            this.soundClose = null;
            this.doCloseX = false;
            this.doCloseButton = false;
            //windowRect = new Rect(Verse.UI.screenWidth - UIConstants.DefualtWindowPin2RightIntend - UIConstants.DefaultWindowWidth, 100f, UIConstants.DefaultWindowWidth, UIConstants.TitleHeight + UIConstants.TitleIntend + 35f);
            windowRect = new Rect(Verse.UI.screenWidth - UIConstants.DefualtWindowPin2RightIntend - 300f, 100f, UIConstants.DefaultWindowWidth, UIConstants.TitleHeight + UIConstants.TitleIntend + 35f);
            dangerNum = neutralNum = uidNum = 0;
            //minified = false;
            //this.Resizer.minWindowSize.x = 200.0f;
            //this.Resizer.minWindowSize.y = 100.0f;
            //this.windowRect = new Rect(Verse.UI.screenWidth - 435f, 100f, 420f, 420f);
            IsOpening = true;
        }

        static ColonySecurityDashBoard_Window()
        {
            EventManger.NotifyCaravanListChange += Recache;
            IsOpening = false;
        }

        protected override void SetInitialSizeAndPosition()
        {
            ReCalulateSize();
        }

        private Vector2 scrollViewVec = Vector2.zero;

        public override void DoWindowContents(Rect inRect)
        {
            //if (Minified)
            //{
            //    DoMinifiedContent(inRect);
            //    return;
            //}
            int bulletinNum = BulletinCache.Count;
            GUI.Label(new Rect(5f, -2.5f, 150f, 25f), "PES_UI_Title".Translate());
            //if (Widgets.ButtonImage(new Rect(380f, 5f, 20f, 5f), BaseContent.WhiteTex))
            //Minified = true;
            GUI.DrawTexture(new Rect(2.5f, 25f, UIConstants.BulletinWidth + 30f, 1f), BaseContent.GreyTex);

            float scrollHeight = (UIConstants.BulletinHeight + UIConstants.BulletinIntend) * Math.Min(3, bulletinNum);
            scrollViewVec = GUI.BeginScrollView(new Rect(5f, 30f, UIConstants.BulletinWidth + 20f, scrollHeight), scrollViewVec, new Rect(0f, 0f, UIConstants.BulletinWidth, BulletinCache.Count * (UIConstants.BulletinHeight + UIConstants.BulletinIntend)));

            float bulletinY = 0f;
            for (int i = 0; i < BulletinCache.Count; ++i)
            {
                var bulletin = BulletinCache[i];
                bulletin.OnDraw(0f, bulletinY);
                if (i != BulletinCache.Count - 1)
                {
                    Color color = GUI.color;
                    GUI.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                    GUI.DrawTexture(new Rect(10f, bulletinY + UIConstants.BulletinHeight + 2.5f, UIConstants.BulletinWidth, 1f), BaseContent.WhiteTex);
                    GUI.color = color;
                }
                bulletinY += UIConstants.BulletinHeight + 5f;
            }

            GUI.EndScrollView();
            GUI.Label(new Rect(10f, 30f + scrollHeight + 0f, 400f, 25f), DetectionAbility);
        }

        //public void DoMinifiedContent(Rect inRect)
        //{
        //    int bulletinNum = BulletinCache.Count;
        //    Rect rect = new Rect(0f, 0f, 300f, 28f);
        //    if (Widgets.ButtonInvisible(rect))
        //    {
        //        if (BulletinCache.Count > 0)
        //            Minified = false;
        //    }
        //    Widgets.DrawHighlightIfMouseover(rect);
        //    Text.Font = GameFont.Medium;
        //    Widgets.Label(new Rect(0f, 0f, 250f, 30f), string.Format(@"<b>{0}</b>", "PES_UI_Title".Translate()));
        //    if (bulletinNum != 0)
        //        GUI.DrawTexture(new Rect(225f, 0f, 25f, 25f), Textures.IconPlusIcon);
        //    GUI.DrawTexture(new Rect(0, 30f, 250f, 1f), BaseContent.GreyTex);
        //    Text.Font = GameFont.Tiny;
        //    Widgets.Label(new Rect(0f, 32f, 300f, 20f), DetectionAbility);
        //    Widgets.Label(new Rect(0f, 52f, 300f, 20f), BulltinBrief);
        //}

        private string DetectionAbility
        {
            get
            {
                int VisionRange = 0, DetectionRange = 0;
                if (Find.CurrentMap != null)
                {
                    if (!DetectDangerUtilities.DetectionAbilityInMapTile.TryGetValue(Find.CurrentMap.Tile, out DetectionEffect res))
                        VisionRange = DetectionRange = 0;
                    if (res.LastTick != Find.TickManager.TicksGame)
                        VisionRange = DetectionRange = 0;
                    else
                    {
                        VisionRange = res.Vision;
                        DetectionRange = res.Detection;
                    }
                    return "PES_UI_Ranges".Translate(
                        ChangeColorZero(VisionRange, "white", "brown"),
                        ChangeColorZero(DetectionRange, "white", "brown")
                        );
                }
                return "";
            }
        }

        private string BulltinBrief
        {
            get
            {
                return "PES_UI_Bulletins".Translate(
                    ChangeColorZero(dangerNum, "red"),
                    ChangeColorZero(neutralNum, "cyan"),
                    ChangeColorZero(uidNum, "lime")
                );
            }
        }

        public string ChangeColorZero(int num, string normalColor = "white", string zeroColor = "white")
        {
            return string.Format(@"<color={0}>{1}</color>", num == 0 ? zeroColor : normalColor, num.ToString());
        }
    }
}
