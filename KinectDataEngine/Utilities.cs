using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDataEngine
{
    public static class Utilities
    {

        public static CameraSpacePoint SubtractFrom(this CameraSpacePoint cur, CameraSpacePoint other)
        {
            return new CameraSpacePoint(){ X = other.X - cur.X, Y = other.Y - cur.Y, Z = other.Z - cur.Z };
        }

        public static CameraSpacePoint Add(this CameraSpacePoint cur, CameraSpacePoint other)
        {
            return new CameraSpacePoint() { X = other.X + cur.X, Y = other.Y + cur.Y, Z = other.Z + cur.Z };
        }

        public static CameraSpacePoint Multiply(this CameraSpacePoint cur, float scalar)
        {
            return new CameraSpacePoint() { X = cur.X * scalar, Y = cur.Y * scalar, Z = cur.Z * scalar };
        }

        public static float DotProduct(this CameraSpacePoint cur, CameraSpacePoint other)
        {
            return cur.X * other.X + cur.Y * other.Y + cur.Z * other.Z;
        }

        public static CameraSpacePoint CrossProduct(this CameraSpacePoint cur, CameraSpacePoint other)
        {
            return new CameraSpacePoint()
            {
                X = cur.Y * other.Z - cur.Z * other.Y,
                Y = cur.Z * other.X - cur.X * other.Z,
                Z = cur.X * other.Y - cur.Y * other.X
            };
        }

        public static float AngleBetween(this CameraSpacePoint cur, CameraSpacePoint other)
        {
            float curMag = cur.Magnitude();
            float othMag = other.Magnitude();

            float angle = (float)(Math.Atan(cur.DotProduct(other) / (curMag * othMag)) * 180/ Math.PI);
            return angle;
        }

        public static float Magnitude(this CameraSpacePoint cur)
        {
            return (float)Math.Sqrt(cur.X * cur.X + cur.Y * cur.Y + cur.Z * cur.Z);
        }

        public static CameraSpacePoint Norm(this CameraSpacePoint cur)
        {
            float mag = cur.Magnitude();
            return new CameraSpacePoint() { X = cur.X / mag, Y = cur.Y / mag, Z = cur.Z / mag };
        }

        public static float DistTo(this CameraSpacePoint cur, CameraSpacePoint other)
        {
            return (float)Math.Sqrt(Math.Pow(cur.X - other.X, 2) + Math.Pow(cur.Y - other.Y, 2) + Math.Pow(cur.Z - other.Z, 2));
        }

        public static CoordSys GramSchmidt(CameraSpacePoint first, CameraSpacePoint sec, CameraSpacePoint third, CameraSpacePoint center)
        {
            CameraSpacePoint secNew = first.ProjOperator(sec).SubtractFrom(sec);
            CameraSpacePoint thirdNew = secNew.ProjOperator(third).Add(first.ProjOperator(third)).SubtractFrom(third);

            //float res1 = first.DotProduct(secNew);
            //float res2 = first.DotProduct(thirdNew);
            //float res3 = secNew.DotProduct(thirdNew);

            return new CoordSys() { Center = center, Top = center.Add(secNew),
                                                    Front = center.Add(first),
                                                    Side = center.Add(thirdNew)
            };
        }

        public static CameraSpacePoint ProjOperator(this CameraSpacePoint first, CameraSpacePoint sec)
        {
            return first.Multiply(sec.DotProduct(first) / first.DotProduct(first));
        }

        public static CoordSys CalculateCoordSystem(IReadOnlyList<CameraSpacePoint> vertices)
        {
            CameraSpacePoint noseTip = vertices[18];
            CameraSpacePoint foreheadCenter = vertices[28];
            CameraSpacePoint leftCheekBone = vertices[458];

            CameraSpacePoint avgPoint = new CameraSpacePoint()
            {
                X = vertices.Average(a => a.X),
                Y = vertices.Average(a => a.Y),
                Z = vertices.Average(a => a.Z)
            };

            CameraSpacePoint translTop = avgPoint.SubtractFrom(foreheadCenter);
            CameraSpacePoint translFront = avgPoint.SubtractFrom(noseTip);
            CameraSpacePoint translSide = avgPoint.SubtractFrom(leftCheekBone);

            return Utilities.GramSchmidt(translFront, translTop, translSide, avgPoint);
        }

        public static FaceAngleEuler CalculateFaceAngle(CoordSys coordSys, bool withRelToCamera = true)
        {
            CameraSpacePoint transSide = coordSys.Center.SubtractFrom(coordSys.Side);
            CameraSpacePoint transFront = coordSys.Center.SubtractFrom(coordSys.Front);
            CameraSpacePoint transTop = coordSys.Center.SubtractFrom(coordSys.Top);

  
            CameraSpacePoint transSide2D = transSide; transSide2D.Y = 0;
            CameraSpacePoint transFront2D = transFront; transFront2D.X = 0;
            CameraSpacePoint transTop2D = transTop; transTop2D.Z = 0;
     

            transSide2D = transSide2D.Norm();
            transFront2D = transFront2D.Norm();
            transTop2D = transTop2D.Norm();

            FaceAngleEuler angles = new FaceAngleEuler()
            {
                R = (float)(Math.Asin(transTop2D.X) * 180 / Math.PI),
                Y = (float)(Math.Asin(transSide2D.Z) * 180 / Math.PI),
                P = (float)(Math.Asin(transFront2D.Y) * 180 / Math.PI)
            };

            return angles;
        }

        public static FaceAngleEuler CalculateFaceAngleColor(CoordSys coordSys, CoordSys newCoord, CoordinateMapper mapper)
        {


            CameraSpacePoint transSide = coordSys.Center.SubtractFrom(coordSys.Side);
            CameraSpacePoint transFront = coordSys.Center.SubtractFrom(coordSys.Front);
            CameraSpacePoint transTop = coordSys.Center.SubtractFrom(coordSys.Top);

            CameraSpacePoint transSideN = newCoord.Center.SubtractFrom(newCoord.Side);
            CameraSpacePoint transFrontN = newCoord.Center.SubtractFrom(newCoord.Front);
            CameraSpacePoint transTopN = newCoord.Center.SubtractFrom(newCoord.Top);

            CameraSpacePoint transSide2D = transSide; transSide2D.Y = 0;
            CameraSpacePoint transFront2D = transFront; transFront2D.X = 0;
            CameraSpacePoint transTop2D = transTop; transTop2D.Z = 0;

            transSide2D = transSide2D.Norm();
            transFront2D = transFront2D.Norm();
            transTop2D = transTop2D.Norm();

            CameraSpacePoint transSideN2D = transSideN; transSideN2D.Y = 0;
            CameraSpacePoint transFrontN2D = transFrontN; transFrontN2D.X = 0;
            CameraSpacePoint transTopN2D = transTopN; transTopN2D.Z = 0;

            transSideN2D = transSideN2D.Norm();
            transFrontN2D = transFrontN2D.Norm();
            transTopN2D = transTopN2D.Norm();


            //FaceAngleEuler angles = new FaceAngleEuler()
            //{
            //    R = (float)(Math.Asin(transTop2D.X * transTopN2D.X) * 180 / Math.PI),
            //    Y = (float)(Math.Asin(transSide2D.Z * transSideN2D.Z) * 180 / Math.PI),
            //    P = (float)(Math.Asin(transFront2D.Y * transFrontN2D.Y) * 180 / Math.PI)
            //};



            FaceAngleEuler angles = new FaceAngleEuler()
            {
                R = (float)(Math.Acos(transTop2D.DotProduct(transTopN2D)) * 180 / Math.PI),
                Y = (float)(Math.Acos(transSide2D.DotProduct(transSideN2D)) * 180 / Math.PI),
                P = (float)(Math.Acos(transFront2D.DotProduct(transFrontN2D)) * 180 / Math.PI)
            };

            return angles;
        }


        public static CoordSys NewCoordSys(CoordSys coordSys)
        {
            CoordSys newCoord = new CoordSys() { Center = coordSys.Center, Top = coordSys.Top, Side = coordSys.Side, Front = coordSys.Front };

            newCoord.Side.Y = coordSys.Center.Y;
            newCoord.Top.X = coordSys.Center.X;

            CameraSpacePoint transSideN = newCoord.Center.SubtractFrom(newCoord.Side);
            CameraSpacePoint transFrontN = newCoord.Center.SubtractFrom(newCoord.Front);
            CameraSpacePoint transTopN = newCoord.Center.SubtractFrom(newCoord.Top);

            transFrontN = transSideN.CrossProduct(transTopN);

            newCoord.Top = transTopN.Add(coordSys.Center);
            newCoord.Side = transSideN.Add(coordSys.Center);
            newCoord.Front = transFrontN.Add(coordSys.Center);
            //newCoord.Front = newCoord.Side.CrossProduct(newCoord.Top);
            //newCoord.Front.X = coordSys.Center.X;
            //newCoord.Front.Y = coordSys.Center.Y;

            return newCoord;
        }


    }
}
