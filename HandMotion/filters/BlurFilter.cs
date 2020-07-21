using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class BlurFilter : FilterBase
    {
        private BlurConfig _blurConfig;

        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;

            if (IsActive)
                OpenCvSharp.Cv2.GaussianBlur(input, Output, _blurConfig.KernelSize, _blurConfig.Sigma);
            else
            {
                OpenCvSharp.Cv2.CopyTo(input, Output);
            }
            base.Apply(input);
        }
        public BlurFilter(BlurConfig BlurConfig) : base("Blur")
        {
            _blurConfig = BlurConfig;
            IsActive = BlurConfig.IsActive;
            
        }
    }
}
