using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using System.Reflection;

namespace EnhancedDevelopment.Prometheus.Shields
{

    [StaticConstructorOnStartup]
    public class Comp_ShieldGenerator : ThingComp
    {

        Material currentMatrialColour;

        public CompProperties_ShieldGenerator Properties;

        #region Variables

        //UI elements - Unsaved
        private static Texture2D UI_DIRECT_ON;
        private static Texture2D UI_DIRECT_OFF;

        private static Texture2D UI_INDIRECT_ON;
        private static Texture2D UI_INDIRECT_OFF;

        private static Texture2D UI_INTERCEPT_DROPPOD_ON;
        private static Texture2D UI_INTERCEPT_DROPPOD_OFF;

        private static Texture2D UI_SHOW_ON;
        private static Texture2D UI_SHOW_OFF;

        //Visual Settings
        private bool m_ShowVisually_Active = true;
        private float m_ColourRed;
        private float m_ColourGreen;
        private float m_ColourBlue;

        //Field Settings
        public int m_FieldIntegrity_Max;
        private int m_FieldIntegrity_Initial;

        //Recovery Settings
        private int m_RechargeTickDelayInterval;
        private int m_RecoverWarmupDelayTicks;
        private int m_WarmupTicksRemaining;


        #endregion Variables
        
        #region Settings

        // Power Usage --------------------------------------------------------------

        //Comp, found each time.
        CompPowerTrader m_Power;

        private int m_PowerRequired;


        // Range --------------------------------------------------------------------

        public int m_FieldRadius_Avalable;
        public int m_FieldRadius_Requested = 999;

        public int FieldRadius_Active()
        {
            return Math.Min(this.m_FieldRadius_Requested, this.m_FieldRadius_Avalable);
        }

        // Block Direct -------------------------------------------------------------


        private bool m_BlockDirect_Avalable;

        private bool m_BlockDirect_Requested = true;

        public bool BlockDirect_Active()
        {
            return this.m_BlockDirect_Avalable && this.m_BlockDirect_Requested;
        }

        // Block Indirect -----------------------------------------------------------


        private bool m_BlockIndirect_Avalable;

        private bool m_BlockIndirect_Requested = true;

        public bool BlockIndirect_Active()
        {
            return this.m_BlockIndirect_Avalable && this.m_BlockIndirect_Requested;
        }

        //Block Droppods ------------------------------------------------------------

        private bool m_InterceptDropPod_Avalable;

        private bool m_InterceptDropPod_Requested = true;

        public bool IntercepDropPod_Active()
        {
            return m_InterceptDropPod_Avalable && m_InterceptDropPod_Requested;
        }
        
        public bool IsInterceptDropPod_Avalable()
        {
            return this.m_InterceptDropPod_Avalable;
        }

        // Identify Friend Foe ------------------------------------------------------

        private bool m_IdentifyFriendFoe_Avalable = false;

        private bool m_IdentifyFriendFoe_Requested = true;

        public bool IdentifyFriendFoe_Active()
        {
            return this.m_IdentifyFriendFoe_Avalable && this.m_IdentifyFriendFoe_Requested;
        }

        // Slow Discharge -----------------------------------------------------------

        public bool SlowDischarge_Active;

        #endregion

        #region Initilisation

        //Static Construtor
        static Comp_ShieldGenerator()
        {
            //Setup UI
            UI_DIRECT_OFF = ContentFinder<Texture2D>.Get("UI/DirectOff", true);
            UI_DIRECT_ON = ContentFinder<Texture2D>.Get("UI/DirectOn", true);
            UI_INDIRECT_OFF = ContentFinder<Texture2D>.Get("UI/IndirectOff", true);
            UI_INDIRECT_ON = ContentFinder<Texture2D>.Get("UI/IndirectOn", true);
            UI_INTERCEPT_DROPPOD_OFF = ContentFinder<Texture2D>.Get("UI/FireOff", true);
            UI_INTERCEPT_DROPPOD_ON = ContentFinder<Texture2D>.Get("UI/FireOn", true);

            UI_SHOW_ON = ContentFinder<Texture2D>.Get("UI/ShieldShowOn", true);
            UI_SHOW_OFF = ContentFinder<Texture2D>.Get("UI/ShieldShowOff", true);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            this.Properties = ((CompProperties_ShieldGenerator)this.props);
            this.m_Power = this.parent.GetComp<CompPowerTrader>();

            this.RecalculateStatistics();
        }

