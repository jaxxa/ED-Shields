using Verse;

namespace Jaxxa.EnhancedDevelopment.Shields.Shields
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
