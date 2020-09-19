using KinectComputerVision.CVFrames;
using KinectDataEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectComputerVision
{
    public enum CVModelType
    {
        PCA_SVM
    }

    public abstract class CVModel
    {


        public CVModel()
        {

        }

        public void LoadData()
        {

        }

        public void LoadDataBulk()
        {

        }

        public abstract void TrainModel();

        public abstract int ClassifySample(CVFrame frame);

    }
}
