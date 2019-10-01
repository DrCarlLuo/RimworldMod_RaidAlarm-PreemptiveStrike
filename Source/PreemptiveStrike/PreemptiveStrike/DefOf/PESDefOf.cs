using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace PreemptiveStrike
{
    [DefOf]
    static class PESDefOf
    {
        static PESDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PESDefOf));
        }

        public static ThingDef PES_PremitiveWatchtower;

        public static ThingDef PES_watchtower;

        public static ThingDef PES_SentryDroneArray;

        public static JobDef PES_StandGuard_Primitive;
        public static JobDef PES_StandGuard;
        public static JobDef PES_OperateSentryArray;
        public static JobDef PES_Job_InstallUpgrade;

        public static DesignationDef PES_InstallUpgrade;

        public static WeatherDef Fog;
        public static WeatherDef Rain;
        public static WeatherDef RainyThunderstorm;
        public static WeatherDef FoggyRain;
        public static WeatherDef SnowHard;

        public static HediffDef PES_CombatFervor;
        public static HediffDef PES_CombatTiredness;
    }
}
