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
        public static readonly Texture2D IconSword = ContentFinder<Texture2D>.Get("UI/swords");

        public static readonly Texture2D IconUnknown = ContentFinder<Texture2D>.Get("UI/unknown");

        public static readonly Texture2D IconAnimal = ContentFinder<Texture2D>.Get("UI/animals");

        public static readonly Texture2D IconMerchant = ContentFinder<Texture2D>.Get("UI/Merchant");

    }
}
