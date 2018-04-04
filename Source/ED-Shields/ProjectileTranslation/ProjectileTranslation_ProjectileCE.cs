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
        public override int DamageAmountBase()
        {
            throw new NotImplementedException();
        }

        public override bool Destroyed()
        {
            throw new NotImplementedException();
        }

        public override Vector3 ExactPosition()
        {
            throw new NotImplementedException();
        }

        public override Quaternion ExactRotation()
        {
            throw new NotImplementedException();
        }

        public override bool FlyOverhead()
        {
            throw new NotImplementedException();
        }

        public override Thing Launcher()
        {
            throw new NotImplementedException();
        }

        public override Thing ProjectileThing()
        {
            throw new NotImplementedException();
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
            return null;
            //return ProjectileTranslation_VerseProjectile.GetSpecificTranslator(projectileThing);
        }
    }
}