        public void RecalculateStatistics()
        {
            //Log.Message("RecalculateStatistics");

            //Visual Settings
            this.m_ColourRed = 0.5f;
            this.m_ColourGreen = 0.0f;
            this.m_ColourBlue = 0.5f;

            //Field Settings
            this.m_FieldIntegrity_Max = this.Properties.m_FieldIntegrity_Max_Base;
            this.m_FieldIntegrity_Initial = this.Properties.m_FieldIntegrity_Initial;
            this.m_FieldRadius_Avalable = this.Properties.m_Field_Radius_Base;

            //Mode Settings - Avalable
            this.m_BlockIndirect_Avalable = this.Properties.m_BlockIndirect_Avalable;
            this.m_BlockDirect_Avalable = this.Properties.m_BlockDirect_Avalable;
            this.m_InterceptDropPod_Avalable = this.Properties.m_InterceptDropPod_Avalable;

            //Power Settings
            this.m_PowerRequired = this.Properties.m_PowerRequired_Charging;

            //Recovery Settings
            this.m_RechargeTickDelayInterval = this.Properties.m_RechargeTickDelayInterval_Base;
            this.m_RecoverWarmupDelayTicks = this.Properties.m_RecoverWarmupDelayTicks_Base;

            //Power converter
            this.SlowDischarge_Active = false;

            //IFF
            this.m_IdentifyFriendFoe_Avalable = false;

            //Store the List of Building in initilisation????

            CompFacility _Facility = this.parent.GetComp<CompFacility>();
            Patch.Patcher.LogNULL(_Facility, "_Facility");

            FieldInfo _LinkedBuildingsInfo = typeof(CompFacility).GetField("linkedBuildings", BindingFlags.NonPublic | BindingFlags.Instance);
            Patch.Patcher.LogNULL(_LinkedBuildingsInfo, "_LinkedBuildingsInfo");

            List<Thing> _LinkedBuildings = _LinkedBuildingsInfo.GetValue(_Facility) as List<Thing>;
            Patch.Patcher.LogNULL(_LinkedBuildings, "_LinkedBuildings");

            //Log.Message(_LinkedBuildings.Count.ToString());


            _LinkedBuildings.ForEach(b =>
            {
                Building _Building = b as Building;
                Comp_ShieldUpgrade _Comp = _Building.GetComp<Comp_ShieldUpgrade>();

                Patch.Patcher.LogNULL(_Comp, "_Comp");
                Patch.Patcher.LogNULL(_Comp.Properties, "_Comp.Properties");

                this.AddStatsFromUpgrade(_Comp);

            });

            this.m_Power.powerOutputInt = -this.m_PowerRequired;

        }

        private void AddStatsFromUpgrade(Comp_ShieldUpgrade comp)
        {

            this.m_FieldIntegrity_Max += comp.Properties.FieldIntegrity_Increase;
            this.m_FieldRadius_Avalable += comp.Properties.Range_Increase;

            //Power
            this.m_PowerRequired += comp.Properties.PowerUsage_Increase;


            if (comp.Properties.DropPodIntercept)
            {
                this.m_InterceptDropPod_Avalable = true;
            }

            if (comp.Properties.IdentifyFriendFoe)
            {
                //Log.Message("Setting IFF");
                this.m_IdentifyFriendFoe_Avalable = true;
            }

            if (comp.Properties.SlowDischarge)
            {
                this.SlowDischarge_Active = true;
            }

        }

