using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.Prometheus.Shields
{
    class CompProperties_ShieldUpgrade : CompProperties
    {

        public CompProperties_ShieldUpgrade()
        {
            this.compClass = typeof(Comp_ShieldUpgrade);
        }

        public int PowerUsage_Increase = 0;
        public int FieldIntegrity_Increase = 0;
        public int Range_Increase = 0;

        public bool IdentifyFriendFoe = false;
        public bool DropPodIntercept = false;
        public bool SIFMode = false;
        public bool SlowDischarge = false;

    }
}
