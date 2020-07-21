using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion
{
    public class ColorSkinCalibrationSample
    {
        public Mat Sample { get; set; }
        public ColorSkinCalibrationSampleHSV HSV { get; set; }
        public ColorSkinCalibrationSampleYCRCB YCRCB { get; set; }
        
        public static ColorSkinCalibrationSample From(Mat sample, Mat sampleHsv, Mat sampleYCrCb)
        {
            return new ColorSkinCalibrationSample
            {
                Sample = sample,
                HSV = ColorSkinCalibrationSampleHSV.From(sampleHsv),
                YCRCB = ColorSkinCalibrationSampleYCRCB.From(sampleYCrCb)

            };
        }
    }
    public class ColorSkinCalibrationSampleHSV
    {
        public Mat Histogram_H { get; set; }
        public Mat Histogram_S { get; set; }
        public Mat Histogram_V { get; set; }
        public Mat Sample { get; set; }
        private Point[] _raw_H;
        private Point[] _raw_S;
        private Point[] _raw_V;
        Point[] RawArray_H { get { return _raw_H; } }
        Point[] RawArray_S { get { return _raw_S; } }
        Point[] RawArray_V { get { return _raw_V; } }
        public Point MeanH
        {
            get
            {

                var mean = Helpers.Helper.GetMaxPointFromHistogram(Histogram_H, 10, 180, out _raw_H);

                return mean;
            }
        }
        public Point MeanS
        {
            get
            {
                var mean = Helpers.Helper.GetMaxPointFromHistogram(Histogram_S, 10, 255, out _raw_S);
                return mean;
            }
        }
        public Point MeanV
        {
            get
            {
                var mean = Helpers.Helper.GetMaxPointFromHistogram(Histogram_V, 20, 255, out _raw_V);
                return mean;
            }
        }
        public static ColorSkinCalibrationSampleHSV From(Mat sample)
        {
            return new ColorSkinCalibrationSampleHSV
            {
                Sample = sample,
                Histogram_H = Helpers.Helper.GetHistogram(sample, 0),
                Histogram_S = Helpers.Helper.GetHistogram(sample, 1),
                Histogram_V = Helpers.Helper.GetHistogram(sample, 2)
            };
        }
    }
    public class ColorSkinCalibrationSampleYCRCB
    {
        public Mat Histogram_Y { get; set; }
        public Mat Histogram_Cr { get; set; }
        public Mat Histogram_Cb { get; set; }
        public Mat Sample { get; set; }
        private Point[] _raw_Y;
        private Point[] _raw_Cr;
        private Point[] _raw_Cb;
        Point[] RawArray_Y { get { return _raw_Y; } }
        Point[] RawArray_Cr { get { return _raw_Cr; } }
        Point[] RawArray_Cb { get { return _raw_Cb; } }
        public Point MeanY
        {
            get
            {

                var mean = Helpers.Helper.GetMaxPointFromHistogram(Histogram_Y, 10, 180, out _raw_Y);

                return mean;
            }
        }
        public Point MeanCr
        {
            get
            {
                var mean = Helpers.Helper.GetMaxPointFromHistogram(Histogram_Cr, 10, 255, out _raw_Cr);
                return mean;
            }
        }
        public Point MeanCb        {
            get
            {
                var mean = Helpers.Helper.GetMaxPointFromHistogram(Histogram_Cb, 20, 255, out _raw_Cb);
                return mean;
            }
        }
        public static ColorSkinCalibrationSampleYCRCB From(Mat sample)
        {
            return new ColorSkinCalibrationSampleYCRCB
            {
                Sample = sample,
                Histogram_Y = Helpers.Helper.GetHistogram(sample, 0),
                Histogram_Cr = Helpers.Helper.GetHistogram(sample, 1),
                Histogram_Cb = Helpers.Helper.GetHistogram(sample, 2)
            };
        }
    }
}