        #endregion Initilisation

        #region Methods

        public override void CompTick()
        {
            base.CompTick();

            //this.RecalculateStatistics();

            this.UpdateShieldStatus();

            this.TickRecharge();

        }

        public void UpdateShieldStatus()
        {
            Boolean _PowerAvalable = this.CheckPowerOn();

            switch (this.CurrentStatus)
            {

                case (EnumShieldStatus.Offline):

                    //If it is offline bit has Power start initialising
                    if (_PowerAvalable)
                    {
                        this.CurrentStatus = EnumShieldStatus.Initilising;
                        this.m_WarmupTicksRemaining = this.m_RecoverWarmupDelayTicks;
                    }
                    break;

                case (EnumShieldStatus.Initilising):
                    if (_PowerAvalable)
                    {
                        if (this.m_WarmupTicksRemaining > 0)
                        {
                            this.m_WarmupTicksRemaining--;
                        }
                        else
                        {
                            this.CurrentStatus = EnumShieldStatus.ActiveCharging;
                            this.FieldIntegrity_Current = this.m_FieldIntegrity_Initial;
                        }
                    }
                    else
                    {
                        this.CurrentStatus = EnumShieldStatus.Offline;
                    }
                    break;

                case (EnumShieldStatus.ActiveDischarging):
                    if (_PowerAvalable)
                    {
                        this.CurrentStatus = EnumShieldStatus.ActiveCharging;
                    }
                    else
                    {
                        if (!this.SlowDischarge_Active)
                        {
                            this.m_FieldIntegrity_Current = 0;
                        }

                        if (this.FieldIntegrity_Current <= 0)
                        {
                            this.CurrentStatus = EnumShieldStatus.Offline;

                        }
                    }
                    break;

                case (EnumShieldStatus.ActiveCharging):
                    if (this.FieldIntegrity_Current < 0)
                    {
                        this.CurrentStatus = EnumShieldStatus.Offline;
                    }
                    else
                    {
                        if (!_PowerAvalable)
                        {
                            this.CurrentStatus = EnumShieldStatus.ActiveDischarging;
                        }
                        else if (this.FieldIntegrity_Current >= this.m_FieldIntegrity_Max)
                        {
                            this.CurrentStatus = EnumShieldStatus.ActiveSustaining;
                        }
                    }
                    break;

                case (EnumShieldStatus.ActiveSustaining):
                    if (!_PowerAvalable)
                    {
                        this.CurrentStatus = EnumShieldStatus.ActiveDischarging;
                    }
                    else
                    {
                        if (this.FieldIntegrity_Current < this.m_FieldIntegrity_Max)
                        {
                            this.CurrentStatus = EnumShieldStatus.ActiveCharging;
                        }
                    }
                    break;
            }
        }

        public bool IsActive()
        {
            //return true;
            return (this.CurrentStatus == EnumShieldStatus.ActiveCharging ||
                 this.CurrentStatus == EnumShieldStatus.ActiveDischarging ||
                 this.CurrentStatus == EnumShieldStatus.ActiveSustaining);
        }

        public bool CheckPowerOn()
        {
            if (this.m_Power != null)
            {
                if (this.m_Power.PowerOn)
                {
                    return true;
                }
            }
            return false;
        }

        public void TickRecharge()
        {
            if (Find.TickManager.TicksGame % this.m_RechargeTickDelayInterval == 0)
            {
                if (this.CurrentStatus == EnumShieldStatus.ActiveCharging)
                {
                    this.FieldIntegrity_Current++;
                }
                else if (this.CurrentStatus == EnumShieldStatus.ActiveDischarging)
                {
                    this.FieldIntegrity_Current--;
                }
            }
        }

