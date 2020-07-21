using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class YCrCbFilter : FilterBase
    {
        
        public YCrCbFilter(ActivableFilter config) : base("YCrCb") {
            IsActive = config.IsActive;
        }
        public YCrCbFilter() : base("YCrCb")
        {
            IsActive = true;
        }


        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            if (IsActive)
            {
                OpenCvSharp.Cv2.CvtColor(InputArray.Create(input), OutputArray.Create(Output), ColorConversionCodes.BGR2YCrCb);
            }
            else
                OpenCvSharp.Cv2.CopyTo(input, Output);
            base.Apply(input);
        }

    }
}
