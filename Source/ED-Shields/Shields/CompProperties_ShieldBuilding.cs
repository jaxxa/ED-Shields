using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.Shields
{

    public class CompProperties_ShieldBuilding : CompProperties
    //public class Comp_ShieldBuilding : ThingComp
    {

        public CompProperties_ShieldBuilding()
        {
            this.compClass = typeof(Comp_ShieldBuilding);
        }

        public int m_FieldIntegrity_Max;
        public int m_FieldIntegrity_Initial;
        public int m_Field_Radius;

        public int m_PowerRequiredCharging;
        public int m_PowerRequiredSustaining;

        public int m_RechargeTickDelayInterval;
        public int m_RecoverWarmupDelayTicks;

        public bool m_BlockIndirect_Avalable;
        public bool m_BlockDirect_Avalable;
        public bool m_FireSupression_Avalable;
        public bool m_InterceptDropPod_Avalable;

        public bool m_StructuralIntegrityMode;

        public float m_ColourRed;
        public float m_ColourGreen;
        public float m_ColourBlue;

        public List<string> SIFBuildings;
    }
}
