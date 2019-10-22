using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreemptiveStrike.Mod
{
    class PES_Settings : ModSettings
    {
        public static bool DebugModeOn = false;

        public static float DetectionCoefficient = 1f;
        public static int TickForIncidentCaravanCoverOneTile = 2500; //default: 2500 for one hour

        //SkyFaller Ticks
        public static int LargeSkyFallerDuration = 5000;
        public static int LargeSkyFallerIdentificationTime = 2500;
        public static int SmallSkyFallerDuration = 2500;
        public static int SmallSkyFallerIdentificationTime = 1000;

        public static int InfestationDuration = 2500;

        //Make contact chances
        public static float MessageRecieveOffset = 0f;
        public static float BasePersuadeChance_Friendly = 0.3f;
        public static float BasePersuadeChance_Hostile = 0.1f;

        public static float BaseIntimidationFrightenChance_Friendly = 0.21f;
        public static float BaseIntimidationContactChance_Friendly = 0.45f;
        public static float BaseIntimidationFrightenChance_Hostile = 0.11f;
        public static float BaseIntimidationContactChance_Hostile = 0.4f;

        public static float BaseBeguilementFrightenChance_Friendly = 0.9f;
        public static float BaseBeguilementFrightenChance_Hostile = 0.25f;
        public static float BaseBeguilementContactChance_Hostile = 0.25f;

        //Delay raid chances
        public static float BaseDelayPersuasionChance = 0.2f;
        public static float BaseDelayIntimidationSuccessChance = 0.3f;
        public static float BaseDelayIntimidationSmiteChance = 0.3f;
        public static float BaseDelayBeguilementChance = 0.4f;

        public static float BaseBargainPersuasionChance = 0.3f;
        public static float BaseBargainIntimidationSuccessChance = 0.4f;
        public static float BaseBargainIntimidationSmiteChance = 0.4f;
        public static float BaseBargainBeguilementChance = 0.5f;

        public static float BaseRemedyPersuasionChance = 0.3f;
        public static float BaseRemedyIntimidationSuccessChance = 0.4f;
        public static float BaseRemedyIntimidationSmiteChance = 0.4f;
        public static float BaseRemedyBeguilementChance = 0.5f;

        //UI settings
        public static bool TinyUIRectSaved = false;
        public static float tinyUIRect_x;
        public static float tinyUIRect_y;
        public static float tinyUIRect_width;
        public static float tinyUIRect_height;

        public static Rect TinyUIRect
        {
            get
            {
                return new Rect(tinyUIRect_x, tinyUIRect_y, tinyUIRect_width, tinyUIRect_height);
            }
            set
            {
                TinyUIRectSaved = true;
                Rect curRect = new Rect(tinyUIRect_x, tinyUIRect_y, tinyUIRect_width, tinyUIRect_height);
                if (curRect != value)
                {
                    tinyUIRect_x = value.x;
                    tinyUIRect_y = value.y;
                    tinyUIRect_width = value.width;
                    tinyUIRect_height = value.height;
                    if (Scribe.mode == LoadSaveMode.Inactive)
                        PES.Instance.WriteSettings();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DebugModeOn, "DebugModeOn", false);
            Scribe_Values.Look(ref DetectionCoefficient, "DetectionCoefficient", 1f);
            Scribe_Values.Look(ref TickForIncidentCaravanCoverOneTile, "TickForIncidentCaravanCoverOneTile", 2500);
            Scribe_Values.Look(ref LargeSkyFallerDuration, "LargeSkyFallerDuration", 5000);
            Scribe_Values.Look(ref LargeSkyFallerIdentificationTime, "LargeSkyFallerIdentificationTime", 2500);
            Scribe_Values.Look(ref SmallSkyFallerDuration, "SmallSkyFallerDuration", 2500);
            Scribe_Values.Look(ref SmallSkyFallerIdentificationTime, "SmallSkyFallerIdentificationTime", 1000);
            Scribe_Values.Look(ref InfestationDuration, "InfestationDuration", 2500);
            Scribe_Values.Look(ref MessageRecieveOffset, "MessageRecieveOffset", 0f);
            Scribe_Values.Look(ref BasePersuadeChance_Friendly, "BasePersuadeChance_Friendly", 0.3f);
            Scribe_Values.Look(ref BasePersuadeChance_Hostile, "BasePersuadeChance_Hostile", 0.1f);
            Scribe_Values.Look(ref BaseIntimidationFrightenChance_Friendly, "BaseIntimidationFrightenChance_Friendly", 0.21f);
            Scribe_Values.Look(ref BaseIntimidationContactChance_Friendly, "BaseIntimidationContactChance_Friendly", 0.45f);
            Scribe_Values.Look(ref BaseIntimidationFrightenChance_Hostile, "BaseIntimidationFrightenChance_Hostile", 0.11f);
            Scribe_Values.Look(ref BaseIntimidationContactChance_Hostile, "BaseIntimidationContactChance_Hostile", 0.4f);
            Scribe_Values.Look(ref BaseBeguilementFrightenChance_Friendly, "BaseBeguilementFrightenChance_Friendly", 0.9f);
            Scribe_Values.Look(ref BaseBeguilementFrightenChance_Hostile, "BaseBeguilementFrightenChance_Hostile", 0.25f);
            Scribe_Values.Look(ref BaseBeguilementContactChance_Hostile, "BaseBeguilementContactChance_Hostile", 0.25f);
            Scribe_Values.Look(ref BaseDelayPersuasionChance, "BaseDelayPersuasionChance", 0.2f);
            Scribe_Values.Look(ref BaseDelayIntimidationSuccessChance, "BaseDelayIntimidationSuccessChance", 0.3f);
            Scribe_Values.Look(ref BaseDelayIntimidationSmiteChance, "BaseDelayIntimidationSmiteChance", 0.3f);
            Scribe_Values.Look(ref BaseDelayBeguilementChance, "BaseDelayBeguilementChance", 0.4f);
            Scribe_Values.Look(ref BaseBargainPersuasionChance, "BaseBargainPersuasionChance", 0.3f);
            Scribe_Values.Look(ref BaseBargainIntimidationSuccessChance, "BaseBargainIntimidationSuccessChance", 0.4f);
            Scribe_Values.Look(ref BaseBargainIntimidationSmiteChance, "BaseBargainIntimidationSmiteChance", 0.4f);
            Scribe_Values.Look(ref BaseBargainBeguilementChance, "BaseBargainBeguilementChance", 0.5f);
            Scribe_Values.Look(ref BaseRemedyPersuasionChance, "BaseRemedyPersuasionChance", 0.3f);
            Scribe_Values.Look(ref BaseRemedyIntimidationSuccessChance, "BaseRemedyIntimidationSuccessChance", 0.4f);
            Scribe_Values.Look(ref BaseRemedyIntimidationSmiteChance, "BaseRemedyIntimidationSmiteChance", 0.4f);
            Scribe_Values.Look(ref BaseRemedyBeguilementChance, "BaseRemedyBeguilementChance", 0.5f);

            Scribe_Values.Look(ref TinyUIRectSaved, "TinyUIRectSaved",false);
            Scribe_Values.Look(ref tinyUIRect_x, "tinyUIRect_x");
            Scribe_Values.Look(ref tinyUIRect_y, "tinyUIRect_y");
            Scribe_Values.Look(ref tinyUIRect_width, "tinyUIRect_width");
            Scribe_Values.Look(ref tinyUIRect_height, "tinyUIRect_height");
        }

        public static void SetToDefault()
        {
            DebugModeOn = false;
            DetectionCoefficient = 1f;
            TickForIncidentCaravanCoverOneTile = 2500;
            LargeSkyFallerDuration = 5000;
            LargeSkyFallerIdentificationTime = 2500;
            SmallSkyFallerDuration = 2500;
            SmallSkyFallerIdentificationTime = 1000;
            InfestationDuration = 2500;
            MessageRecieveOffset = 0f;
            BasePersuadeChance_Friendly = 0.3f;
            BasePersuadeChance_Hostile = 0.1f;
            BaseIntimidationFrightenChance_Friendly = 0.21f;
            BaseIntimidationContactChance_Friendly = 0.45f;
            BaseIntimidationFrightenChance_Hostile = 0.11f;
            BaseIntimidationContactChance_Hostile = 0.4f;
            BaseBeguilementFrightenChance_Friendly = 0.9f;
            BaseBeguilementFrightenChance_Hostile = 0.25f;
            BaseBeguilementContactChance_Hostile = 0.25f;
            BaseDelayPersuasionChance = 0.2f;
            BaseDelayIntimidationSuccessChance = 0.3f;
            BaseDelayIntimidationSmiteChance = 0.3f;
            BaseDelayBeguilementChance = 0.4f;
            BaseBargainPersuasionChance = 0.3f;
            BaseBargainIntimidationSuccessChance = 0.4f;
            BaseBargainIntimidationSmiteChance = 0.4f;
            BaseBargainBeguilementChance = 0.5f;
            BaseRemedyPersuasionChance = 0.3f;
            BaseRemedyIntimidationSuccessChance = 0.4f;
            BaseRemedyIntimidationSmiteChance = 0.4f;
            BaseRemedyBeguilementChance = 0.5f;
        }

        public static Vector2 ScrollerPos = Vector2.zero;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard ls = new Listing_Standard();
            Rect rect = new Rect(0, 0, 0.9f * inRect.width, 2000f);
            void simpleField<T>(string label, ref T num, float min, float max) where T : struct
            {
                string ss = num.ToString();
                ls.TextFieldNumericLabeled(label.Translate(), ref num, ref ss, min, max);
            }
            ls.BeginScrollView(inRect, ref ScrollerPos, ref rect);

            if (ls.ButtonText("PES_setting_default".Translate()))
                SetToDefault();

            ls.CheckboxLabeled("Debug Mode", ref DebugModeOn, "DONT turn this on plz, this is only for the developer of this mod.");
            ls.Label("PES_setting_notes".Translate());
            
            //coefficient
            ls.GapLine(20f);
            Text.Font = GameFont.Medium;
            ls.Label("PE_setting_detectionMechanism".Translate());
            Text.Font = GameFont.Small;
            ls.Gap(10f);
            ls.Label("PES_setting_detectionFormula".Translate(), -1, "PES_setting_detectionExplain".Translate());
            simpleField("PES_setting_detectioncoefficient", ref DetectionCoefficient, 0f, 10f);

            //moving speed
            string s2 = TickForIncidentCaravanCoverOneTile.ToString();
            ls.TextFieldNumericLabeled("PES_setting_caravanSpeed".Translate(), ref TickForIncidentCaravanCoverOneTile, ref s2, 500, 12500);

            //Sky objects
            ls.Gap(10f);
            ls.Label("PES_setting_skyObject".Translate());
            simpleField("PES_setting_skyObject_Large_a", ref LargeSkyFallerDuration, 500, 12500);
            simpleField("PES_setting_skyObject_Large_b", ref LargeSkyFallerIdentificationTime, 500, 12500);
            simpleField("PES_setting_skyObject_Small_a", ref SmallSkyFallerDuration, 500, 12500);
            simpleField("PES_setting_skyObject_Small_b", ref SmallSkyFallerIdentificationTime, 500, 12500);

            ls.Gap(10f);
            simpleField("PES_setting_infestation", ref InfestationDuration, 500, 12500);

            //Negotiations
            ls.GapLine(20f);
            Text.Font = GameFont.Medium;
            ls.Label("PES_setting_negotiationProb".Translate());
            Text.Font = GameFont.Small;
            ls.Gap(10f);
            ls.Label("PES_setting_negotiationIntro".Translate(), -1, "PES_setting_negotiationTip".Translate());

            ls.Gap(10f);
            ls.Label("PES_setting_negintro".Translate());
            ls.Label("PES_setting_neg_delay".Translate());
            simpleField("PES_setting_delay_persuasion", ref BaseDelayPersuasionChance, 0f, 1f);
            simpleField("PES_setting_delay_intimidation_s", ref BaseDelayIntimidationSuccessChance, 0f, 1f);
            simpleField("PES_setting_delay_intimidation_b", ref BaseDelayIntimidationSmiteChance, 0f, 1f);
            simpleField("PES_setting_delay_beguilment", ref BaseDelayBeguilementChance, 0f, 1f);
            ls.Label("PES_setting_neg_bargain".Translate());
            simpleField("PES_setting_bargain_persuasion", ref BaseBargainPersuasionChance, 0f, 1f);
            simpleField("PES_setting_bargain_intimidation_s", ref BaseBargainIntimidationSuccessChance, 0f, 1f);
            simpleField("PES_setting_bargain_intimidation_b", ref BaseBargainIntimidationSmiteChance, 0f, 1f);
            simpleField("PES_setting_bargain_beguilment", ref BaseBargainBeguilementChance, 0f, 1f);
            ls.Label("PES_setting_neg_remedy".Translate());
            simpleField("PES_setting_remedy_persuasion", ref BaseRemedyPersuasionChance, 0f, 1f);
            simpleField("PES_setting_remedy_intimidation_s", ref BaseRemedyIntimidationSuccessChance, 0f, 1f);
            simpleField("PES_setting_remedy_intimidation_b", ref BaseRemedyIntimidationSmiteChance, 0f, 1f);
            simpleField("PES_setting_remedy_beguilment", ref BaseRemedyBeguilementChance, 0f, 1f);

            ls.Gap(10f);
            ls.Label("PES_setting_commintro".Translate());
            simpleField("PES_setting_commpersuation_friend", ref BasePersuadeChance_Friendly, 0f, 1f);
            simpleField("PES_setting_commpersuation_hostile", ref BasePersuadeChance_Hostile, 0f, 1f);
            simpleField("PES_setting_commintimidation_contact_friend", ref BaseIntimidationFrightenChance_Friendly, 0f, 1f);
            simpleField("PES_setting_commintimidation_frighten_friend", ref BaseIntimidationContactChance_Friendly, 0f, 1f);
            simpleField("PES_setting_commintimidation_contact_Hostile", ref BaseIntimidationFrightenChance_Hostile, 0f, 1f);
            simpleField("PES_setting_commintimidation_frighten_Hostile", ref BaseIntimidationContactChance_Hostile, 0f, 1f);
            simpleField("PES_setting_commbeguilment_friend", ref BaseBeguilementFrightenChance_Friendly, 0f, 1f);
            simpleField("PES_setting_commbeguilment_Contact_hostile", ref BaseBeguilementFrightenChance_Hostile, 0f, 1f);
            simpleField("PES_setting_commbeguildment_frighten_hostile", ref BaseBeguilementContactChance_Hostile, 0f, 1f);
            ls.Label("PES_setting_mesintro".Translate());
            ls.Label("PES_setting_mesExp".Translate(), -1, "PES_setting_megTip".Translate());
            simpleField("PES_setting_mesOffset", ref MessageRecieveOffset, 0f, 1f);

            ls.EndScrollView(ref rect);
        }


    }
}
