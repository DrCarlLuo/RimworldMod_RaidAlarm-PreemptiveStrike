using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PreemptiveStrike
{
    [StaticConstructorOnStartup]
    static class Textures
    {
        public static readonly Texture2D IconRaidHuman = ContentFinder<Texture2D>.Get("UI/PES_Raid_Human");
        public static readonly Texture2D IconRaidAnimal = ContentFinder<Texture2D>.Get("UI/PES_Raid_Animal");
        public static readonly Texture2D IconHerd = ContentFinder<Texture2D>.Get("UI/PES_AnimalHerd");
        public static readonly Texture2D IconTrader = ContentFinder<Texture2D>.Get("UI/PES_Trader");
        public static readonly Texture2D IconTraveler = ContentFinder<Texture2D>.Get("UI/PES_Traveler");
        public static readonly Texture2D IconUnknown = ContentFinder<Texture2D>.Get("UI/PES_unknown");

        public static readonly Texture2D IconPlusIcon = ContentFinder<Texture2D>.Get("UI/PES_PlusIcon");

        public static readonly Texture2D IconLargeSkyObj_Hostile = ContentFinder<Texture2D>.Get("UI/PES_LargeSkyObj_Hostile");
        public static readonly Texture2D IconLargeSkyObj_Neutral = ContentFinder<Texture2D>.Get("UI/PES_LargeSkyObj_Neutral");
        public static readonly Texture2D IconSmallSkyObj_Hostile = ContentFinder<Texture2D>.Get("UI/PES_SmallSkyObj_Hostile");
        public static readonly Texture2D IconSmallSkyObj_Neutral = ContentFinder<Texture2D>.Get("UI/PES_SmallSkyObj_Neutral");

        public static readonly Texture2D IconInfestation = ContentFinder<Texture2D>.Get("UI/PES_Infestation");

        public static readonly Texture2D IconSolarFlare = ContentFinder<Texture2D>.Get("UI/PES_SolarFlare");

        public static readonly Texture2D IconReportNormal = ContentFinder<Texture2D>.Get("UI/PES_Report_Normal");
        public static readonly Texture2D IconReportWarning = ContentFinder<Texture2D>.Get("UI/PES_Report_Warning");
        public static readonly Texture2D IconReportDanger = ContentFinder<Texture2D>.Get("UI/PES_Report_Danger");

        public static readonly Texture2D IconFriendly_Unknown = ContentFinder<Texture2D>.Get("UI/PES_Friendly_Unknown");

        public static readonly Graphic FenceGraphics = GraphicDatabase.Get<Graphic_Single>("Things/Minor/Watchtower_Fence", ShaderDatabase.Transparent, new Vector2(5, 9.5f), Color.white);
    }
}
