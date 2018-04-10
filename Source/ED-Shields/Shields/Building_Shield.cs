using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

using RimWorld;
using Verse;
using Verse.Sound;

using EnhancedDevelopment.Shields.Basic.ShieldUtils;

namespace EnhancedDevelopment.Shields.Basic
{
    [StaticConstructorOnStartup]
    public class Building_Shield : Building
    {

        Material currentMatrialColour;

        //Ratio of lost power per damage
        private const float powerToDamage = 1f;

        //Hit sound
        public static readonly SoundDef HitSoundDef = SoundDef.Named("Shields_HitShield");

        #region Constants

        private const int DAMAGE_TO_FIRE = 10;
        private const int DAMAGE_FROM_FIRE = 2;
        private const int FIRE_SUPRESSION_TICK_DELAY = 15;

        #endregion

        #region Variables

        //UI elements
        private static Texture2D UI_DIRECT_ON;
        private static Texture2D UI_DIRECT_OFF;

        private static Texture2D UI_INDIRECT_ON;
        private static Texture2D UI_INDIRECT_OFF;

        private static Texture2D UI_FIRE_ON;
        private static Texture2D UI_FIRE_OFF;

        private static Texture2D UI_INTERCEPT_DROPPOD_ON;
        private static Texture2D UI_INTERCEPT_DROPPOD_OFF;

        private static Texture2D UI_REPAIR_ON;
        private static Texture2D UI_REPAIR_OFF;

        private static Texture2D UI_SHOW_ON;
        private static Texture2D UI_SHOW_OFF;

        //Default all flasgs to be shown initially
        protected bool m_BlockIndirect_Active = true;
        protected bool m_BlockDirect_Active = true;
        protected bool m_FireSupression_Active = true;
        protected bool m_RepairMode_Active = true;
        protected bool m_ShowVisually_Active = true;
        protected bool m_InterceptDropPod_Active = true;

        //Variables to store what modes the shield have avalable.
        protected bool m_BlockIndirect_Avalable;
        protected bool m_BlockDirect_Avalable;
        protected bool m_FireSupression_Avalable;
        protected bool m_InterceptDropPod_Avalable;

        protected bool m_StructuralIntegrityMode;

        //variables that are read in from XML
        private int m_FieldIntegrity_Max;
        private int m_FieldIntegrity_Initial;
        public int m_Field_Radius;

        private int m_PowerRequired_Charging;
        private int m_PowerRequired_Sustaining;

        private int m_RechargeTickDelayInterval;
        private int m_RecoverWarmupDelayTicks;

        private float m_ColourRed;
        private float m_ColourGreen;
        private float m_ColourBlue;

        private List<string> m_SIFBuildings;

        private int m_WarmupTicksRemaining = 0;
        private long m_TickCount = 0;

        CompPowerTrader m_Power;
        //Prepared data
        private ShieldBlendingParticle[] m_SparksParticle = new ShieldBlendingParticle[3];

        #endregion

        #region Properties

        public enumShieldStatus CurrentStatus
        {
            get
            {
                return this.m_CurrentStatus;
            }
            set
            {
                this.m_CurrentStatus = value;

                if (this.m_CurrentStatus == enumShieldStatus.ActiveSustaining)
                {
                    this.m_Power.powerOutputInt = this.m_PowerRequired_Sustaining;
                }
                else
                {
                    this.m_Power.powerOutputInt = this.m_PowerRequired_Charging;
                }
            }
        }
        private enumShieldStatus m_CurrentStatus = enumShieldStatus.Offline;

