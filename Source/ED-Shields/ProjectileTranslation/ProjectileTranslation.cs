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

        public abstract bool Destroyed();

        public abstract bool FlyOverhead();

        public abstract Thing Launcher();

        public abstract bool WillTargetLandInRange(int fieldRadius);

        public abstract Quaternion ExactRotation();

        public abstract Vector3 ExactPosition();

        public abstract int DamageAmountBase();

        public abstract Thing ProjectileThing();

        public abstract Vector3 TargetLocation();

    }
}
