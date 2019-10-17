using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PreemptiveStrike.UI
{
    static class UIConstants
    {
        public static readonly float MainLabelHeight = 30f;
        public static readonly float TinyLabelHeight = 20f;//17
        public static readonly float MainLabelIntend = 0f;
        public static readonly float TinyLabelIntend = -1f;

        public static readonly float BulletinWidth = 400f;
        public static readonly float BulletinHeight = MainLabelHeight + MainLabelIntend + TinyLabelHeight * 3 + TinyLabelIntend * 2;
        public static readonly float BulletinIntend = 5f;

        public static readonly float BulletinIconIntend = 10f;
        public static readonly float BulletinIconSize = BulletinHeight - BulletinIconIntend * 2;

        public static readonly float DefualtWindowPin2RightIntend = 35f;
        public static readonly float DefaultWindowWidth = BulletinWidth + 50f;
        public static readonly float TitleHeight = 30f;
        public static readonly float TitleIntend = 20f;

        public static readonly float MinifiedWindowWidth = 300f;
        public static readonly float DiffOfOrgAndMinify = DefaultWindowWidth - MinifiedWindowWidth;

        public static readonly float TinyWindowSize = 50f;
        public static readonly float TinyWindowPinIntend = 5f;
    }
}
