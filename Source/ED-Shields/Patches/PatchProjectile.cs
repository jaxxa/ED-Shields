using EnhancedDevelopment.Shields;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Shields
{
    class PatchProjectile : Patch
    {

        protected override void ApplyPatch(HarmonyInstance harmony = null)
        {

            this.ApplyLaunchPatch(harmony);
            this.ApplyTickPatch(harmony);

        }
        
        protected override string PatchDescription()
        {
            return "PatchProjectile";
        }

        protected override bool ShouldPatchApply()
        {
            return true;
            //return Mod_EnhancedOptions.Settings.Plant24HEnabled;
        }


        #region "Launch Patch"

        private void ApplyLaunchPatch(HarmonyInstance harmony)
        {

            //Get the Launch Method
            Type[] _TypeArray = new Type[] { typeof(Verse.Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(Thing), typeof(Thing) };
            MethodInfo _ProjectileLaunch = typeof(Verse.Projectile).GetMethod("Launch", _TypeArray);
            Patcher.LogNULL(_ProjectileLaunch, "_ProjectileLaunch");

            //Get the Launch Prefix Patch
            MethodInfo _ProjectileLaunchPrefix = typeof(PatchProjectile).GetMethod("ProjectileLaunchPrefix", BindingFlags.Public | BindingFlags.Static);
            Patcher.LogNULL(_ProjectileLaunchPrefix, "_ProjectileLaunchPrefix");

            //Apply the Prefix Patches
            harmony.Patch(_ProjectileLaunch, new HarmonyMethod(_ProjectileLaunchPrefix), null);
        }


        // prefix
        // - wants instance, result and count
        // - wants to change count
        // - returns a boolean that controls if original is executed (true) or not (false)
        public static Boolean ProjectileLaunchPrefix()
        {
            Log.Message("Created Projectile");
            //This is the result that will be used, note that it was passed as a ref.
            //  __result = false;

            //Retuen False so the origional method is not executed, overriting the false result.
            return true;
        }

        #endregion


        #region "Tick Patch"

        private void ApplyTickPatch(HarmonyInstance harmony)
        {

            //Get the Launch Method
            MethodInfo _ProjectileTick = typeof(Verse.Projectile).GetMethod("Tick");
            Patcher.LogNULL(_ProjectileTick, "_ProjectileTick");

            //Get the Launch Prefix Patch
            MethodInfo _ProjectileTickPrefix = typeof(PatchProjectile).GetMethod("ProjectileTickPrefix", BindingFlags.Public | BindingFlags.Static);
            Patcher.LogNULL(_ProjectileTickPrefix, "_ProjectileTickPrefix");

            //Apply the Prefix Patches
            harmony.Patch(_ProjectileTick, new HarmonyMethod(_ProjectileTickPrefix), null);
        }


        // prefix
        // - wants instance, result and count
        // - wants to change count
        // - returns a boolean that controls if original is executed (true) or not (false)
        public static Boolean ProjectileTickPrefix(ref Projectile __instance)
        {
            Log.Message("Ticking Projectile");
            //This is the result that will be used, note that it was passed as a ref.
            //  __result = false;

            if (__instance.Map.GetComponent<ShieldManagerMapComp>().WillBeBlocked(__instance))
            {
                return false;
            }


            //Retuen False so the origional method is not executed, overriting the false result.
            return true;
        }

        #endregion
    }
}
