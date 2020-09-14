using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace KinectDataEngine
{
    public class KinectFrameEventArgs : EventArgs
    {

        public byte[] FramePixels { get; set; }

        public IList<Body> Bodies { get; set; }

        public IList<Face> Faces { get; set; }
        
        public FrameSourceTypes FrameType { get; set; }

        public KinectFrameEventArgs(byte[] framePixels, FrameSourceTypes frameType, IList<Body> bodies, IList<Face> faces)
        {

            this.FrameType = frameType;
            this.FramePixels = framePixels;
            this.Bodies = bodies;
            this.Faces = faces;
        }

    }
}
