using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace PreemptiveStrike
{
    static class PES_NameGenerator
    {
        public static string ManhunterName(string animalName)
        {
            int theChosenOne = new IntRange(1, 4).RandomInRange;
            return ("PES_ManHunterName_" + theChosenOne).Translate(animalName);
        }

        public static string FarmAnimalName(string animalName)
            => "PES_FarmAnimal_1".Translate(animalName);

        public static string MigrationAnimalName(string animalName)
            => "PES_MigrationName_1".Translate(animalName);

        public static string ThrumboName()
            => "PES_Thrumboname_1".Translate();

        public static string TravelerName(string factionName)
            => "PES_TravelerName_1".Translate(factionName);

        public static string VisitorName(string factionName)
            => "PES_VisitorName_1".Translate(factionName);

        public static string TraderName(string factionName)
            => "PES_TraderName_1".Translate(factionName);

        public static string ArmyName()
        {
            return NameGenerator.GenerateName(PESDefOf.PES_NamerArmy, new List<string>());
        }

        private static Random random = new Random();

        public static string MechArmyName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string Lettercode = new string(Enumerable.Repeat(chars, 2).Select(s => s[random.Next(s.Length)]).ToArray());
            string code = string.Format("{0}-{1}", Lettercode, new IntRange(0, 99).RandomInRange.ToString());

            int theChosenOne = new IntRange(1, 5).RandomInRange;
            return ("PES_MechName_" + theChosenOne).Translate(code);
        }
    }
}
