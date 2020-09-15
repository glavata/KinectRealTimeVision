using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KinectComputerVision
{
    struct FacePoseCollectionSettings
    {
        public float FaceAngleMarginYaw;
        public float FaceAngleMarginPitch;
        public float FaceAngleMarginRoll;
        public bool SeparateFolderPerson;
        public bool SeparateFolderBins;
        public string TargetDirectory;
    }

    struct GeneralSettings
    {
        public bool ManualFrameCollection;
    }

    class Settings
    {


        private static Settings _instance;

        protected Settings()
        {
            this.GeneralSettings = new GeneralSettings();
            this.FacePoseCollectionSettings = new FacePoseCollectionSettings();

            GeneralSettings g = this.GeneralSettings;
            g.ManualFrameCollection = false;

            FacePoseCollectionSettings fp = this.FacePoseCollectionSettings;
            fp.FaceAngleMarginYaw = 30;
            fp.FaceAngleMarginPitch = 30;
            fp.FaceAngleMarginRoll = 30;
            fp.SeparateFolderPerson = true;
            fp.SeparateFolderBins = true;
            fp.TargetDirectory = "manual";
        }

        public static Settings Instance()
        {

            if (_instance == null)
            {
                _instance = new Settings();
            }

            return _instance;
        }


        public void UpdateSettings(string type, string field, object val)
        {
            var propertySetting = this.GetType().GetProperty(type + "Settings").GetValue(this, null);
            propertySetting.GetType().GetField(field).SetValue(propertySetting, (bool)val);
        }


        public void UpdateSettingsManual(object val)
        {
            GeneralSettings g = new GeneralSettings();
            g.ManualFrameCollection = (bool)val;
            this.GeneralSettings = g;
        }

        public object GetSettingValue(string type, string field)
        {
            var propertySetting = this.GetType().GetProperty(type + "Settings").GetValue(this, null);
            return propertySetting.GetType().GetField(field).GetValue(propertySetting);
        }


        public GeneralSettings GeneralSettings { get; private set; }
        public FacePoseCollectionSettings FacePoseCollectionSettings { get; private set; }

    }

}
