using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    class HSVThresoldFilter : FilterBase
    {
        private HSVThresholdConfig _binaryzationConfig;


        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            if (IsActive)
            {
                OpenCvSharp.Cv2.InRange(input, new OpenCvSharp.Scalar(_binaryzationConfig.H.Min, _binaryzationConfig.S.Min, _binaryzationConfig.V.Min), new OpenCvSharp.Scalar(_binaryzationConfig.H.Max, _binaryzationConfig.S.Max, _binaryzationConfig.V.Max), Output);
            }
            else
            {
                OpenCvSharp.Cv2.CopyTo(input, Output);
            }
            base.Apply(input);
        }

        public HSVThresoldFilter(HSVThresholdConfig binaryzationConfig) : base("HSV Threshold")
        {
            _binaryzationConfig = binaryzationConfig;
            IsActive = binaryzationConfig.IsActive;
        }
    }
}
