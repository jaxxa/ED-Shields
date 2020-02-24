using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Prometheus.Shields
{
    class ITab_ShieldGenerator : ITab
    {

        //private Comp_ShieldGenerator _CachedComp;

        //public ITab_ShieldGenerator() : base()
        //{
        //    _CachedComp = 

        //}

        private static readonly Vector2 WinSize = new Vector2(500f, 400f);

        private Comp_ShieldGenerator SelectedCompShieldGenerator
        {
            get
            {
                Thing thing = Find.Selector.SingleSelectedThing;
                MinifiedThing minifiedThing = thing as MinifiedThing;
                if (minifiedThing != null)
                {
                    thing = minifiedThing.InnerThing;
                }
                if (thing == null)
                {
                    return null;
                }
                return thing.TryGetComp<Comp_ShieldGenerator>();
            }
        }

        public override bool IsVisible
        {
            get
            {
                return this.SelectedCompShieldGenerator != null;
            }
        }

        public ITab_ShieldGenerator()
        {
            base.size = ITab_ShieldGenerator.WinSize;
            base.labelKey = "Shield";
        }

        protected override void FillTab()
        {

            Vector2 winSize = ITab_ShieldGenerator.WinSize;
            float x = winSize.x;
            Vector2 winSize2 = ITab_ShieldGenerator.WinSize;
            Rect rect = new Rect(0f, 0f, x, winSize2.y).ContractedBy(10f);
            //Rect rect2 = rect;
            //Text.Font = GameFont.Medium;
            //Widgets.Label(rect2, "Shield Generator Label Rec2");
            //if (ITab_Art.cachedImageSource != this.SelectedCompArt || ITab_Art.cachedTaleRef != this.SelectedCompArt.TaleRef)
            //{
            //    ITab_Art.cachedImageDescription = this.SelectedCompArt.GenerateImageDescription();
            //    ITab_Art.cachedImageSource = this.SelectedCompArt;
            //    ITab_Art.cachedTaleRef = this.SelectedCompArt.TaleRef;
            //}
            //Rect rect3 = rect;
            //rect3.yMin += 35f;
            //Text.Font = GameFont.Small;
            //Widgets.Label(rect3, "ShieldGenerator Rec3");

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = 250f;
            listing_Standard.Begin(rect);


            listing_Standard.GapLine(12f);
            listing_Standard.Label("Charge: " + this.SelectedCompShieldGenerator.FieldIntegrity_Current + " / " + this.SelectedCompShieldGenerator.m_FieldIntegrity_Max);

            listing_Standard.Gap(12f);

            listing_Standard.Label("Radius: " + this.SelectedCompShieldGenerator.m_FieldRadius_Requested + " / " + this.SelectedCompShieldGenerator.m_FieldRadius_Avalable);
            listing_Standard.IntAdjuster(ref this.SelectedCompShieldGenerator.m_FieldRadius_Requested, 1, 1);
            if (this.SelectedCompShieldGenerator.m_FieldRadius_Requested > this.SelectedCompShieldGenerator.m_FieldRadius_Avalable)
            {
                this.SelectedCompShieldGenerator.m_FieldRadius_Requested = this.SelectedCompShieldGenerator.m_FieldRadius_Avalable;
            }

            //Direct
            if (this.SelectedCompShieldGenerator.BlockDirect_Active())
            {
                if (listing_Standard.ButtonText("Toggle Direct: Active"))
                {
                    this.SelectedCompShieldGenerator.SwitchDirect();
                }
            } 
            else
            {
                if (listing_Standard.ButtonText("Toggle Direct: Inactive"))
                {
                    this.SelectedCompShieldGenerator.SwitchDirect();
                }

            }

            //Indirect
            if (this.SelectedCompShieldGenerator.BlockIndirect_Active())
            {
                if (listing_Standard.ButtonText("Toggle Indirect: Active"))
                {
                    this.SelectedCompShieldGenerator.SwitchIndirect();
                }
            }
            else
            {
                if (listing_Standard.ButtonText("Toggle Indirect: Inactive"))
                {
                    this.SelectedCompShieldGenerator.SwitchIndirect();
                }

            }

            if (this.SelectedCompShieldGenerator.IsInterceptDropPod_Avalable())
            {
                if (this.SelectedCompShieldGenerator.IntercepDropPod_Active())
                {
                    if (listing_Standard.ButtonText("Toggle DropPod Intercept: Active"))
                    {
                        this.SelectedCompShieldGenerator.SwitchInterceptDropPod();
                    }
                }
                else
                {
                    if (listing_Standard.ButtonText("Toggle DropPod Intercept: Inactive"))
                    {
                        this.SelectedCompShieldGenerator.SwitchInterceptDropPod();
                    }

                }
                               
            }
            else
            {
                listing_Standard.Label("DropPod Intercept Unavalable");
            }

            if (this.SelectedCompShieldGenerator.IdentifyFriendFoe_Active())
            {
                listing_Standard.Label("IFF Active");

            }
            else
            {
                listing_Standard.Label("IFF Inactive");
            }

            listing_Standard.End();
        }

    } //Class

} //NameSpace