        public bool WillInterceptDropPod(DropPodIncoming dropPodToCheck)
        {
            //Check if can and wants to intercept
            if (!this.IntercepDropPod_Active())
            {
                return false;
            }

            //Check if online
            if (this.CurrentStatus == EnumShieldStatus.Offline || this.CurrentStatus == EnumShieldStatus.Initilising)
            {
                return false;
            }


            //Check IFF
            if (this.IdentifyFriendFoe_Active())
            {
                bool _Hostile = dropPodToCheck.Contents.innerContainer.Any(x => x.Faction.HostileTo(Faction.OfPlayer));

                if (!_Hostile)
                {
                    return false;
                }
            }

            //Check Distance
            float _Distance = Vector3.Distance(dropPodToCheck.Position.ToVector3(), this.parent.Position.ToVector3());
            float _Radius = this.FieldRadius_Active();
            if (_Distance > _Radius)
            {
                return false;
            }

            //All Tests passed so intercept the pod
            return true;

        }

        public bool WillProjectileBeBlocked(Verse.Projectile projectile)
        {

            //Check if online
            if (this.CurrentStatus == EnumShieldStatus.Offline || this.CurrentStatus == EnumShieldStatus.Initilising)
            {
                return false;
            }

            //Check if can and wants to intercept
            if (projectile.def.projectile.flyOverhead)
            {
                if (!this.BlockIndirect_Active()) { return false; }
            }
            else
            {
                if (!this.BlockDirect_Active()) { return false; }
            }

            //Check Distance
            float _Distance = Vector3.Distance(projectile.Position.ToVector3(), this.parent.Position.ToVector3());
            if (_Distance > this.FieldRadius_Active())
            {
                return false;
            }

            //Check Angle
            if (!Comp_ShieldGenerator.CorrectAngleToIntercept(projectile, this.parent))
            {
                return false;
            }

            //Check IFF
            if (this.IdentifyFriendFoe_Active())
            {
                FieldInfo _LauncherFieldInfo = typeof(Projectile).GetField("launcher", BindingFlags.NonPublic | BindingFlags.Instance);
                Patch.Patcher.LogNULL(_LauncherFieldInfo, "_LauncherFieldInfo");
                Thing _Launcher = (Thing)_LauncherFieldInfo.GetValue(projectile);
                Patch.Patcher.LogNULL(_Launcher, "_Launcher");

                if (_Launcher.Faction.IsPlayer)
                {
                    return false;
                }
            }

            return true;

        }

        public static Boolean CorrectAngleToIntercept(Projectile pr, Thing shieldBuilding)
        {
            //Detect proper collision using angles
            Quaternion targetAngle = pr.ExactRotation;

            Vector3 projectilePosition2D = pr.ExactPosition;
            projectilePosition2D.y = 0;

            Vector3 shieldPosition2D = shieldBuilding.Position.ToVector3();
            shieldPosition2D.y = 0;

            Quaternion shieldProjAng = Quaternion.LookRotation(projectilePosition2D - shieldPosition2D);

            if ((Quaternion.Angle(targetAngle, shieldProjAng) > 90))
            {
                return true;
            }

            return false;
        }

        #endregion Methods

        #region Properties

        public EnumShieldStatus CurrentStatus
        {
            get
            {
                return this.m_CurrentStatus;
            }
            set
            {
                this.m_CurrentStatus = value;

                //if (this.m_CurrentStatus == EnumShieldStatus.ActiveSustaining)
                //{
                //    this.m_Power.powerOutputInt = -this.m_PowerRequired_Standby;
                //}
                //else
                //{
                //    this.m_Power.powerOutputInt = -this.m_PowerRequired_Charging;
                //}
            }
        }
        private EnumShieldStatus m_CurrentStatus = EnumShieldStatus.Offline;

