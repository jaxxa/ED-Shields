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
            Log.Message("Adding");
            m_Translations.Add(new ProjectileTranslationGenerator_VerseProjectile());
        }

        public static List<ProjectileTranslationGenerator> m_Translations = new List<ProjectileTranslationGenerator>();
        
        public static ProjectileTranslation GetTranslator(Thing projectileThing)
        {
            
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
