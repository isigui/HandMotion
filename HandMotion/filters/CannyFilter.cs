using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    class CannyFilter : FilterBase
    {
        private int _minThreshold;
        public CannyFilter(int minThreshold) : base("canny")
        {
            _minThreshold = minThreshold;
        }
        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
 
            OpenCvSharp.Cv2.Canny(InputArray.Create(Input), Output, _minThreshold, _minThreshold * 2);
            base.Apply(input);
        }
    }
}
