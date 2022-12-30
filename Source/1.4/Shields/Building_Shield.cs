using RimWorld;
using Verse;

namespace Jaxxa.EnhancedDevelopment.Shields.Shields
{
    [StaticConstructorOnStartup]
    public class Building_Shield : Building
    {

        #region Methods

        public override string GetInspectString()
        {
            return this.GetComp<Comp_ShieldGenerator>().CompInspectStringExtra();
        }

        public bool WillInterceptDropPod(DropPodIncoming dropPodToCheck)
        {
            return this.GetComp<Comp_ShieldGenerator>().WillInterceptDropPod(dropPodToCheck);
        }

        public bool WillProjectileBeBlocked(Projectile projectileToCheck)
        {
            return this.GetComp<Comp_ShieldGenerator>().WillProjectileBeBlocked(projectileToCheck);
        }

        public void TakeDamageFromProjectile(Projectile projectile)
        {
            this.GetComp<Comp_ShieldGenerator>().FieldIntegrity_Current -= projectile.DamageAmount;
        }

        public void RecalculateStatistics()
        {
            this.GetComp<Comp_ShieldGenerator>().RecalculateStatistics();
        }               
        
        #endregion //Methods

    }
}