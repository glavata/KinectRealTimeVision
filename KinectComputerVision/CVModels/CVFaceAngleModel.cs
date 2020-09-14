using KinectComputerVision.CVFrames;
using KinectDataEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectComputerVision.CVModels
{

    public enum CVFaceAngleType
    {
        OutOfBounds = 0,
        Center = 1,
        Left = 2,
        Right = 3,
        Top = 4,
        Bottom = 5
    }

    class CVFaceAngleModel : CVModel
    {


        public CVFaceAngleModel()
        {

        }

        public override void TrainModel()
        {
            throw new NotImplementedException();
        }

        public override int ClassifySample(CVFrame frame)
        {
            CVFaceFrame faceFrame = (CVFaceFrame)frame;
            FaceAngleEuler angles = faceFrame.FaceAngleEuler;
            if(Math.Abs(angles.Y) < 10 && Math.Abs(angles.P) < 10 && Math.Abs(angles.R) < 10)
            {
                return (int)CVFaceAngleType.Center;
            }

            return (int)CVFaceAngleType.OutOfBounds;

            //TODO use database model
        }


    }
}
