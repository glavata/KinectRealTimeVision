using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.ML;

using Emgu.CV.Structure;


namespace KinectComputerVision.CVAlgorithms
{


    public class CV_SVM : CV_Classifier
    {

        private SVM innerSVM;
        private bool trained = false;

        public CV_SVM(SVM.SvmKernelType kType, float C, float degree, float gamma)
        {
            innerSVM = new SVM();
            innerSVM.SetKernel(kType);
            innerSVM.C = C;
            innerSVM.Degree = degree;
            innerSVM.Gamma = gamma;
            innerSVM.Coef0 = 0;
            innerSVM.Nu = 0;
            innerSVM.P = 0;
            innerSVM.TermCriteria = new MCvTermCriteria(100, 1.0e-6);
        }

        public void Fit(Mat X, Mat y)
        {
            TrainData td = new TrainData(X, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, y);

            innerSVM.Train(td);
            trained = true;
        }

        public float Predict(Mat X)
        {
            return innerSVM.Predict(X);
        }




    }
}
