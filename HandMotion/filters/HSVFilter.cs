using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class HSVFilter : FilterBase
    {
        
        public HSVFilter(ActivableFilter config) : base("HSV") {
            IsActive = config.IsActive;
        }
        public HSVFilter() : base("HSV")
        {
            IsActive = true;
        }


        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            if (IsActive)
            {
                OpenCvSharp.Cv2.CvtColor(InputArray.Create(input), OutputArray.Create(Output), ColorConversionCodes.BGR2HSV);
            }
            else
                OpenCvSharp.Cv2.CopyTo(input, Output);
            base.Apply(input);
        }

    }
}
