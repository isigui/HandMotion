using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class ResizeFilter : FilterBase
    {
        private ResizeConfig _resizeConfig;
        public ResizeFilter(ResizeConfig config) : base("resize")
        {

            _resizeConfig = config;

        }
        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            Cv2.Resize(input, Output, new Size(input.Width / _resizeConfig.ScaleDown, input.Height / _resizeConfig.ScaleDown));
            base.Apply(input);
        }
    }
}
