using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.Prometheus.Shields.Utilities
{
    class VectorsUtils
    {
        public static double EuclDist(IntVec3 a, IntVec3 b)
        {
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));

        }
        public static double VectorSize(IntVec3 a)
        {
            return Math.Sqrt((a.x) * (a.x) + (a.y) * (a.y) + (a.z) * (a.z));
        }
        public static IntVec3 vecFromAngle(float angle1, float angle2, float r)
        {
            IntVec3 ret = new IntVec3(
                   (int)(r * Math.Sin(angle1) * Math.Cos(angle2)),
                   (int)(r * Math.Sin(angle1) * Math.Sin(angle2)),
                   (int)(r * Math.Cos(angle1))
                );
            return ret;
        }
        public static double vectorAngleA(IntVec3 a)
        {
            double r = VectorSize(a);
            return Math.Acos(a.z / r);
        }
        public static IntVec3 randomDirection(float r)
        {
            return vecFromAngle(UnityEngine.Random.Range(0, 360), 0, r);
        }

        public static Vector3 IntVecToVec(IntVec3 from)
        {
            return new Vector3(from.x, from.y, from.z);
        }

        public static IntVec3 VecToIntVec(Vector3 from)
        {
            return new IntVec3((int)from.x, (int)from.y, (int)from.z);
        }

    }
}
