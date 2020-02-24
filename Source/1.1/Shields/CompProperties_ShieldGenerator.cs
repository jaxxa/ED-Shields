using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.Prometheus.Shields
{
    public class CompProperties_ShieldGenerator : CompProperties
    {

        public CompProperties_ShieldGenerator()
        {
            this.compClass = typeof(Comp_ShieldGenerator);
        }
        
        //Field Settings
        public int m_FieldIntegrity_Initial = 0;
        public int m_FieldIntegrity_Max_Base = 0;
        public int m_Field_Radius_Base = 0;

        //Power Settings
        public int m_PowerRequired_Charging = 0;
        public int m_PowerRequired_Standby = 0;

        //Recovery Settings
        public int m_RechargeTickDelayInterval_Base = 0; //Global Constant?
        public int m_RechargeAmmount_Base = 1;
        public int m_RecoverWarmupDelayTicks_Base = 0;

        //Mode Selections
        public bool m_BlockDirect_Avalable = false;
        public bool m_BlockIndirect_Avalable = false;
        public bool m_InterceptDropPod_Avalable = false;
        public bool m_StructuralIntegrityMode = false;
        
       // public List<string> SIFBuildings = new List<string>(); // Move to Global List.
    }
}