        public int FieldIntegrity_Current
        {
            get
            {
                return this.m_FieldIntegrity_Current;
            }
            set
            {
                if (value < 0)
                {
                    this.CurrentStatus = EnumShieldStatus.Offline;
                    this.m_FieldIntegrity_Current = 0;
                }
                else if (value > this.m_FieldIntegrity_Max)
                {
                    this.m_FieldIntegrity_Current = this.m_FieldIntegrity_Max;
                }
                else
                {
                    this.m_FieldIntegrity_Current = value;
                }
            }
        }
        private int m_FieldIntegrity_Current;

        #endregion Properties

        #region Drawing

        public override void PostDraw()
        {
            //Log.Message("DrawComp");
            base.PostDraw();

            this.DrawShields();


        }

        /// <summary>
        /// Draw the shield Field
        /// </summary>
        public void DrawShields()
        {
            if (!this.IsActive() || !this.m_ShowVisually_Active)
            {
                return;
            }

            //Draw field
            this.DrawField(EnhancedDevelopment.Prometheus.Shields.Utilities.VectorsUtils.IntVecToVec(this.parent.Position));

        }

        //public override void DrawExtraSelectionOverlays()
        //{
        //    //    GenDraw.DrawRadiusRing(base.Position, shieldField.shieldShieldRadius);
        //}

        public void DrawSubField(IntVec3 center, float radius)
        {
            this.DrawSubField(EnhancedDevelopment.Prometheus.Shields.Utilities.VectorsUtils.IntVecToVec(center), radius);
        }

        //Draw the field on map
        public void DrawField(Vector3 center)
        {
            DrawSubField(center, this.FieldRadius_Active());
        }

        public void DrawSubField(Vector3 position, float shieldShieldRadius)
        {
            position = position + (new Vector3(0.5f, 0f, 0.5f));

            Vector3 s = new Vector3(shieldShieldRadius, 1f, shieldShieldRadius);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(position, Quaternion.identity, s);

            if (currentMatrialColour == null)
            {
                //Log.Message("Creating currentMatrialColour");
                currentMatrialColour = SolidColorMaterials.NewSolidColorMaterial(new Color(m_ColourRed, m_ColourGreen, m_ColourBlue, 0.15f), ShaderDatabase.MetaOverlay);
                //currentMatrialColour = SolidColorMaterials.NewSolidColorMaterial(new Color(0.5f, 0.0f, 0.0f, 0.15f), ShaderDatabase.MetaOverlay);
            }

            UnityEngine.Graphics.DrawMesh(EnhancedDevelopment.Prometheus.Shields.Utilities.Graphics.CircleMesh, matrix, currentMatrialColour, 0);

        }

        #endregion Drawing

        #region UI

