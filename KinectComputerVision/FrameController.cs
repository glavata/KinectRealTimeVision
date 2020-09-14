using KinectComputerVision.CVFrames;
using KinectComputerVision.CVModels;
using KinectDataEngine;
using Microsoft.Kinect;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KinectComputerVision
{
    public class FrameController
    {

        public event FrameReceivedEvent OnFrameReceived;

        public delegate void FrameReceivedEvent(object sender, KinectFrameEventArgs e);

        private DataExtractor dataExtractor;

        private Person[] people;

        private CVModel recModel;


        public FrameController()
        {
            FrameSourceTypes type = FrameSourceTypes.Color;

            dataExtractor = new DataExtractor(type);
            dataExtractor.OnFrameArrived += KinectFrameReceivedEvent;

            this.FrameHeightColor = dataExtractor.FrameHeightColor;
            this.FrameWidthColor = dataExtractor.FrameWidthColor;
            this.FrameHeightDepth = dataExtractor.FrameHeightDepth;
            this.FrameWidthDepth = dataExtractor.FrameWidthDepth;

            this.Sensor = dataExtractor.Sensor;

            this.people = new Person[Sensor.BodyFrameSource.BodyCount];

            //this.recModel = Database.LoadModel(CVModelType.PCA_SVM);

        }



        public void ChangeFrameType(FrameSourceTypes type)
        {
            dataExtractor.UpdateSettings(type);
        }


        public void KinectFrameReceivedEvent(object sender, EventArgs e)
        {
            KinectFrameEventArgs eventArgs = (KinectFrameEventArgs)e;

            for(int i = 0; i < eventArgs.Faces.Count; i++)
            {
                if (eventArgs.Faces[i].IsTracked)
                {
                    if (this.people[i] == null || this.people[i].TrackingId != eventArgs.Faces[i].TrackingId)
                    {
                        this.people[i] = new Person(eventArgs.Faces[i].TrackingId);
                    }

                    CVFaceFrame faceFrame = new CVFaceFrame(eventArgs.Faces[i], this.Sensor.CoordinateMapper, eventArgs.FramePixels);
                    this.people[i].Update(faceFrame, this.recModel);
                }

            }

            OnFrameReceived?.Invoke(this, eventArgs);
        }


        public KinectSensor Sensor { get; }

        public int FrameWidthColor { get; } = 0;

        public int FrameHeightColor { get; } = 0;

        public int FrameWidthDepth { get; } = 0;

        public int FrameHeightDepth { get; } = 0;

    }
}
