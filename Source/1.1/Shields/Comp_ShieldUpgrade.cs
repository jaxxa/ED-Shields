using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace EnhancedDevelopment.Prometheus.Shields
{
    class Comp_ShieldUpgrade : ThingComp
    {

        public CompProperties_ShieldUpgrade Properties;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            this.Properties = ((CompProperties_ShieldUpgrade)this.props);
            
            this.parent.Map.GetComponent<ShieldManagerMapComp>().RecalaculateAll();

        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            map.GetComponent<ShieldManagerMapComp>().RecalaculateAll();

        }
        
    }
}
