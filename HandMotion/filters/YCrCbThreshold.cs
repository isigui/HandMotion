using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class YCrCbThreshold : FilterBase
    {
        private YCrCbThresholdConfig _config;
        public YCrCbThreshold(YCrCbThresholdConfig config) : base("YcrcbThreshold")
        {
            _config = config;
            IsActive = config.IsActive;
        }
        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            if (IsActive)
            {
                OpenCvSharp.Cv2.InRange(input, new OpenCvSharp.Scalar(_config.Y.Min, _config.Cr.Min, _config.Cb.Min), new OpenCvSharp.Scalar(_config.Y.Max, _config.Cr.Max, _config.Cb.Max), Output);
            }
            else
            {
                OpenCvSharp.Cv2.CopyTo(input, Output);
            }
            base.Apply(input);
        }
    }
}
