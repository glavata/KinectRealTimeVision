
using KinectDataEngine;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectView
{
    public class DrawingEngine
    {

        private enum FaceBorderPoints
        {
            ForeheadCenter = 28,
            Leftcheekbone = 458,
            Rightcheekbone = 674,
            ChinCenter = 4
        }

        private readonly Color[] colorPoints = new Color[]{ Colors.YellowGreen, Colors.DarkCyan, Colors.HotPink,
                                                  Colors.Khaki, Colors.Orchid, Colors.Sienna};

        private readonly Color[] colorBones = new Color[]{ Colors.Purple, Colors.Gold, Colors.Lavender,
                                                 Colors.OrangeRed, Colors.Brown, Colors.GreenYellow};

        private readonly Color[] faceColors = new Color[] { Colors.Red, Colors.Orange, Colors.Green,
                                                  Colors.LightBlue, Colors.Indigo, Colors.Violet};

        private readonly int[,] boneIndices = { {3, 2},   {2, 20},   {20, 4},    {20, 8},
                                                {20, 1},  {4, 5},    {8, 9 },    {5, 6 },
                                                {9, 10},  {6, 7},    {10, 11},   {11, 23},
                                                {7, 21},  {21, 22},  {23, 24 },  {1, 0 },
                                                {0, 12 }, {0, 16 },  {12, 13 },  {16, 17 },
                                                {13, 14}, {17, 18 }, {14, 15 },  {18, 19 }
        };

        private Canvas canvas;
        private KinectSensor sensor;
        private float ratioW;
        private float ratioH;

        public DrawingEngine(KinectSensor sensor, Canvas canvas)
        {
            this.canvas = canvas;
            this.sensor = sensor;

            UpdateCanvasRatio();

            this.canvas.SizeChanged += OnCanvasSizeChanged;
        }

        public void OnCanvasSizeChanged(object sender, EventArgs e)
        {
            UpdateCanvasRatio();
        }

        private void UpdateCanvasRatio()
        {
            this.ratioW = sensor.ColorFrameSource.FrameDescription.Width / (float)canvas.ActualWidth;
            this.ratioH = sensor.ColorFrameSource.FrameDescription.Height / (float)canvas.ActualHeight;
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
        }

        public void DrawInfo(IList<Face> faces, IList<Body> bodies)
        {
            int order = 0;
            foreach (var face in faces)
            {
                if (face.IsTracked)
                {
                    DrawFaceInfo(face, order);
                    order++;
                }
            }

        }

        public void DrawBodies(IList<Body> bodies)
        {
            int colorIndex = 0;
            
            foreach (var body in bodies)
            {
                if (body != null && body.IsTracked)
                {
                    DrawSkeleton(body, colorIndex++);
                }
            }
        }

        public void DrawFaces(IList<Face> faces)
        {
            int colorIndex = 0;

            foreach (var face in faces)
            {
                if (face.IsTracked)
                {
                    DrawFaceBoundingBox(face.Vertices, faceColors[colorIndex++]);
                    DrawFaceCoordinateSystem(face.CoordSys);
                }

            }
        }

        private void DrawFaceInfo(Face face, int order)
        {
            int top = (order * 30 + 10) / (int)ratioH;

            TextBlock textBlockFaceInfo = new TextBlock();
            textBlockFaceInfo.Background = new SolidColorBrush(Colors.White);
            textBlockFaceInfo.Foreground = new SolidColorBrush(faceColors[order]);
            textBlockFaceInfo.Width = 250 / ratioW;

            textBlockFaceInfo.Text = String.Format(" {0:0} {1:0} {2:0}",
                                    face.FaceAngleEuler.Y,
                                    face.FaceAngleEuler.P,
                                    face.FaceAngleEuler.R);

            Canvas.SetLeft(textBlockFaceInfo, canvas.ActualWidth - textBlockFaceInfo.Width - 50);
            Canvas.SetTop(textBlockFaceInfo, top / ratioH);

            canvas.Children.Add(textBlockFaceInfo);

        }

        private void DrawFaceBoundingBox(IReadOnlyList<CameraSpacePoint> vertices, Color color)
        {

            if (vertices.Count > 0)
            {
                CameraSpacePoint verticeTop = vertices[(int)FaceBorderPoints.ForeheadCenter];
                ColorSpacePoint pointTop = sensor.CoordinateMapper.MapCameraPointToColorSpace(verticeTop);

                CameraSpacePoint verticeLeft = vertices[(int)FaceBorderPoints.Leftcheekbone];
                ColorSpacePoint pointLeft = sensor.CoordinateMapper.MapCameraPointToColorSpace(verticeLeft);

                CameraSpacePoint verticeRight = vertices[(int)FaceBorderPoints.Rightcheekbone];
                ColorSpacePoint pointRight = sensor.CoordinateMapper.MapCameraPointToColorSpace(verticeRight);

                CameraSpacePoint verticeBottom = vertices[(int)FaceBorderPoints.ChinCenter];
                ColorSpacePoint pointBottom = sensor.CoordinateMapper.MapCameraPointToColorSpace(verticeBottom);
                

                if (float.IsInfinity(pointTop.X) || float.IsInfinity(pointTop.Y)) return;
                if (float.IsInfinity(pointLeft.X) || float.IsInfinity(pointLeft.Y)) return;
                if (float.IsInfinity(pointRight.X) || float.IsInfinity(pointRight.Y)) return;
                if (float.IsInfinity(pointBottom.X) || float.IsInfinity(pointBottom.Y)) return;

                float posX = pointLeft.X;
                float posY = pointTop.Y;
                double width = Math.Abs(pointRight.X - pointLeft.X);
                double height = Math.Abs(pointTop.Y - pointBottom.Y);

                DrawRectangle(posX, posY, color, 5, width, height);
            }
        }

        private void DrawFaceCoordinateSystem(CoordSys coordSys)
        {
            ColorSpacePoint pointNoseTip = sensor.CoordinateMapper.MapCameraPointToColorSpace(coordSys.Front);
            ColorSpacePoint pointforeheadCenter = sensor.CoordinateMapper.MapCameraPointToColorSpace(coordSys.Top);
            ColorSpacePoint pointleftCheek = sensor.CoordinateMapper.MapCameraPointToColorSpace(coordSys.Side);
            ColorSpacePoint pointavgPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(coordSys.Center);

            DrawLine(pointforeheadCenter.X, pointforeheadCenter.Y, pointavgPoint.X, pointavgPoint.Y, 3, Colors.Blue);
            DrawLine(pointNoseTip.X, pointNoseTip.Y, pointavgPoint.X, pointavgPoint.Y, 3, Colors.Red);
            DrawLine(pointleftCheek.X, pointleftCheek.Y, pointavgPoint.X, pointavgPoint.Y, 3, Colors.Green);
        }

        private void DrawRectangle(double X, double Y, Color color, double strokeThickness, double width, double height)
        {
            Rectangle rect = new Rectangle();
            rect.Width = width / ratioW;
            rect.Height = height / ratioH;
            rect.Stroke = new SolidColorBrush(color);
            rect.StrokeThickness = strokeThickness;
            rect.StrokeLineJoin = PenLineJoin.Round;
            rect.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            Canvas.SetLeft(rect, X / ratioW);
            Canvas.SetTop(rect, Y / ratioH);

            canvas.Children.Add(rect);
        }

        private void DrawSkeleton(Body body, int colorIndex)
        {
            foreach (Joint joint in body.Joints.Values)
            {
                DrawJoint(joint, colorPoints[colorIndex]);
            }

            for (int i = 0; i < boneIndices.GetLength(0); i++)
            {
                DrawBone(body.Joints[(JointType)boneIndices[i, 0]],
                         body.Joints[(JointType)boneIndices[i, 1]], colorBones[colorIndex]);
            }
        }

        private ColorSpacePoint ToColorSpacePoint(Joint joint)
        {

            CameraSpacePoint jointPosition = joint.Position;
            ColorSpacePoint jointPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
            
            return jointPoint;
        }

        private void DrawJoint(Joint joint, Color color)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;

            ColorSpacePoint jointPoint = ToColorSpacePoint(joint);

            if (float.IsInfinity(jointPoint.X) || float.IsInfinity(jointPoint.Y)) return;

            DrawPoint(jointPoint.X, jointPoint.Y, 20, 20, color);
        }

        private void DrawBone(Joint first, Joint second, Color color)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            ColorSpacePoint jointPointFirst = ToColorSpacePoint(first);
            ColorSpacePoint jointPointSecond = ToColorSpacePoint(second);

            if (float.IsInfinity(jointPointFirst.X) || float.IsInfinity(jointPointFirst.Y)) return;
            if (float.IsInfinity(jointPointSecond.X) || float.IsInfinity(jointPointSecond.Y)) return;

            DrawLine(jointPointFirst.X, jointPointFirst.Y, jointPointSecond.X, jointPointSecond.Y, 8, color);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, double thickness, Color color)
        {
            Line line = new Line
            {
                X1 = x1 / ratioW, Y1 = y1 / ratioH, X2 = x2/ratioW, Y2 = y2 / ratioH,
                StrokeThickness = thickness,
                Stroke = new SolidColorBrush(color)
            };

            canvas.Children.Add(line);
        }

        private void DrawPoint(double X, double Y, double width, double height, Color color)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = width / ratioW,
                Height = height / ratioH,
                Fill = new SolidColorBrush(color)
            };

            Canvas.SetLeft(ellipse, (X - ellipse.Width / 2) /  ratioW);
            Canvas.SetTop(ellipse, (Y - ellipse.Height / 2) / ratioH);

            canvas.Children.Add(ellipse);
        }


    }
}
