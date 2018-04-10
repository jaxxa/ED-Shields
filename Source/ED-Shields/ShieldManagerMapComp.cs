using EnhancedDevelopment.Shields.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Shields.Basic
{
    class ShieldManagerMapComp : MapComponent
    {

        private List<Projectile> m_Projectiles = new List<Projectile>();

        public ShieldManagerMapComp(Map map) : base(map)
        {
            //   map.components.<ShieldManagerMapComp>();
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            // Log.Message("MapCompTick");
        }

        public bool WillBeBlocked(Verse.Projectile projectile)
        {

            IEnumerable<Building_Shield> _ShieldBuildings = map.listerBuildings.AllBuildingsColonistOfClass<Building_Shield>();
            Log.Message("Buildings: " + _ShieldBuildings.Count().ToString());

            //if (_ShieldBuildings.Any(x => (Vector3.Distance(projectile.ExactPosition, x.Position.ToVector3()) <= 5.0f)))
            if (_ShieldBuildings.Any(x =>
            {
                Vector3 _Projetile2DPosition = new Vector3(projectile.ExactPosition.x, 0, projectile.ExactPosition.z);
                float _Distance = Vector3.Distance(_Projetile2DPosition, x.Position.ToVector3());

                Log.Message("Projectile:" + _Projetile2DPosition.ToString());
                Log.Message("Shield:" + x.Position.ToVector3());

                Log.Message("Distance: " + _Distance.ToString());

                if (_Distance <= x.m_Field_Radius)
                {
                    return this.CorrectAngleToIntercept(projectile, x);
                }
                return false;
            }))
            {
                Log.Message("Blocked");
                projectile.Destroy();
                return true;
            }

            return false;
        }


        public Boolean CorrectAngleToIntercept(Projectile pr, Building_Shield shieldBuilding)
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

    }
}

