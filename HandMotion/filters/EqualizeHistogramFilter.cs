using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class EqualizeHistogramFilter : FilterBase
    {
        public EqualizeHistogramFilter(EqualizeHistogramConfig config) : base("equalize histogram")
        {
            IsActive = config.IsActive;
        }
        public override void Apply(Mat input) 
        {
            _start = DateTime.Now;
            Input = input;
            if (IsActive)
            {
                Mat equ = new Mat();
                Cv2.CvtColor(input, equ, ColorConversionCodes.BGR2YCrCb);

                Mat[] Channels;
                Cv2.Split(equ, out Channels);
                Cv2.EqualizeHist(Channels[0], Channels[0]);
                Cv2.Merge(Channels, equ);
                Cv2.CvtColor(equ, Output, ColorConversionCodes.YCrCb2BGR);
            }
            else
                Cv2.CopyTo(Input, Output);

            base.Apply(input);


        }
    }
}
