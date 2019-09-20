using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.IncidentCaravan;

namespace PreemptiveStrike.Interceptor
{
    [StaticConstructorOnStartup]
    class IncidentInterceptorUtility
    {
        //Intercepting Switches(Used in harmony Patches)
        public static bool IsIntercepting_IncidentExcecution;
        public static bool IsIntercepting_PawnGeneration;
        public static bool IsIntercepting_PawnArrivalWorker;

        public static List<Pawn> tmpPawnList;

        static IncidentInterceptorUtility()
        {
            IsIntercepting_IncidentExcecution = true;
            IsIntercepting_PawnGeneration = false;
        }

        //TODO: SHOULD CHECK IF IT IS ALLIED RAID!!!
        public static bool Intercept_Raid_EdgeWalkIn(IncidentParms parms)
        {
            InterceptedIncident incident = new InterceptedIncident_HumanCrowd_RaidEnemy(parms);
            if (!IncidentCaravanUtility.AddNewIncidentCaravan(incident))
            {
                Log.Error("Fail to create Incident Caravan");
                return false;
            }
            return true;
        }
    }
}
