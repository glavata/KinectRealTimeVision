using KinectDataEngine;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectComputerVision.CVFrames
{


    class CVFaceFrame : CVFrame
    {
        public ulong TrackingId { get; set; } = 0;
        public FaceAngleEuler FaceAngleEuler { get; set; }
        public CoordSys CoordSys { get; set; }
        public Bitmap RGBCrop { get; set; }

        public CVFaceFrame(Face face, CoordinateMapper mapper, byte[] pixels, bool grayScale = false)
        {
            this.TrackingId = face.TrackingId;
            this.FaceAngleEuler = face.FaceAngleEuler;
            this.CoordSys = face.CoordSys;
            this.RGBCrop = Utilities.CropFaceBitmap(face.Vertices, mapper, pixels, grayScale);
        }


    }
}