        public override string CompInspectStringExtra()
        {
            StringBuilder _StringBuilder = new StringBuilder();
            //return base.CompInspectStringExtra();
            _StringBuilder.Append(base.CompInspectStringExtra());

            if (this.IsActive())
            {
                _StringBuilder.AppendLine("Shield: " + this.FieldIntegrity_Current + "/" + this.m_FieldIntegrity_Max);
            }
            else if (this.CurrentStatus == EnumShieldStatus.Initilising)
            {
                //stringBuilder.AppendLine("Initiating shield: " + ((warmupTicks * 100) / recoverWarmup) + "%");
                _StringBuilder.AppendLine("Ready in " + Math.Round(GenTicks.TicksToSeconds(m_WarmupTicksRemaining)) + " seconds.");
                //stringBuilder.AppendLine("Ready in " + m_warmupTicksCurrent + " seconds.");
            }
            else
            {
                _StringBuilder.AppendLine("Shield disabled!");
            }

            if (m_Power != null)
            {
                string text = m_Power.CompInspectStringExtra();
                if (!text.NullOrEmpty())
                {
                    _StringBuilder.Append(text);
                }
                else
                {
                    _StringBuilder.Append("Error, No Power Comp Text.");
                }
            }
            else
            {
                _StringBuilder.Append("Error, No Power Comp.");
            }

            return _StringBuilder.ToString();

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //return base.CompGetGizmosExtra();

            //Add the stock Gizmoes
            foreach (var g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            if (m_BlockDirect_Avalable)
            {
                if (this.BlockDirect_Active())
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchDirect();
                    act.icon = UI_DIRECT_ON;
                    act.defaultLabel = "Block Direct";
                    act.defaultDesc = "On";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
                else
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchDirect();
                    act.icon = UI_DIRECT_OFF;
                    act.defaultLabel = "Block Direct";
                    act.defaultDesc = "Off";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
            }

            if (this.m_BlockIndirect_Avalable)
            {
                if (this.BlockIndirect_Active())
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchIndirect();
                    act.icon = UI_INDIRECT_ON;
                    act.defaultLabel = "Block Indirect";
                    act.defaultDesc = "On";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
                else
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchIndirect();
                    act.icon = UI_INDIRECT_OFF;
                    act.defaultLabel = "Block Indirect";
                    act.defaultDesc = "Off";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
            }

            if (m_InterceptDropPod_Avalable)
            {
                if (this.IntercepDropPod_Active())
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchInterceptDropPod();
                    act.icon = UI_INTERCEPT_DROPPOD_ON;
                    act.defaultLabel = "Intercept DropPod";
                    act.defaultDesc = "On";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
                else
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchInterceptDropPod();
                    act.icon = UI_INTERCEPT_DROPPOD_OFF;
                    act.defaultLabel = "Intercept DropPod";
                    act.defaultDesc = "Off";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
            }


            if (true)
            {
                if (m_ShowVisually_Active)
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchVisual();
                    act.icon = UI_SHOW_ON;
                    act.defaultLabel = "Show Visually";
                    act.defaultDesc = "Show";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
                else
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchVisual();
                    act.icon = UI_SHOW_OFF;
                    act.defaultLabel = "Show Visually";
                    act.defaultDesc = "Hide";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
            }








        } //CompGetGizmosExtra()

        public void SwitchDirect()
        {
            this.m_BlockDirect_Requested = !this.m_BlockDirect_Requested;
        }

        public void SwitchIndirect()
        {
            this.m_BlockIndirect_Requested = !this.m_BlockIndirect_Requested;
        }

        public void SwitchInterceptDropPod()
        {
            this.m_InterceptDropPod_Requested = !this.m_InterceptDropPod_Requested;
        }

        private void SwitchVisual()
        {
            this.m_ShowVisually_Active = !this.m_ShowVisually_Active;
        }

        #endregion UI

        #region DataAcess

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref m_FieldRadius_Requested, "m_FieldRadius_Requested");
            Scribe_Values.Look(ref m_BlockDirect_Requested, "m_BlockDirect_Requested");
            Scribe_Values.Look(ref m_BlockIndirect_Requested, "m_BlockIndirect_Requested");
            Scribe_Values.Look(ref m_InterceptDropPod_Requested, "m_InterceptDropPod_Requested");
            Scribe_Values.Look(ref m_IdentifyFriendFoe_Requested, "m_IdentifyFriendFoe_Requested");

            Scribe_Values.Look(ref m_RechargeTickDelayInterval, "m_shieldRechargeTickDelay");
            Scribe_Values.Look(ref m_RecoverWarmupDelayTicks, "m_shieldRecoverWarmup");

            Scribe_Values.Look(ref m_ColourRed, "m_colourRed");
            Scribe_Values.Look(ref m_ColourGreen, "m_colourGreen");
            Scribe_Values.Look(ref m_ColourBlue, "m_colourBlue");

            Scribe_Values.Look(ref m_WarmupTicksRemaining, "m_WarmupTicksRemaining");

            Scribe_Values.Look(ref m_CurrentStatus, "m_CurrentStatus");
            Scribe_Values.Look(ref m_FieldIntegrity_Current, "m_FieldIntegrity_Current");

        }

        #endregion DataAcess

    }
}
