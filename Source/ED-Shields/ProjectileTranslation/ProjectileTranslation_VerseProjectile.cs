using EnhancedDevelopment.Shields.Basic.ShieldUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Shields.Basic.ProjectileTranslation
{
    class ProjectileTranslation_VerseProjectile : ProjectileTranslation
    {

        Verse.Projectile m_Projectile = null;

        static public ProjectileTranslation GetSpecificTranslator(Thing projectileThing)
        {
            if (projectileThing is Projectile)
            {
                Projectile pr = (Projectile)projectileThing;
                if (pr != null)
                {
                    ProjectileTranslation_VerseProjectile _Translator = new ProjectileTranslation_VerseProjectile();
                    _Translator.m_Projectile = pr;
                    return _Translator;
                }
            }
            return null;

        }

        public override int DamageAmountBase()
        {
            Log.Message("Damange");
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
            return ReflectionHelper.GetInstanceField(typeof(Projectile), this.m_Projectile, "launcher") as Thing;
        }

        public override Thing ProjectileThing()
        {
            return this.m_Projectile;
        }

        public override bool WillTargetLandInRange(int fieldRadius)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Checks if the projectile will land within the shield or pass over.
        /// </summary>
        /// <param name="projectile">The specific projectile that is being checked</param>
        /// <returns>True if the projectile will land close, false if it will be far away.</returns>
        private static bool WillTargetLandInRange(ProjectileTranslation translation, int fieldRadius)
        {
            Vector3 targetLocation = translation.TargetLocation();

            if (Vector3.Distance(translation.ExactPosition(), targetLocation) > fieldRadius)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override Vector3 TargetLocation()
        {
            FieldInfo fieldInfo = this.m_Projectile.GetType().GetField("destination", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            Vector3 reoveredVector = (Vector3)fieldInfo.GetValue(this.m_Projectile);
            return reoveredVector;
        }
    }

    class ProjectileTranslationGenerator_VerseProjectile : ProjectileTranslationGenerator
    {
        public override ProjectileTranslation GetTranslator(Thing projectileThing)
        {
            return ProjectileTranslation_VerseProjectile.GetSpecificTranslator(projectileThing);
        }
    }
}
