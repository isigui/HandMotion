using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class SkinDetectFilter : FilterBase
    {
        private SkinDetectConfig _config;
        public SkinDetectFilter(SkinDetectConfig config) : base("skin detection")
        {
            _config = config;
        }
        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            Mat hsv = new Mat();
            Mat ycrcb = new Mat();
            Mat hsv_mask = new Mat(input.Size(), MatType.CV_8UC1, Scalar.All(0));
            Mat ycrcb_mask = new Mat(input.Size(), MatType.CV_8UC1, Scalar.All(0));
            Mat global_mask = new Mat(input.Size(), MatType.CV_8UC1, Scalar.All(0));

            
            //Cv2.CvtColor(hsv, hsv, ColorConversionCodes.BGR2HSV);
            //Cv2.CvtColor(ycrcb, ycrcb, ColorConversionCodes.BGR2HSV);
            if (_config.HSVThresholdConfig.IsActive)
            {
                Cv2.CopyTo(input, hsv);
                HSVFilter hsvFilter = new HSVFilter();
                HSVThresoldFilter hsvThreshold = new HSVThresoldFilter(_config.HSVThresholdConfig);
                hsvFilter.Apply(hsv);
                hsvThreshold.Apply(hsvFilter.Output);
                Cv2.CopyTo(hsvThreshold.Output, hsv_mask);
            }
            else
            {
                hsv_mask = new Mat(input.Size(), MatType.CV_8UC1, Scalar.All(255));
            }
            if (_config.YCrCbThresholdConfig.IsActive)
            {
                Cv2.CopyTo(input, ycrcb);
                YCrCbFilter ycrcbFilter = new YCrCbFilter();
                YCrCbThreshold ycrcbthreshold = new YCrCbThreshold(_config.YCrCbThresholdConfig);
                ycrcbFilter.Apply(ycrcb);
                ycrcbthreshold.Apply(ycrcbFilter.Output);
                Cv2.CopyTo(ycrcbthreshold.Output, ycrcb_mask);
            }
            else
            {
                ycrcb_mask = new Mat(input.Size(), MatType.CV_8UC1, Scalar.All(255));
            }


            Cv2.BitwiseAnd(hsv_mask, ycrcb_mask, global_mask);


            Cv2.CopyTo(global_mask, Output);
            //Cv2.CopyTo(input, Output, global_mask);


            //if (IsActive)
            //{
            //OpenCvSharp.Cv2.InRange(input, new OpenCvSharp.Scalar(_config.HSVThresholdConfig.H.Min, _config.HSVThresholdConfig.S.Min, _config.HSVThresholdConfig.V.Min), new OpenCvSharp.Scalar(_config.HSVThresholdConfig.H.Max, _config.HSVThresholdConfig.S.Max, _config.HSVThresholdConfig.V.Max), Output);
            //}
            //else
            //{
            //    OpenCvSharp.Cv2.CopyTo(input, Output);
            //}
            base.Apply(input);
        }
    }
}
