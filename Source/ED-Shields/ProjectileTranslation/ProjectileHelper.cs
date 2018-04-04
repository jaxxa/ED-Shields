using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.Shields.Basic.ProjectileTranslation
{
    static class ProjectileHelper
    {
        public static ProjectileTranslation GetTranslator(Thing projectileThing)
        {

            return ProjectileTranslation_VerseProjectile.GetSpecificTranslator(projectileThing);

        }
    }
}
