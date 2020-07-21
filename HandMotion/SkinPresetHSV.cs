using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion
{
    public class SkinPresetHSV
    {
        public HSV HSV { get; set; }
        public YCrCb YCRCB { get; set; }
    }
    public class HSV
    {
        public int H { get; set; }
        public int S { get; set; }
        public int V { get; set; }
    }

    public class YCrCb
    {
        public int Y { get; set; }
        public int Cr { get; set; }
        public int Cb { get; set; }
    }
}
