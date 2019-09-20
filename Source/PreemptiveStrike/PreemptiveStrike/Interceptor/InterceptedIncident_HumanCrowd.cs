using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    abstract class InterceptedIncident_HumanCrowd : InterceptedIncident
    {
        public bool intention_revealed = false;
        public bool faction_revealed = false;
        public bool crowdSize_revealed = false;
        public bool spawnPosition_revealed = false;

        public virtual Faction SourceFaction => parms.faction;
        public virtual int CrowdSize => parms.pawnGroups.Count;
        public virtual IntVec3 SpawnPosition => parms.spawnCenter;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref intention_revealed, "intention_revealed", false, false);
            Scribe_Values.Look<bool>(ref faction_revealed, "faction_revealed", false, false);
            Scribe_Values.Look<bool>(ref crowdSize_revealed, "crowdSize_revealed", false, false);
            Scribe_Values.Look<bool>(ref spawnPosition_revealed, "spawnPosition_revealed", false, false);
        }

        protected virtual void RevealIntention()
        {
            intention_revealed = true;
            Log.Message("Intention Revealed!!!");
        }

        protected virtual void RevealFaction()
        {
            faction_revealed = true;
            Log.Message("faction Revealed!!!");
        }

        protected virtual void RevealSpawnPosition()
        {
            spawnPosition_revealed = true;
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

        public InterceptedIncident_HumanCrowd() { }

        public InterceptedIncident_HumanCrowd(IncidentDef incidentDef, IncidentParms incidentParms) : base(incidentDef, incidentParms) { }
    }
}
