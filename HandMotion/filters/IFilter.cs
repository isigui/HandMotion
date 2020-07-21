using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;

namespace HandMotion.filters
{
    public interface IFilter
    {
        void Apply(Mat input);
        Mat Output { get;}
        Mat Input { get; }
        string FilterName { get; }
        bool IsActive { get; set; }
    }
    
}
