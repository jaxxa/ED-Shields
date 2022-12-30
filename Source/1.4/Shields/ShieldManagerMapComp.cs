using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace Jaxxa.EnhancedDevelopment.Shields.Shields
{
    [StaticConstructorOnStartup]
    class ShieldManagerMapComp : MapComponent
    {

        public static readonly SoundDef HitSoundDef = SoundDef.Named("Shields_HitShield");

        public ShieldManagerMapComp(Map map) : base(map)
        {
            this.map = map;
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
        }

        public bool WillDropPodBeIntercepted(DropPodIncoming dropPodToTest)
        {
            IEnumerable<Building_Shield> _ShieldBuildings = map.listerBuildings.AllBuildingsColonistOfClass<Building_Shield>();
            return _ShieldBuildings.Any(x => x.WillInterceptDropPod(dropPodToTest));
        }

        public bool WillProjectileBeBlocked(Verse.Projectile projectile)
        {

            IEnumerable<Building_Shield> _ShieldBuildings = map.listerBuildings.AllBuildingsColonistOfClass<Building_Shield>();

            Building_Shield _BlockingShield = _ShieldBuildings.FirstOrFallback(x => x.WillProjectileBeBlocked(projectile));

            if (_BlockingShield == null)
            {
                return false;
            }
            _BlockingShield.TakeDamageFromProjectile(projectile);

            FleckMaker.ThrowLightningGlow(projectile.ExactPosition, this.map, 0.5f);
            HitSoundDef.PlayOneShot((SoundInfo)new TargetInfo(projectile.Position, projectile.Map, false));
            projectile.Destroy();
            return true;
        }
              
        public void RecalaculateAll()
        {
            List<Building_Shield> _ShieldBuildings = map.listerBuildings.AllBuildingsColonistOfClass<Building_Shield>().ToList();
            _ShieldBuildings.ForEach(x => x.RecalculateStatistics());
        }

    }
}
