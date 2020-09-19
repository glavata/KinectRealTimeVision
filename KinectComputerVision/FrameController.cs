using KinectComputerVision.CVFrames;
using KinectComputerVision.CVModels;
using KinectDataEngine;
using Microsoft.Kinect;
using System;
using System.Data.Entity.Migrations.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KinectComputerVision
{
    public class FrameController
    {

        private Settings settings;

        public event FrameReceivedEvent OnFrameReceived;

        public delegate void FrameReceivedEvent(object sender, KinectFrameEventArgs e);

        private DataExtractor dataExtractor;

        private Person[] people;

        private CVModel recModel;

        private Database db;

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

            this.settings = Settings.Instance();
            this.db = new Database();
            //this.recModel = Database.LoadModel(CVModelType.PCA_SVM);

        }


        public void KinectFrameReceivedEvent(object sender, EventArgs e)
        {
            KinectFrameEventArgs eventArgs = (KinectFrameEventArgs)e;

            Parallel.For(0, eventArgs.Faces.Count, i =>
            {
                if (eventArgs.Faces[i].IsTracked)
                {
                    if (this.people[i] == null || this.people[i].TrackingId != eventArgs.Faces[i].TrackingId)
                    {
                        this.people[i] = new Person(eventArgs.Faces[i].TrackingId);
                    }

                    Task.Factory.StartNew(() => this.UpdateDatabase(this.people[i], eventArgs.Faces[i], Sensor.CoordinateMapper, eventArgs.FramePixels));

                    //if ((bool)this.settings.GetSettingValue("General", "ManualFrameCollection"))
                    //{
                    //    Task.Factory.StartNew(() => ManualFrameSave(eventArgs.Faces[i], Sensor.CoordinateMapper, eventArgs.FramePixels));
                    //}

                    //CVFaceFrame faceFrame = new CVFaceFrame(eventArgs.Faces[i], this.Sensor.CoordinateMapper, eventArgs.FramePixels);
                    //this.people[i].Update(faceFrame, this.recModel);
                }
            });



            OnFrameReceived?.Invoke(this, eventArgs);
        }


        public void UpdateDatabase(Person person, Face face, CoordinateMapper mapper, byte[] pixels)
        {

        }

        public void ChangeFrameType(FrameSourceTypes type)
        {
            dataExtractor.UpdateSettings(type);
        }

        public void UpdateSetting(string type, object val)
        {
            if(type == "ManualFrameCollection")
            {
                //this.settings.UpdateSettings("General", type, val);
                this.settings.UpdateSettingsManual(val);
            }
        }

        public void ManualFrameSave(Face f, CoordinateMapper mapper, byte[] pixels)
        {
            ulong tr_id = f.TrackingId;
            float y = f.FaceAngleEuler.Y;
            float p = f.FaceAngleEuler.P;
            float r = f.FaceAngleEuler.R;

            if (Math.Abs(y) > 30.0 || Math.Abs(p) > 30.0 || Math.Abs(r) > 30.0)
                return;

            // float bin = 5;

            String folderPerson = "manual\\" + tr_id.ToString();


            String fileName = String.Format("{0}\\{1}_{2}_{3}.jpg", folderPerson, (int)y, (int)p, (int)r);
            DirectoryInfo di = Directory.CreateDirectory(folderPerson);

            //for(int y_i = -30; y_i < 30; y_i+=5)
            //{
            //    for (int p_i = -30; p_i < 30; p_i += 5)
            //    {
            //        for (int r_i = -30; r_i < 30; r_i += 5)
            //        {
            //            DirectoryInfo di_a = Directory.CreateDirectory(String.Format("{0}\\[({1}_{2})({3}_{4})({5}_{6})]", folderPerson, y_i, y_i + 5, p_i, p_i+5, r_i, r_i+5));
            //        }
            //    }
            //}


            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Bitmap rgb = Utilities.CropFaceBitmap(f.Vertices, mapper, pixels, false);
                    rgb.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }


        }


        public KinectSensor Sensor { get; }

        public int FrameWidthColor { get; } = 0;

        public int FrameHeightColor { get; } = 0;

        public int FrameWidthDepth { get; } = 0;

        public int FrameHeightDepth { get; } = 0;

    }
}
