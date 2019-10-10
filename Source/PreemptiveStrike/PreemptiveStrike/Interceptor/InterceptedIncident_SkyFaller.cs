using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PreemptiveStrike.Mod;
using RimWorld;
using Verse;

namespace PreemptiveStrike.Interceptor
{
    enum SkyFallerType
    {
        Big,
        Small
    }

    abstract class InterceptedIncident_SkyFaller : InterceptedIncident
    {
        public abstract SkyFallerType FallerType { get; }

        public override string IncidentTitle_Unknow
        {
            get
            {
                if (FallerType == SkyFallerType.Big)
                    return "PES_Unknown_SkyObject_Large".Translate();
                return "PES_Unknown_SkyObject_Small".Translate();
            }
        }

        public override string IntentionStr => "";

        public override IncidentIntelLevel IntelLevel
        {
            get
            {
                if (revealed)
                {
                    return IsHostileToPlayer ? IncidentIntelLevel.Danger : IncidentIntelLevel.Neutral;
                }
                return IncidentIntelLevel.Unknown;
            }
        }

        public abstract void confirmMessage();

        public virtual void detectMessage()
        {
            if (FallerType == SkyFallerType.Big)
                Messages.Message("PES_Spot_SkyFaller_Big".Translate(), MessageTypeDefOf.NeutralEvent);
            else
                Messages.Message("PES_Spot_SkyFaller_Small".Translate(), MessageTypeDefOf.NeutralEvent);
        }

        public bool revealed;
        public virtual void Reveal()
        {
            revealed = true;
            EventManger.NotifyCaravanListChange?.Invoke();
            parentCaravan.TryNotifyCaravanIntel();
            if (PES_Settings.DebugModeOn)
                Log.Message("SkyFaller Revealed!!!");
        }

        public override void RevealAllInformation()
        {
            if (!revealed)
                Reveal();
        }

        public sealed override void RevealRandomInformation()
        {
            throw new Exception("Should be called with sky faller");
        }

        public sealed override void RevealInformationWhenCommunicationEstablished()
        {
            throw new Exception("Should be called with sky faller");
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref revealed, "revealed", false);
        }

        public abstract bool PreCalculateDroppingSpot();
    }
}
