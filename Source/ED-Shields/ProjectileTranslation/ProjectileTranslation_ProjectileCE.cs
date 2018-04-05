using CombatExtended;
using EnhancedDevelopment.Shields.Basic.ShieldUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Shields.Basic.ProjectileTranslation
{
    class ProjectileTranslation_ProjectileCE : ProjectileTranslation
    {
        CombatExtended.ProjectileCE m_Projectile = null;

        static public ProjectileTranslation GetSpecificTranslator(Thing projectileThing)
        {
            if (projectileThing is ProjectileCE)
            {
                ProjectileCE pr = projectileThing as ProjectileCE;
                if (pr != null)
                {
                    ProjectileTranslation_ProjectileCE _Translator = new ProjectileTranslation_ProjectileCE();
                    _Translator.m_Projectile = pr;
                    return _Translator;
                }
            }
            return null;

        }

        public override int DamageAmountBase()
        {
            Log.Message("DamangeCE");
            return this.m_Projectile.def.projectile.damageAmountBase;
        }

        public override bool Destroyed()
        {
            return this.m_Projectile.Destroyed;
        }

        public override Vector3 ExactPosition()
        {
            return this.m_Projectile.ExactPosition;
        }

        public override Quaternion ExactRotation()
        {
            return this.m_Projectile.ExactRotation;
        }

        public override bool FlyOverhead()
        {
            return this.m_Projectile.def.projectile.flyOverhead;
        }

        public override Thing Launcher()
        {
            return ReflectionHelper.GetInstanceField(typeof(ProjectileCE), this.m_Projectile, "launcher") as Thing;
        }

        public override Thing ProjectileThing()
        {
            return this.m_Projectile;
        }

        public override Vector3 TargetLocation()
        {
            throw new NotImplementedException();
        }

        public override bool WillTargetLandInRange(int fieldRadius)
        {
            throw new NotImplementedException();
        }
    }


    class ProjectileTranslationGenerator_ProjectileCE : ProjectileTranslationGenerator
    {
        public override ProjectileTranslation GetTranslator(Thing projectileThing)
        {
            return ProjectileTranslation_ProjectileCE.GetSpecificTranslator(projectileThing);
        }
    }
}
