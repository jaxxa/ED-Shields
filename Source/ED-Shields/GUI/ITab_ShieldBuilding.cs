using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Shields.Basic.GUI
{
    class ITab_ShieldBuilding : ITab
    {

        private static readonly Vector2 WinSize = new Vector2(400f, 300f);

        public ITab_ShieldBuilding()
        {
            base.size = ITab_ShieldBuilding.WinSize;
            base.labelKey = "Shield";
        }


        protected override void FillTab()
        {
            //throw new NotImplementedException();

            Vector2 winSize = ITab_ShieldBuilding.WinSize;
            float x = winSize.x;
            Vector2 winSize2 = ITab_ShieldBuilding.WinSize;
            Rect rect = new Rect(0f, 0f, x, winSize2.y).ContractedBy(10f);
            Rect rect2 = rect;
            Text.Font = GameFont.Medium;
           // Widgets.Label(rect2, this.SelectedCompArt.Title);
            Widgets.Label(rect2, "Shield Status");
            //if (ITab_Art.cachedImageSource != this.SelectedCompArt || ITab_Art.cachedTaleRef != this.SelectedCompArt.TaleRef)
            //{
            //    ITab_Art.cachedImageDescription = this.SelectedCompArt.GenerateImageDescription();
            //    ITab_Art.cachedImageSource = this.SelectedCompArt;
            //    ITab_Art.cachedTaleRef = this.SelectedCompArt.TaleRef;
            //}
            Rect rect3 = rect;
            rect3.yMin += 35f;
            Text.Font = GameFont.Small;
            Widgets.Label(rect3, "More Descriptions");
        }
    }
}
