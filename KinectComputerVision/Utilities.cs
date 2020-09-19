using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;

using System.Runtime.Remoting.Messaging;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace KinectComputerVision
{
    public static class Utilities
    {

        public static Mat BitmapToMat(Bitmap bmp, bool gray = false)
        {
            Mat result;

            if (gray)
            {
                Image<Gray, byte> res = bmp.ToImage<Gray, byte>();
                result = res.Mat;
            }
            else
            {
                Image<Rgb, byte> res = bmp.ToImage<Rgb, byte>();
                result = res.Mat;
            }

            return result;
        }

        public static List<Mat> BitmapsToMat(IList<Bitmap> list, bool gray = false)
        {
            int count = list.Count();
            List<Mat> res = new List<Mat>(count);
            for(int i = 0; i < count; i++)
            {
                Mat m = BitmapToMat(list[i], gray);
                res[i] = m;
            }
            return res;
        }

        public static Bitmap CropFaceBitmap(IReadOnlyList<CameraSpacePoint> points, CoordinateMapper mapper, byte[] pixels, bool grayScale)
        {
            int minX = (int)points.Min(x => mapper.MapCameraPointToColorSpace(x).X);
            int maxX = (int)points.Max(x => mapper.MapCameraPointToColorSpace(x).X);
            int minY = (int)points.Min(x => mapper.MapCameraPointToColorSpace(x).Y);
            int maxY = (int)points.Max(x => mapper.MapCameraPointToColorSpace(x).Y);
            int width = maxX - minX;
            var height = maxY - minY;

            PixelFormat targetFormat = grayScale ? PixelFormat.Format8bppIndexed : PixelFormat.Format32bppRgb;
            Bitmap target = new Bitmap(width, height, targetFormat);
            BitmapData bmapdata = target.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, targetFormat);

            lock (pixels)
            {

                IntPtr ptr = bmapdata.Scan0;
                int rowLengthBytes = 7680;

                if (targetFormat == PixelFormat.Format32bppRgb)
                {
                    int buffColStart = minX * 4;
                    int cropWidthBytes = width * 4;

                    for (int r = minY; r < minY + height; r++)
                    {
                        Marshal.Copy(pixels, buffColStart + (rowLengthBytes * r), ptr, cropWidthBytes);
                        ptr = IntPtr.Add(ptr, bmapdata.Stride);
                    }
                }
                else
                {
                    for (int row = minY; row < minY + height; row++)
                    {
                        for(int col  = minX; col < minX + width; col++)
                        {
                            int cur_pixel_ind = (col * 4) + (rowLengthBytes * row);
                            int b = pixels[cur_pixel_ind];
                            int g = pixels[cur_pixel_ind + 1];
                            int r = pixels[cur_pixel_ind + 2];
                            int a = pixels[cur_pixel_ind + 3];

                            int gray = (int)(r * 0.2126 + g * 0.7152 + b * 0.0722);

                            Marshal.WriteInt32(ptr, gray);

                            ptr = IntPtr.Add(ptr, 1);
                        }                    
                        
                    }
                }
            }

            target.UnlockBits(bmapdata);
            return target;

        }

    }
}
