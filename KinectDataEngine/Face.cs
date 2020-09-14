using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace KinectDataEngine
{

    public struct FaceAngleEuler
    {
        public float Y { get; set; }
        public float P { get; set; }
        public float R { get; set; }
    }

    public struct CoordSys
    {
        public CameraSpacePoint Center;
        public CameraSpacePoint Top;
        public CameraSpacePoint Front;
        public CameraSpacePoint Side;
    }

    public class Face
    {
        public ulong TrackingId { get; set; } = 0;
        public FaceAngleEuler FaceAngleEuler { get; set; }
        public CoordSys CoordSys { get; set; }
        public IReadOnlyList<CameraSpacePoint> Vertices { get; set; }
        public bool IsTracked { get; set; } = false;
        public bool[] faceFrameAttributes { get; set; }

        public Face()
        {
        }

        public void Update(IReadOnlyList<CameraSpacePoint> vertices)
        {
            this.faceFrameAttributes = faceFrameAttributes;
            this.Vertices = new List<CameraSpacePoint>(vertices);

            this.CoordSys = Utilities.CalculateCoordSystem(vertices);
            CoordSys newC = Utilities.NewCoordSys(CoordSys);
            this.FaceAngleEuler = Utilities.CalculateFaceAngle(this.CoordSys);
            //this.FaceAngleEuler = Utilities.CalculateFaceAngleColor(this.CoordSys, newC, mapper);
            //this.CoordSys = newC;


            
        }







    }
}
