using HarmonyLib;
using Jaxxa.EnhancedDevelopment.Shields.Shields;
using RimWorld;
using System;
using System.Reflection;
using Verse;
using Verse.Sound;

namespace Jaxxa.EnhancedDevelopment.Shields.Patch.Patches
{
    class PatchDropPod : Patch
    {

        protected override void ApplyPatch(Harmony harmony = null)
        {
            ApplyTickPatch(harmony);
        }

        protected override string PatchDescription()
        {
            return "PatchDropPod";
        }

        protected override bool ShouldPatchApply()
        {
            return true;
        }

        #region "Impact Patch"

        private void ApplyTickPatch(Harmony harmony)
        {

            //Get the Launch Method
            MethodInfo _DropPodIncomingImpact = typeof(RimWorld.DropPodIncoming).GetMethod("Impact", BindingFlags.NonPublic | BindingFlags.Instance);
            Patcher.LogNULL(_DropPodIncomingImpact, "_DropPodIncomingImpact");

            //Get the Launch Prefix Patch
            MethodInfo _DropPodIncomingImpactPrefix = typeof(PatchDropPod).GetMethod("DropPodIncomingImpactPrefix", BindingFlags.Public | BindingFlags.Static);
            Patcher.LogNULL(_DropPodIncomingImpactPrefix, "_DropPodIncomingImpactPrefix");

            //Apply the Prefix Patches
            harmony.Patch(_DropPodIncomingImpact, new HarmonyMethod(_DropPodIncomingImpactPrefix), null);
        }


        // prefix
        // - wants instance, result and count
        // - wants to change count
        // - returns a boolean that controls if original is executed (true) or not (false)
        public static Boolean DropPodIncomingImpactPrefix(ref DropPodIncoming __instance)
        {
            Log.Message("Dp incoming called.");

            if (__instance.Map.GetComponent<ShieldManagerMapComp>().WillDropPodBeIntercepted(__instance)) // if the droppod is being intercepted, prevent it from spawning the contents.
            {
                __instance.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
                SkyfallerShrapnelUtility.MakeShrapnel(__instance.Position, __instance.Map, __instance.shrapnelDirection, __instance.def.skyfaller.shrapnelDistanceFactor, 3, 0, true);
                Find.CameraDriver.shaker.DoShake(__instance.def.skyfaller.cameraShake);
                SoundStarter.PlayOneShot(DamageDefOf.Bomb.soundExplosion, SoundInfo.InMap(new TargetInfo(__instance.Position, __instance.Map)));
                __instance.Destroy(DestroyMode.Vanish);

                return false;
            }

            //Retuen False so the origional method is not executed, overriting the false result.
            return true;
        }

        #endregion
    }
}
