using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class MorphOpenFilter : FilterBase
    {
        private MorphConfig _morphConfig;
        public MorphOpenFilter(MorphConfig config) : base("morphOpen")
        {
            _morphConfig = config;
            IsActive = config.IsActive;
        }
        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            if (IsActive)
            {
                Mat kernel = Mat.Ones(_morphConfig.KernelSize, _morphConfig.KernelSize);
                MorphTypes morphType = MorphTypes.Open;
                Enum.TryParse(_morphConfig.MorphType, out morphType);
                OpenCvSharp.Cv2.MorphologyEx(Input, Output, morphType, kernel);
            }
            else
            {
                OpenCvSharp.Cv2.CopyTo(input, Output);
            }
            base.Apply(input);
        }
    }
}
