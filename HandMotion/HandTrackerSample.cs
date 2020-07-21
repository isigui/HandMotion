using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion
{
    public class HandTrackerSample
    {
        public DateTime FrameDate { get; set; }
        public Mat InputFrame { get; set; }
        public Point[] HandArea { get; set; }
        public static HandTrackerSample From(DateTime frameDate, Mat inputFrame, Point[] handArea)
        {
            return new HandTrackerSample
            {
                FrameDate = frameDate,
                InputFrame = inputFrame,
                HandArea = handArea
            };
        }
    }
}
