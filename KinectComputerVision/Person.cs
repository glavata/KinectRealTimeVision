using KinectComputerVision.CVFrames;
using KinectComputerVision.CVModels;
using KinectDataEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectComputerVision
{
    public enum PersonStatus
    {

        Recognizing,
        Recognized,
        Training,
        Trained
    }

    public class Person
    {
        public ulong TrackingId {get; set;} = 0;

        public bool IsIdentified { get; set; } = false;

        public PersonStatus Status { get; set; }

        public Person(ulong tr_id)
        {
            this.TrackingId = tr_id;
            this.Status = PersonStatus.Recognizing;
        }

        public void Update(CVFaceFrame face, CVModel recModel)
        {

            switch (this.Status)
            {
                case PersonStatus.Recognizing:
                    if(recModel.ClassifySample(face) == 1)
                    {
                        recModel.ClassifySample(face);
                    }
                    break;

                case PersonStatus.Training:

                    break;
            }
        }

    }
}
