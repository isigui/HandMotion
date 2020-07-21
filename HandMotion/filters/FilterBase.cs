using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;

namespace HandMotion.filters
{
    public abstract class FilterBase : IFilter
    {

        private Mat _output;

        public Mat Output => _output;

        public Mat Input
        {
            get;
            protected set;
        }

        
        public string FilterName {
            get;
            protected set ; 
        }

        public int ComputeTime
        {
            get;
            protected set;
        }
        public bool IsActive { get; set; }

        protected DateTime _start;
        public FilterBase()
        {
            _output = new Mat();
        }
        public FilterBase(string filterName) : this()
        {
            FilterName = filterName;
        }
        public virtual void Apply(Mat input)
        {
            ComputeTime = (int)(DateTime.Now - _start).TotalMilliseconds;
        }

        

    }
}
