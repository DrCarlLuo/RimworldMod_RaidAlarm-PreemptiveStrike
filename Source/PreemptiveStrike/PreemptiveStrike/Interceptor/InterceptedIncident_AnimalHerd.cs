using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PreemptiveStrike.Mod;

namespace PreemptiveStrike.Interceptor
{
    abstract class InterceptedIncident_AnimalHerd : InterceptedIncident
    {
        public bool intention_revealed = false;
        public bool spawnPosition_revealed = false;
        public bool animalType_revealed = false;
        public bool animalNum_revealed = false;

        public PawnKindDef AnimalType;
        public int AnimalNum;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref intention_revealed, "intention_revealed", false, false);
            Scribe_Values.Look<bool>(ref animalType_revealed, "animalType_revealed", false, false);
            Scribe_Values.Look<bool>(ref animalNum_revealed, "animalNum_revealed", false, false);
            Scribe_Values.Look<bool>(ref spawnPosition_revealed, "spawnPosition_revealed", false, false);
            Scribe_Values.Look<int>(ref AnimalNum, "AnimalNum", 0, false);
            Scribe_Defs.Look<PawnKindDef>(ref AnimalType, "AnimalType");
        }

        protected virtual void RevealIntention()
        {
            intention_revealed = true;
            PreemptiveStrike.UI.ColonySecurityDashBoard_Window.Recache();

            if (PES_Settings.DebugModeOn)
                Log.Message("Intention Revealed!!!");
        }

        protected virtual void RevealSpawnPosition()
        {
            spawnPosition_revealed = true;
            if (PES_Settings.DebugModeOn)
                Log.Message("SpawnPosition Revealed!!!");
        }

        protected virtual void RevealAnimalType()
        {
            animalType_revealed = true;
            if (PES_Settings.DebugModeOn)
                Log.Message("AnimalType Revealed: " + AnimalType.label);
        }

        protected virtual void RevealAnimalNum()
        {
            animalNum_revealed = true;
            if (PES_Settings.DebugModeOn)
                Log.Message("AnimalNum Revealed: " + AnimalNum);
        }

        public override void RevealAllInformation()
        {
            if (!intention_revealed)
                RevealIntention();
            if (!spawnPosition_revealed)
                RevealSpawnPosition();
            if (!animalType_revealed)
                RevealAnimalType();
            if (!animalNum_revealed)
                RevealAnimalNum();
        }

        public override void RevealRandomInformation()
        {
            List<Action> availables = new List<Action>();
            if (!intention_revealed)
                availables.Add(RevealIntention);
            if (!spawnPosition_revealed)
                availables.Add(RevealSpawnPosition);
            if (!animalType_revealed)
                availables.Add(RevealAnimalType);
            if (!animalNum_revealed)
                availables.Add(RevealAnimalNum);
            if (availables.Count != 0)
            {
                Action OneToReveal = availables.RandomElement<Action>();
                OneToReveal();
            }
        }

        protected abstract void SetInterceptFlag(WorkerPatchType value);

        public override void ExecuteNow()
        {
            SetInterceptFlag(WorkerPatchType.Substitution);
            IncidentInterceptorUtility.tmpIncident = this;

            if (this.incidentDef != null && this.parms != null)
                this.incidentDef.Worker.TryExecute(this.parms);
            else
                Log.Error("No IncidentDef or parms in InterceptedIncident!");

            IncidentInterceptorUtility.tmpIncident = null;
            SetInterceptFlag(WorkerPatchType.Forestall);
        }
    }
}
