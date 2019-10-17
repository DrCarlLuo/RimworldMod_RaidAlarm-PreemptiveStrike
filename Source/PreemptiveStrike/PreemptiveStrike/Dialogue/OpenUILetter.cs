using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PreemptiveStrike.UI;
using PreemptiveStrike.IncidentCaravan;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Dialogue
{
    class OpenUILetter : ChoiceLetter
    {
        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                DiaOption option = new DiaOption("PES_UI_Letter_ReadMore".Translate());
                option.action = () =>
                {
                    ColonySecurityDashBoard_Window.OpenIt();
                    Find.LetterStack.RemoveLetter(this);
                };
                option.resolveTree = true;
                yield return option;
                yield return base.Option_Close;
                yield break;
            }
        }

        public static void Make(string label, string text, LetterDef def)
        {
            var lclass = def.letterClass;
            def.letterClass = typeof(OpenUILetter);
            ChoiceLetter Letter = LetterMaker.MakeLetter(label, text, def);
            def.letterClass = lclass;
            Find.LetterStack.ReceiveLetter(Letter);
        }
    }

    class SparkUILetter : ChoiceLetter
    {
        public TravelingIncidentCaravan caravan;
        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                DiaOption option = new DiaOption("PES_UI_Letter_ReadMore".Translate());
                option.action = () =>
                {
                    if (caravan != null)
                        ColonySecurityDashBoard_Window.DoSparkWithBulletin(caravan);
                    ColonySecurityDashBoard_Window.OpenIt();
                    Find.LetterStack.RemoveLetter(this);
                };
                option.resolveTree = true;
                yield return option;
                yield return base.Option_Close;
                yield break;
            }
        }

        public static void Make(string label, string text, LetterDef def, TravelingIncidentCaravan caravan)
        {
            var lclass = def.letterClass;
            def.letterClass = typeof(SparkUILetter);
            ChoiceLetter Letter = LetterMaker.MakeLetter(label, text, def);
            def.letterClass = lclass;
            if (Letter is SparkUILetter sLetter)
                sLetter.caravan = caravan;
            Find.LetterStack.ReceiveLetter(Letter);
        }
    }
}
