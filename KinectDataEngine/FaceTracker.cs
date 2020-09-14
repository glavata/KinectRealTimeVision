using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace KinectDataEngine
{
    public class FaceTracker
    {
        private ulong trackingId;

        public FaceTracker(KinectSensor sensor, ulong trackingId)
        {
            this.Source = new HighDefinitionFaceFrameSource(sensor);
            this.Reader = this.Source.OpenReader();
            this.Alignment = new FaceAlignment();
            this.TrackingId = trackingId;
            this.Model = new FaceModel();
        }

        public HighDefinitionFaceFrameSource Source { get; set; }

        public HighDefinitionFaceFrameReader Reader { get; set; }

        public FaceAlignment Alignment { get; set; }

        public FaceModelBuilder ModelBuilder { get; set; }

        public FaceModel Model { get; set; }

        public ulong TrackingId
        {
            get
            {
                return this.trackingId;
            }
            set
            {
                this.trackingId = value;
                this.Source.TrackingId = value;
            }
        }

        public bool IsTrackingIdValid
        {
            get { return this.Source.IsTrackingIdValid; }
        }


    }
}

