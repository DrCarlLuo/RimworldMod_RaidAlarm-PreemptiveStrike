using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using PreemptiveStrike.Mod;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    abstract class InterceptedIncident_HumanCrowd : InterceptedIncident
    {
        public bool intention_revealed = false;
        public bool faction_revealed = false;
        public bool crowdSize_revealed = false;
        public bool spawnPosition_revealed = false;
        public List<Pawn> pawnList;

        public virtual Faction SourceFaction => parms.faction;
        public virtual int CrowdSize => pawnList != null ? pawnList.Count : 0;
        public virtual IntVec3 SpawnPosition => parms.spawnCenter;

        public override string IncidentTitle_Unknow
        {
            get
            {
                return "PES_Unknown_HumanCrowd".Translate();
            }
        }

        public override IncidentIntelLevel IntelLevel
        {
            get
            {
                if(faction_revealed|| intention_revealed)
                    return this.IsHostileToPlayer ? IncidentIntelLevel.Danger : IncidentIntelLevel.Neutral;
                return IncidentIntelLevel.Unknown;
            }
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref intention_revealed, "intention_revealed", false, false);
            Scribe_Values.Look(ref faction_revealed, "faction_revealed", false, false);
            Scribe_Values.Look(ref crowdSize_revealed, "crowdSize_revealed", false, false);
            Scribe_Values.Look(ref spawnPosition_revealed, "spawnPosition_revealed", false, false);
            Scribe_Collections.Look(ref pawnList, "pawnList", LookMode.Deep);
        }

        protected virtual void RevealIntention()
        {
            intention_revealed = true;
            EventManger.NotifyCaravanListChange?.Invoke();
            if (PES_Settings.DebugModeOn)
                Log.Message("Intention Revealed!!!");
        }

        protected virtual void RevealFaction()
        {
            faction_revealed = true;
            EventManger.NotifyCaravanListChange?.Invoke();
            if (PES_Settings.DebugModeOn)
                Log.Message("faction Revealed!!!");
        }

        protected virtual void RevealSpawnPosition()
        {
            spawnPosition_revealed = true;
            if (PES_Settings.DebugModeOn)
                Log.Message("SpawnPosition Revealed!!!");
        }

        protected abstract void RevealCrowdSize();

        public override void RevealAllInformation()
        {
            if (!intention_revealed)
                RevealIntention();
            if (!faction_revealed)
                RevealFaction();
            if (!spawnPosition_revealed)
                RevealSpawnPosition();
            if (!crowdSize_revealed)
                RevealCrowdSize();
        }

    }
}