        private int FieldIntegrity_Current
        {
            get
            {
                return this.m_FieldIntegrity_Current;
            }
            set
            {
                if (value < 0)
                {
                    this.CurrentStatus = enumShieldStatus.Offline;
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

        //List of squares around the shield centre

        //List<IntVec3> CellsToProtect
        //{
        //    get
        //    {
        //        if (this.m_CellsToProtect == null)
        //        {
        //            this.ReCalibrateCells();
        //        }
        //        return this.m_CellsToProtect;
        //    }
        //    set
        //    {
        //        this.m_CellsToProtect = value;
        //    }
        //}
        //List<IntVec3> m_CellsToProtect = null;

        #endregion

        #region Initilisation

        //Constructor

        static Building_Shield()
        {
            //Setup UI
            UI_DIRECT_OFF = ContentFinder<Texture2D>.Get("UI/DirectOff", true);
            UI_DIRECT_ON = ContentFinder<Texture2D>.Get("UI/DirectOn", true);
            UI_INDIRECT_OFF = ContentFinder<Texture2D>.Get("UI/IndirectOff", true);
            UI_INDIRECT_ON = ContentFinder<Texture2D>.Get("UI/IndirectOn", true);
            UI_FIRE_OFF = ContentFinder<Texture2D>.Get("UI/FireOff", true);
            UI_FIRE_ON = ContentFinder<Texture2D>.Get("UI/FireOn", true);
            UI_INTERCEPT_DROPPOD_OFF = ContentFinder<Texture2D>.Get("UI/FireOff", true);
            UI_INTERCEPT_DROPPOD_ON = ContentFinder<Texture2D>.Get("UI/FireOn", true);

            UI_REPAIR_ON = ContentFinder<Texture2D>.Get("UI/RepairOn", true);
            UI_REPAIR_OFF = ContentFinder<Texture2D>.Get("UI/RepairOff", true);

            UI_SHOW_ON = ContentFinder<Texture2D>.Get("UI/ShieldShowOn", true);
            UI_SHOW_OFF = ContentFinder<Texture2D>.Get("UI/ShieldShowOff", true);
        }

        //Dummy override
        //This is called before Spawn Setup.
        public override void PostMake()
        {
            base.PostMake();
            //Log.Message("PM");
        }

        //On spawn, get the power component reference
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            //Log.Message("SS");

            base.SpawnSetup(map, respawningAfterLoad);
            this.m_Power = base.GetComp<CompPowerTrader>();

            Comp_ShieldBuilding _CompShield = this.GetComp<Comp_ShieldBuilding>();
            //_CompShield.props
            if (_CompShield.props is CompProperties_ShieldBuilding)
            {
                //Read in variables from the custom MyThingDef
                m_FieldIntegrity_Max = ((CompProperties_ShieldBuilding)_CompShield.props).m_FieldIntegrity_Max;
                m_FieldIntegrity_Initial = ((CompProperties_ShieldBuilding)_CompShield.props).m_FieldIntegrity_Initial;
                m_Field_Radius = ((CompProperties_ShieldBuilding)_CompShield.props).m_Field_Radius;

                m_PowerRequired_Charging = ((CompProperties_ShieldBuilding)_CompShield.props).m_PowerRequiredCharging;
                m_PowerRequired_Sustaining = ((CompProperties_ShieldBuilding)_CompShield.props).m_PowerRequiredSustaining;

                m_RechargeTickDelayInterval = ((CompProperties_ShieldBuilding)_CompShield.props).m_RechargeTickDelayInterval;
                m_RecoverWarmupDelayTicks = ((CompProperties_ShieldBuilding)_CompShield.props).m_RecoverWarmupDelayTicks;

                m_BlockIndirect_Avalable = ((CompProperties_ShieldBuilding)_CompShield.props).m_BlockIndirect_Avalable;
                m_BlockDirect_Avalable = ((CompProperties_ShieldBuilding)_CompShield.props).m_BlockDirect_Avalable;
                m_FireSupression_Avalable = ((CompProperties_ShieldBuilding)_CompShield.props).m_FireSupression_Avalable;
                m_InterceptDropPod_Avalable = ((CompProperties_ShieldBuilding)_CompShield.props).m_InterceptDropPod_Avalable;

                m_StructuralIntegrityMode = ((CompProperties_ShieldBuilding)_CompShield.props).m_StructuralIntegrityMode;

                m_ColourRed = ((CompProperties_ShieldBuilding)_CompShield.props).m_ColourRed;
                m_ColourGreen = ((CompProperties_ShieldBuilding)_CompShield.props).m_ColourGreen;
                m_ColourBlue = ((CompProperties_ShieldBuilding)_CompShield.props).m_ColourBlue;

                m_SIFBuildings = ((CompProperties_ShieldBuilding)_CompShield.props).SIFBuildings;
                //Log.Error("Count:" + SIFBuildings.Count);
            }
            else
            {
                Log.Error("Shield definition not of type \"ShieldBuildingThingDef\"");
            }
        }
        #endregion

        #region Methods

        public override void Tick()
        {
            base.Tick();
            this.m_TickCount += 1;

            this.UpdateShieldStatus();

            //Disable shield when power goes off

            ////Do tick for the shield field

            //if (this.IsActive())
            //{
            //    this.TickProtection();

            //    if (this.m_TickCount % 5000 == 0)
            //    {
            //        this.ReCalibrateCells();
            //    }
            //}

            this.TickRecharge();
        }

        public bool IsActive()
        {
            //return true;
            return (this.CurrentStatus == enumShieldStatus.ActiveCharging ||
                 this.CurrentStatus == enumShieldStatus.ActiveDischarging ||
                 this.CurrentStatus == enumShieldStatus.ActiveSustaining);
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

        public void UpdateShieldStatus()
        {
            Boolean _PowerAvalable = this.CheckPowerOn();

            switch (this.CurrentStatus)
            {

                case (enumShieldStatus.Offline):

                    //If it is offline bit has Power that initialising
                    if (_PowerAvalable)
                    {
                        this.CurrentStatus = enumShieldStatus.Initilising;
                        //this.m_warmupTicksCurrent = GenDate.SecondsToTicks(this.m_shieldRecoverWarmup);
                        this.m_WarmupTicksRemaining = this.m_RecoverWarmupDelayTicks;
                        //this.CellsToProtect = null;
                    }
                    break;


                case (enumShieldStatus.Initilising):
                    if (_PowerAvalable)
                    {
                        if (this.m_WarmupTicksRemaining > 0)
                        {
                            this.m_WarmupTicksRemaining--;
                            //  Log.Message(this.m_warmupTicksCurrent.ToString());
                        }
                        else
                        {
                            this.CurrentStatus = enumShieldStatus.ActiveCharging;
                            this.FieldIntegrity_Current = this.m_FieldIntegrity_Initial;
                        }
                    }
                    else
                    {
                        this.CurrentStatus = enumShieldStatus.Offline;
                    }
                    break;


                case (enumShieldStatus.ActiveDischarging):
                    if (_PowerAvalable)
                    {
                        this.CurrentStatus = enumShieldStatus.ActiveCharging;
                    }
                    else if (this.FieldIntegrity_Current <= 0)
                    {
                        this.CurrentStatus = enumShieldStatus.Offline;
                    }
                    break;
                case (enumShieldStatus.ActiveCharging):
                    if (this.FieldIntegrity_Current < 0)
                    {
                        this.CurrentStatus = enumShieldStatus.Offline;
                    }
                    else
                    {
                        if (!_PowerAvalable)
                        {
                            this.CurrentStatus = enumShieldStatus.ActiveDischarging;
                        }
                        else if (this.FieldIntegrity_Current >= this.m_FieldIntegrity_Max)
                        {
                            this.CurrentStatus = enumShieldStatus.ActiveSustaining;
                        }
                    }
                    break;

                case (enumShieldStatus.ActiveSustaining):
                    if (!_PowerAvalable)
                    {
                        this.CurrentStatus = enumShieldStatus.ActiveDischarging;
                    }
                    else
                    {
                        if (this.FieldIntegrity_Current < this.m_FieldIntegrity_Max)
                        {
                            this.CurrentStatus = enumShieldStatus.ActiveCharging;
                        }
                    }
                    break;
            }
        }

        //public void TickProtection()
        //{

        //    foreach (IntVec3 square in this.CellsToProtect)
        //    {
        //        //Only use squares around, not inside
        //        this.ProtectSquare(square);

        //        if (this.m_StructuralIntegrityMode)
        //        {
        //            this.SupressFire(square);
        //            this.RepairSytem(square);
        //        }
        //    }

        //    if (!this.m_StructuralIntegrityMode)
        //    {
        //        this.SupressFire();
        //    }

        //    this.interceptPods();
        //}

        public void TickRecharge()
        {
            if (this.m_TickCount % this.m_RechargeTickDelayInterval == 0)
            {
                if (this.CurrentStatus == enumShieldStatus.ActiveCharging)
                {
                    this.FieldIntegrity_Current++;
                }
                else if (this.CurrentStatus == enumShieldStatus.ActiveDischarging)
                {
                    this.FieldIntegrity_Current--;
                }
            }
        }

        //public void ReCalibrateCells()
        //{
        //    //Log.Message("Recalibrate");
        //    this.CellsToProtect = new List<IntVec3>();

        //    if (this.m_StructuralIntegrityMode)
        //    {
        //        IEnumerable<IntVec3> _AllSquares = GenRadial.RadialCellsAround(this.Position, this.m_Field_Radius, false);

        //        foreach (IntVec3 _Square in _AllSquares)
        //        {
        //            // Log.Message("Testing:" + _Square.ToString());
        //            List<Thing> _Things = this.Map.thingGrid.ThingsListAt(_Square);

        //            for (int i = 0, l = _Things.Count(); i < l; i++)
        //            {
        //                if (_Things[i] is Building)
        //                {
        //                    Building building = (Building)_Things[i];

        //                    if (isBuildingValid(building))
        //                    {
        //                        this.CellsToProtect.Add(_Square);
        //                        i = int.MaxValue - 1;
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        IEnumerable<IntVec3> _AllSquares = GenRadial.RadialCellsAround(this.Position, this.m_Field_Radius, false);

        //        foreach (IntVec3 _Square in _AllSquares)
        //        {
        //            //if (Vectors.VectorSize(_Square) >= (float)this.m_Field_Radius - 1.5f)
        //            if (Vectors.EuclDist(_Square, this.Position) >= (float)this.m_Field_Radius - 1.5f)
        //            {
        //                this.CellsToProtect.Add(_Square);
        //            }
        //        }
        //    }
        //    //this.m_CellsToProtect
        //    //if (Vectors.VectorSize(square) >= (float)this.m_shieldShieldRadius - 1.5f)
        //}

        /// <summary>
        /// Finds all projectiles at the position and destroys them
        /// </summary>
        /// <param name="square">The current square to protect</param>
        /// <param name="flag_direct">Block direct Fire</param>
        /// <param name="flag_indirect">Block indirect Fire</param>
        virtual public void ProtectSquare(IntVec3 square)
        {
            //Ignore squares outside the map
            if (!square.InBounds(this.Map))
            {
                return;
            }
            List<Thing> things = this.Map.thingGrid.ThingsListAt(square);
            List<Thing> thingsToDestroy = new List<Thing>();
            Boolean _IFFCheck = this.m_StructuralIntegrityMode;

            for (int i = 0, l = things.Count(); i < l; i++)
            {

                if (things[i] != null && things[i] is Projectile)
                {
                    //Assign to variable
                    Projectile pr = (Projectile)things[i];
                    if (!pr.Destroyed && ((this.m_BlockIndirect_Avalable && this.m_BlockDirect_Active && pr.def.projectile.flyOverhead) || (this.m_BlockDirect_Avalable && this.m_BlockIndirect_Active && !pr.def.projectile.flyOverhead)))
                    {
                        bool wantToIntercept = true;

                        //Check IFF
                        if (_IFFCheck == true)
                        {
                            //Log.Message("IFFcheck == true");
                            Thing launcher = ReflectionHelper.GetInstanceField(typeof(Projectile), pr, "launcher") as Thing;

                            if (launcher != null)
                            {
                                if (launcher.Faction != null)
                                {
                                    //Log.Message("launcher != null");
                                    if (launcher.Faction.IsPlayer)
                                    {
                                        wantToIntercept = false;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }

                        //Check OverShoot
                        if (pr.def.projectile.flyOverhead)
                        {
                            if (this.WillTargetLandInRange(pr))
                            {
                                //Log.Message("Fly Over");
                            }
                            else
                            {
                                wantToIntercept = false;
                                //Log.Message("In Range");
                            }
                        }


                        if (wantToIntercept)
                        {

                            //Detect proper collision using angles
                            Quaternion targetAngle = pr.ExactRotation;

                            Vector3 projectilePosition2D = pr.ExactPosition;
                            projectilePosition2D.y = 0;

                            Vector3 shieldPosition2D = Vectors.IntVecToVec(this.Position);
                            shieldPosition2D.y = 0;

                            Quaternion shieldProjAng = Quaternion.LookRotation(projectilePosition2D - shieldPosition2D);


                            if ((Quaternion.Angle(targetAngle, shieldProjAng) > 90))
                            {

                                //On hit effects
                                MoteMaker.ThrowLightningGlow(pr.ExactPosition, this.Map, 0.5f);
                                //On hit sound
                                HitSoundDef.PlayOneShot((SoundInfo)new TargetInfo(this.Position, this.Map, false));

                                //Damage the shield
                                ProcessDamage(pr.def.projectile.damageAmountBase);
                                //add projectile to the list of things to be destroyed
                                thingsToDestroy.Add(pr);
                            }
                        }
                        else
                        {
                            //Log.Message("Skip");
                        }

                    }
                }
            }
            foreach (Thing currentThing in thingsToDestroy)
            {
                currentThing.Destroy();
            }

        }

        /// <summary>
        /// Damage the Shield
        /// </summary>
        /// <param name="damage">The amount of damage to give</param>
        public void ProcessDamage(int damage)
        {
            //Prevent negative health
            if (this.FieldIntegrity_Current <= 0)
                return;
            this.FieldIntegrity_Current -= (int)(((float)damage) * powerToDamage);
        }

        /// <summary>
        /// Checks if the projectile will land within the shield or pass over.
        /// </summary>
        /// <param name="projectile">The specific projectile that is being checked</param>
        /// <returns>True if the projectile will land close, false if it will be far away.</returns>
        public bool WillTargetLandInRange(Projectile projectile)
        {
            Vector3 targetLocation = GetTargetLocationFromProjectile(projectile);

            if (Vector3.Distance(this.Position.ToVector3(), targetLocation) > this.m_Field_Radius)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Vector3 GetTargetLocationFromProjectile(Projectile projectile)
        {
            FieldInfo fieldInfo = projectile.GetType().GetField("destination", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            Vector3 reoveredVector = (Vector3)fieldInfo.GetValue(projectile);
            return reoveredVector;
        }

        public bool isBuildingValid(Thing currentBuilding)
        {
            if (this.m_SIFBuildings != null)
            {
                if (this.m_SIFBuildings.Contains(currentBuilding.def.defName))
                {
                    return true;
                }
            }
            else
            {
                Log.Error("ShieldField.validBuildings Not set up properly");
            }

            return false;
        }

        private void interceptPods()
        {
            if (this.m_InterceptDropPod_Avalable && this.m_InterceptDropPod_Active)
            {
                IEnumerable<Thing> dropPods = this.Map.listerThings.ThingsOfDef(ThingDefOf.ActiveDropPod);

                if (dropPods != null)
                {
                    //Test to protect the entire map from droppods.
                    //IEnumerable<Thing> closeFires = dropPods.Where<Thing>(t => t.Position.InHorDistOf(this.position, 9999999.0f));

                    IEnumerable<Thing> closeFires = dropPods.Where<Thing>(t => t.Position.InHorDistOf(this.Position, this.m_Field_Radius));

                    foreach (RimWorld.ActiveDropPod currentDropPod in closeFires.ToList())
                    {
                        //currentDropPod.Destroy();

                        GenExplosion.DoExplosion(currentDropPod.Position, this.Map, 1, DamageDefOf.Bomb, currentDropPod);

                        currentDropPod.Destroy(DestroyMode.Vanish);


                        // BodyPartDamageInfo bodyPartDamageInfo = new BodyPartDamageInfo(new BodyPartHeight?(), new BodyPartDepth?(BodyPartDepth.Outside));
                        //new ExplosionInfo()
                        //{
                        //    center = currentDropPod.Position,
                        //    radius = 1,
                        //    dinfo = new DamageInfo(DamageDefOf.Flame, 10, currentDropPod)

                        //}.DoExplosion();

                    }
                }
            }
        }

        private void SupressFire()
        {
            if (this.m_FireSupression_Avalable && this.m_FireSupression_Active && (this.m_TickCount % FIRE_SUPRESSION_TICK_DELAY == 0))
            //if (this.m_FireSupression_Avalable && this.m_FireSupression_Active)
            {
                IEnumerable<Thing> fires = this.Map.listerThings.ThingsOfDef(ThingDefOf.Fire);

                if (fires != null)
                {
                    IEnumerable<Thing> closeFires = fires.Where<Thing>(t => t.Position.InHorDistOf(this.Position, this.m_Field_Radius));

                    if (closeFires != null)
                    {
                        //List<Thing> fireTo
                        foreach (Fire currentFire in closeFires.ToList())
                        {
                            if (this.FieldIntegrity_Current > DAMAGE_FROM_FIRE)
                            {
                                //Damage the shield
                                ProcessDamage(DAMAGE_FROM_FIRE);

                                currentFire.TakeDamage(new DamageInfo(DamageDefOf.Extinguish, DAMAGE_TO_FIRE, -1, this));
                            }
                        }
                    }
                }
            }
        }

        private void SupressFire(IntVec3 position)
        {
            if (this.m_FireSupression_Avalable && this.m_FireSupression_Active && (this.m_TickCount % FIRE_SUPRESSION_TICK_DELAY == 0))
            //if (this.m_FireSupression_Avalable && this.m_FireSupression_Active)
            {
                IEnumerable<Thing> things = this.Map.thingGrid.ThingsAt(position);
                List<Thing> fires = new List<Thing>();

                foreach (Thing currentThing in things)
                {
                    if (currentThing is Fire)
                    {
                        fires.Add(currentThing);
                    }
                }

                foreach (Fire currentFire in fires.ToList())
                {
                    if (this.FieldIntegrity_Current > DAMAGE_FROM_FIRE)
                    {
                        //Damage the shield
                        ProcessDamage(DAMAGE_FROM_FIRE);

                        currentFire.TakeDamage(new DamageInfo(DamageDefOf.Extinguish, DAMAGE_TO_FIRE, -1, this));
                    }
                }
            }
        }

        private void RepairSytem(IntVec3 position)
        {
            if (this.m_RepairMode_Active)
            {
                List<Thing> things = this.Map.thingGrid.ThingsListAt(position);

                foreach (Thing thing in things)
                {
                    if (thing is Building)
                    {
                        if (thing.HitPoints < thing.MaxHitPoints)
                        {
                            if (this.FieldIntegrity_Current > 1)
                            {
                                //Damage the shield
                                ProcessDamage(1);

                                thing.HitPoints += 1;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Drawing

        public override void Draw()
        {
            base.Draw();
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

            if (this.m_StructuralIntegrityMode)
            {
                //foreach (IntVec3 _Square in this.CellsToProtect)
                //{
                //    DrawSubField(Vectors.IntVecToVec(_Square), 0.8f);
                //}
            }
            else
            {
                //Draw field
                this.DrawField(Vectors.IntVecToVec(base.Position));
            }

            //Initialize the spark particle array
            if (m_SparksParticle[0] == null)
            {
                for (int i = 0; i < m_SparksParticle.Length; i++)
                {
                    m_SparksParticle[i] = new ShieldBlendingParticle(this.DrawPos, (int)Math.Round(((float)i / (float)(m_SparksParticle.Length - 1)) * (float)ShieldBlendingParticle.transitionMax));
                }
            }
            //Animate spark particles
            for (int i = 0, l = m_SparksParticle.Length; i < l; i++)
            {
                m_SparksParticle[i].DrawMe();
            }
        }

        public override void DrawExtraSelectionOverlays()
        {
            //    GenDraw.DrawRadiusRing(base.Position, shieldField.shieldShieldRadius);
        }

        public void DrawSubField(IntVec3 center, float radius)
        {
            this.DrawSubField(Vectors.IntVecToVec(center), radius);
        }

        //Draw the field on map
        public void DrawField(Vector3 center)
        {
            DrawSubField(center, m_Field_Radius);
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
            }

            UnityEngine.Graphics.DrawMesh(EnhancedDevelopment.Shields.Basic.ShieldUtils.Graphics.CircleMesh, matrix, currentMatrialColour, 0);

        }

        #endregion

        #region UI

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (this.IsActive())
            {
                stringBuilder.AppendLine("Shield: " + this.FieldIntegrity_Current + "/" + this.m_FieldIntegrity_Max);
            }
            else if (this.CurrentStatus == enumShieldStatus.Initilising)
            {
                //stringBuilder.AppendLine("Initiating shield: " + ((warmupTicks * 100) / recoverWarmup) + "%");
                stringBuilder.AppendLine("Ready in " + Math.Round(GenTicks.TicksToSeconds(m_WarmupTicksRemaining)) + " seconds.");
                //stringBuilder.AppendLine("Ready in " + m_warmupTicksCurrent + " seconds.");
            }
            else
            {
                stringBuilder.AppendLine("Shield disabled!");
            }

            if (m_Power != null)
            {
                string text = m_Power.CompInspectStringExtra();
                if (!text.NullOrEmpty())
                {
                    stringBuilder.Append(text);
                }
                else
                {
                    stringBuilder.Append("Error, No Power Comp Text.");
                }
            }
            else
            {
                stringBuilder.Append("Error, No Power Comp.");
            }

            return stringBuilder.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            //Add the stock Gizmoes
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            if (m_BlockDirect_Avalable)
            {
                if (m_BlockIndirect_Active)
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

            if (m_BlockIndirect_Avalable)
            {
                if (m_BlockDirect_Active)
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

            if (m_FireSupression_Avalable)
            {
                if (m_FireSupression_Active)
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchFire();
                    act.icon = UI_INDIRECT_ON;
                    act.defaultLabel = "Fire Suppression";
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
                    act.action = () => this.SwitchFire();
                    act.icon = UI_INDIRECT_OFF;
                    act.defaultLabel = "Fire Suppression";
                    act.defaultDesc = "Off";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
            }

            if (m_InterceptDropPod_Avalable)
            {
                if (m_InterceptDropPod_Active)
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


            if (m_StructuralIntegrityMode)
            {
                if (m_RepairMode_Active)
                {

                    Command_Action act = new Command_Action();
                    //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                    act.action = () => this.SwitchShieldRepairMode();
                    act.icon = UI_REPAIR_ON;
                    act.defaultLabel = "Repair Mode";
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
                    act.action = () => this.SwitchShieldRepairMode();
                    act.icon = UI_REPAIR_OFF;
                    act.defaultLabel = "Repair Mode";
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


        }

        private void SwitchDirect()
        {
            m_BlockIndirect_Active = !m_BlockIndirect_Active;
        }

        private void SwitchIndirect()
        {
            m_BlockDirect_Active = !m_BlockDirect_Active;
        }

        private void SwitchFire()
        {
            m_FireSupression_Active = !m_FireSupression_Active;
        }

        private void SwitchInterceptDropPod()
        {
            m_InterceptDropPod_Active = !m_InterceptDropPod_Active;
        }

        private void SwitchVisual()
        {
            m_ShowVisually_Active = !m_ShowVisually_Active;
        }

        private void SwitchShieldRepairMode()
        {
            m_RepairMode_Active = !m_RepairMode_Active;
        }

        #endregion

        #region Saving

        //Saving game
        public override void ExposeData()
        {
            base.ExposeData();

            //  Scribe_Deep.LookDeep(ref shieldField, "shieldField");

            Scribe_Values.Look(ref m_BlockIndirect_Active, "m_BlockIndirect_Active");
            Scribe_Values.Look(ref m_BlockDirect_Active, "m_BlockDirect_Active");
            Scribe_Values.Look(ref m_FireSupression_Active, "m_FireSupression_Active");
            Scribe_Values.Look(ref m_RepairMode_Active, "m_RepairMode_Active");
            Scribe_Values.Look(ref m_ShowVisually_Active, "m_ShowVisually_Active");
            Scribe_Values.Look(ref m_InterceptDropPod_Active, "m_InterceptDropPod_Active");

            Scribe_Values.Look(ref m_BlockIndirect_Avalable, "m_BlockIndirect_Avalable");
            Scribe_Values.Look(ref m_BlockDirect_Avalable, "m_BlockDirect_Avalable");
            Scribe_Values.Look(ref m_FireSupression_Avalable, "m_FireSupression_Avalable");
            Scribe_Values.Look(ref m_InterceptDropPod_Avalable, "m_InterceptDropPod_Avalable");

            Scribe_Values.Look(ref m_StructuralIntegrityMode, "m_StructuralIntegrityMode");

            Scribe_Values.Look(ref m_FieldIntegrity_Max, "m_FieldIntegrity_Max");
            Scribe_Values.Look(ref m_FieldIntegrity_Initial, "m_FieldIntegrity_Initial");
            Scribe_Values.Look(ref m_Field_Radius, "m_Field_Radius");

            Scribe_Values.Look(ref m_PowerRequired_Charging, "m_PowerRequired_Charging");
            Scribe_Values.Look(ref m_PowerRequired_Sustaining, "m_PowerRequired_Sustaining");

            Scribe_Values.Look(ref m_RechargeTickDelayInterval, "m_shieldRechargeTickDelay");
            Scribe_Values.Look(ref m_RecoverWarmupDelayTicks, "m_shieldRecoverWarmup");
            Scribe_Values.Look(ref m_StructuralIntegrityMode, "m_StructuralIntegrityMode");
            Scribe_Values.Look(ref m_StructuralIntegrityMode, "m_StructuralIntegrityMode");

            Scribe_Values.Look(ref m_ColourRed, "m_colourRed");
            Scribe_Values.Look(ref m_ColourGreen, "m_colourGreen");
            Scribe_Values.Look(ref m_ColourBlue, "m_colourBlue");

            Scribe_Values.Look(ref m_WarmupTicksRemaining, "m_WarmupTicksRemaining");

            Scribe_Values.Look(ref m_CurrentStatus, "m_CurrentStatus");
            Scribe_Values.Look(ref m_FieldIntegrity_Current, "m_FieldIntegrity_Current");
        }

        #endregion

    }
}