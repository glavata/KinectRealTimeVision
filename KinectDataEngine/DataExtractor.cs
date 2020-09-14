using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDataEngine
{

    public class DataExtractor
    {
        private const float InfrDataScale = 0.75f;
        private const float InfrMinVal = 0.01f;
        private const float InfrMaxVal = 1.0f;
        private const int MapDepthToByte = 8000 / 256;

        private byte[] _pixelsColor = null;
        private byte[] _pixelsInfra = null;
        private byte[] _pixelsDepth = null;

        MultiSourceFrameReader _reader;
        IList<Body> _bodies;
        IList<Face> _faces;
        private int bodyCountTotal;
        private FrameSourceTypes frameSourceType;


        public event FrameArrivedEvent OnFrameArrived;

        public delegate void FrameArrivedEvent(object sender, KinectFrameEventArgs e);
        private FrameDescription depthFrameDescription;
        private FrameDescription infraredFrameDescription;

        public DataExtractor(FrameSourceTypes type)
        {
            Sensor = KinectSensor.GetDefault();

            if (Sensor != null)
            {
                Sensor.Open();

                FrameWidthColor = Sensor.ColorFrameSource.FrameDescription.Width;
                FrameHeightColor = Sensor.ColorFrameSource.FrameDescription.Height;
                FrameWidthDepth = Sensor.InfraredFrameSource.FrameDescription.Width;
                FrameHeightDepth = Sensor.InfraredFrameSource.FrameDescription.Height;

                this.depthFrameDescription = Sensor.DepthFrameSource.FrameDescription;
                this.infraredFrameDescription = Sensor.InfraredFrameSource.FrameDescription;

                _pixelsColor = new byte[FrameWidthColor * FrameHeightColor * 4];
                _pixelsInfra = new byte[FrameWidthDepth * FrameHeightDepth];
                _pixelsDepth = new byte[FrameWidthDepth * FrameHeightDepth];

                this.frameSourceType = type;

                bodyCountTotal = Sensor.BodyFrameSource.BodyCount;

                this.FaceTrackers = new FaceTracker[bodyCountTotal];
                this._faces = new Face[bodyCountTotal];
                this._bodies = new Body[bodyCountTotal]; 

                for (int i = 0; i < this.bodyCountTotal; i++)
                {
                    this.FaceTrackers[i] = new FaceTracker(Sensor, 0);
                    this.FaceTrackers[i].Source.TrackingIdLost += HdFaceSource_TrackingIdLost;
                    this._faces[i] = new Face();
                }

                _reader = Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Color | FrameSourceTypes.Infrared | FrameSourceTypes.Depth);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            }

        }


        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            byte[] _pixelsFrame = null;

            var reference = e.FrameReference.AcquireFrame();

            if (this.frameSourceType == FrameSourceTypes.Color)
            {
                using (var frame = reference.ColorFrameReference.AcquireFrame())
                {
                    if (frame != null)
                    {
                        if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
                        {
                            frame.CopyRawFrameDataToArray(_pixelsColor);
                        }
                        else
                        {
                            frame.CopyConvertedFrameDataToArray(_pixelsColor, ColorImageFormat.Bgra);
                        }
                        _pixelsFrame = _pixelsColor;
                    }
                }
            }
            else if(this.frameSourceType == FrameSourceTypes.Infrared)
            {
                using (var frame = reference.InfraredFrameReference.AcquireFrame())
                {
                    if (frame != null)
                    {
                        using (KinectBuffer infrBuffer = frame.LockImageBuffer())
                        {

                            if ((infraredFrameDescription.Width * infraredFrameDescription.Height) == (infrBuffer.Size / infraredFrameDescription.BytesPerPixel))
                            {
                                this.ProcessInfraredFrameData(ref _pixelsInfra, infrBuffer.UnderlyingBuffer, infrBuffer.Size);
                            }
                        }
                        _pixelsFrame = _pixelsInfra;
                    }
                }
            }
            else if(this.frameSourceType == FrameSourceTypes.Depth)
            {
                using (var frame = reference.DepthFrameReference.AcquireFrame())
                {
                    if (frame != null)
                    {
                        using (KinectBuffer depthBuffer = frame.LockImageBuffer())
                        {
                            if ((depthFrameDescription.Width * depthFrameDescription.Height) == (depthBuffer.Size / depthFrameDescription.BytesPerPixel))
                            {

                                ushort maxDepth = ushort.MaxValue;

                                ProcessDepthFrameData(ref _pixelsDepth, depthBuffer.UnderlyingBuffer, depthBuffer.Size, frame.DepthMinReliableDistance, maxDepth);
                            }
                        }
                        _pixelsFrame = _pixelsDepth;
                    }
                }
            }

            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _bodies = new Body[bodyCountTotal];
                    frame.GetAndRefreshBodyData(_bodies);

                    for (int i = 0; i < bodyCountTotal; i++)
                    {
                        FaceTracker curTrack = this.FaceTrackers[i];
                        if (!curTrack.Source.IsTrackingIdValid)
                        {
                            if (_bodies[i].IsTracked)
                            {
                                this.FaceTrackers[i].TrackingId = _bodies[i].TrackingId;
                                this._faces[i].TrackingId = _bodies[i].TrackingId;
                            }
                        }
                        if (curTrack.Reader != null)
                        {
                            HighDefinitionFaceFrame hdFrame = curTrack.Reader.AcquireLatestFrame();

                            if (hdFrame != null)
                            {
                                this._faces[i].IsTracked = hdFrame.IsFaceTracked;
                                if (hdFrame.FaceModel != null && hdFrame.IsFaceTracked)
                                {
                                    hdFrame.GetAndRefreshFaceAlignmentResult(curTrack.Alignment);
                                    var vertices = curTrack.Model.CalculateVerticesForAlignment(curTrack.Alignment);
                                    this._faces[i].Update(vertices);
                                }
                            }
                        }

                    }

                }
            }

            KinectFrameEventArgs data = new KinectFrameEventArgs(_pixelsFrame, this.frameSourceType, _bodies, _faces);
            OnFrameArrived?.Invoke(this, data);
        }


        private unsafe void ProcessDepthFrameData(ref byte[] depthPixels, IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {

            ushort* frameData = (ushort*)depthFrameData;

            for (int i = 0; i < (int)(depthFrameDataSize / depthFrameDescription.BytesPerPixel); ++i)
            {

                ushort depth = frameData[i];

                depthPixels[i] = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MapDepthToByte) : 0);
            }
        }

        private unsafe void ProcessInfraredFrameData(ref byte[] infraPixels, IntPtr infrFrameData, uint infrFrameDataSize)
        {
            ushort* frameData = (ushort*)infrFrameData;

            for (int i = 0; i < (int)(infrFrameDataSize / infraredFrameDescription.BytesPerPixel); ++i)
            {
                infraPixels[i] = (byte)((float)Math.Min(InfrMaxVal, (float)frameData[i] / ushort.MaxValue * InfrDataScale * (1.0f - InfrMinVal) + InfrMinVal) * 255);
            }
        }


        public void UpdateSettings(FrameSourceTypes type)
        {
            this.frameSourceType = type;
        }


        void HdFaceSource_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            var faceTracker = this.FaceTrackers.FirstOrDefault(a => a.TrackingId == e.TrackingId);

            if (faceTracker != null)
            {
                faceTracker = new FaceTracker(Sensor, 0);
            }
        }

        public FaceTracker[] FaceTrackers { get; set; }
        public KinectSensor Sensor { get; }
        public int FrameWidthColor { get; } = 0;
        public int FrameHeightColor { get; } = 0;
        public int FrameWidthDepth { get; } = 0;
        public int FrameHeightDepth { get; } = 0;


    }
}
