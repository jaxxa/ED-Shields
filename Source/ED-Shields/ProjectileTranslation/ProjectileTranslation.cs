using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Shields.Basic.ProjectileTranslation
{
    abstract class ProjectileTranslation
    {

        public List<ProjectileTranslation> m_Translations = new List<ProjectileTranslation>();
        
        static ProjectileTranslation GetSpecificTranslator(Thing projectileThing)
        {
            return null;
        }
        
        public abstract bool Destroyed();

        public abstract bool FlyOverhead();

        public abstract Thing Launcher();

        public abstract bool WillTargetLandInRange();

        public abstract Quaternion ExactRotation();

        public abstract Vector3 ExactPosition();

        public abstract int DamageAmountBase();

        public abstract Thing ProjectileThing();
    }
}
