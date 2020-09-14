using KinectComputerVision;
using KinectDataEngine;
using Microsoft.Kinect;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace KinectView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap _bitmapColor;
        private WriteableBitmap _bitmapDepth;
        private DrawingEngine drawEngine;
        private KinectSensor _sensor;
        private int frameWidthC;
        private int frameHeightC;
        private int frameWidthD;
        private int frameHeightD;
        private FrameController controller;

        public MainWindow(FrameController controller)
        {
            this.frameHeightC = controller.FrameHeightColor;
            this.frameWidthC = controller.FrameWidthColor;
            this.frameHeightD = controller.FrameHeightDepth;
            this.frameWidthD = controller.FrameWidthDepth;
            this._sensor = controller.Sensor;

            _bitmapColor = new WriteableBitmap(frameWidthC, frameHeightC, 96.0, 96.0, PixelFormats.Bgra32, null);
            _bitmapDepth = new WriteableBitmap(frameWidthD, frameHeightD, 96.0, 96.0, PixelFormats.Gray8, null);

            this.controller = controller;
            this.controller.OnFrameReceived += UpdateWindow;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cameraColor.Source = _bitmapColor;
            cameraDepth.Source = _bitmapDepth;
            this.drawEngine = new DrawingEngine(_sensor, canvasDraw);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        public void UpdateWindow(object sender, EventArgs e)
        {
            KinectFrameEventArgs eventArgs = (KinectFrameEventArgs)e;

            Dispatcher.BeginInvoke((Action)(() =>
            {
                if(eventArgs.FramePixels != null)
                {
                    if (eventArgs.FrameType == FrameSourceTypes.Color)
                    {
                        UpdateBitmapColor(eventArgs.FramePixels);

                    }
                    else if (eventArgs.FrameType == FrameSourceTypes.Depth || eventArgs.FrameType == FrameSourceTypes.Infrared)
                    {
                        UpdateBitmapDepth(eventArgs.FramePixels);
                    }
                }

                drawEngine.ClearCanvas();
                drawEngine.DrawBodies(eventArgs.Bodies);
                drawEngine.DrawFaces(eventArgs.Faces);
                drawEngine.DrawInfo(eventArgs.Faces, eventArgs.Bodies);

            }));

        }

        private void UpdateBitmapColor(byte[] colorPixels)
        {
            _bitmapColor.Lock();
            Marshal.Copy(colorPixels, 0, _bitmapColor.BackBuffer, colorPixels.Length);
            _bitmapColor.AddDirtyRect(new Int32Rect(0, 0, _bitmapColor.PixelWidth, _bitmapColor.PixelHeight));
            _bitmapColor.Unlock();
        }

        private void UpdateBitmapDepth(byte[] depthPixels)
        {
            _bitmapDepth.Lock();
            Marshal.Copy(depthPixels, 0, _bitmapDepth.BackBuffer, depthPixels.Length);
            _bitmapDepth.AddDirtyRect(new Int32Rect(0, 0, _bitmapDepth.PixelWidth, _bitmapDepth.PixelHeight));
            _bitmapDepth.Unlock();
        }

        private void frameSource_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;

            if (radioButton == r_button_Color)
            {
                this.controller.ChangeFrameType(FrameSourceTypes.Color);
                cameraColor.Visibility = Visibility.Visible;
                cameraDepth.Visibility = Visibility.Hidden;
            }
            else
            {
                cameraColor.Visibility = Visibility.Hidden;
                cameraDepth.Visibility = Visibility.Visible;
                if (radioButton == r_button_Depth)
                {
                    this.controller.ChangeFrameType(FrameSourceTypes.Depth);
                }
                else if (radioButton == r_button_Infra)
                {
                    this.controller.ChangeFrameType(FrameSourceTypes.Infrared);
                }
            }
        }


    }
}