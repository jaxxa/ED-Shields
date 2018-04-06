using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.Shields.Basic
{
    class ShieldManagerMapComp : MapComponent
    {

        private List<Projectile> m_Projectiles = new List<Projectile>();

        public ShieldManagerMapComp(Map map) : base(map)
        {
         //   map.components.<ShieldManagerMapComp>();
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            Log.Message("MapCompTick");
        }
    }
}
