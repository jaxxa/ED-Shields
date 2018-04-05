using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.Shields.Basic.ProjectileTranslation
{

    static class ProjectileHelper
    {
        static ProjectileHelper()
        {
            Log.Error("Adding");

            m_Translations.Add(new ProjectileTranslationGenerator_VerseProjectile());

            Type _CEType = Type.GetType("CombatExtended.ProjectileCE, CombatExtended");
            if (_CEType == null)
            {
                Log.Message("CombatExtended.ProjectileCE not found, Skipping applying ProjectileTranslationGenerator_ProjectileCE.");
            }
            else
            {
                Log.Message("CombatExtended.ProjectileCE found, applying ProjectileTranslationGenerator_ProjectileCE.");
                m_Translations.Add(new ProjectileTranslationGenerator_ProjectileCE());
            }

        }

        public static List<ProjectileTranslationGenerator> m_Translations = new List<ProjectileTranslationGenerator>();
        
        public static ProjectileTranslation GetTranslator(Thing projectileThing)
        {
            //Log.Message("GetTranslator");
            foreach (ProjectileTranslationGenerator _CurrentTransator in m_Translations)
            {
                ProjectileTranslation _Temp = _CurrentTransator.GetTranslator(projectileThing);
                if (_Temp != null)
                {
                    Log.Message("Returning");
                    return _Temp;
                }
            }
            
            return null;
            //return ProjectileTranslation_VerseProjectile.GetSpecificTranslator(projectileThing);
        }
    }
}
