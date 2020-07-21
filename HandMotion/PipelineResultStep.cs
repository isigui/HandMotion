using GalaSoft.MvvmLight;
using HandMotion.filters;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;

namespace HandMotion
{
    public class PipelineResult : ViewModelBase
    {
        public List<PipelineResultStep> ResultSteps { get; set; }

        public PipelineResult()
        {
            ResultSteps = new List<PipelineResultStep>();
        }

    }
    public class PipelineResultStep : ViewModelBase
    {
        public Bitmap OutputBitmap { get; set; }

        public ImageSource OutputImageSource { get; set; }
        public string Format { get; set; }

        public IFilter Filter { get; set; }


        private Mat _mat;
        public PipelineResultStep(IFilter filter)
        {
            Filter = filter;
        }
        public PipelineResultStep(Mat mat)
        {
            _mat = mat;
 
            Format = $"dim: {mat.Width}x{mat.Height} - channel: {mat.Channels()}";
        }
        public void Compute()
        {
            if (Filter == null)
            {
                OutputBitmap = _mat.ToBitmap();
                OutputImageSource = OutputBitmap.ToBitmapImage();
            }
            else
            {
                OutputBitmap = Filter.Output.ToBitmap();
                OutputImageSource = OutputBitmap.ToBitmapImage();
                Format = $"dim: {Filter.Output.Width}x{Filter.Output.Height} - channel: {Filter.Output.Channels()}";
                
            }
            RaisePropertyChanged(() => Format);
            RaisePropertyChanged(() => OutputImageSource);
            RaisePropertyChanged(() => Filter);
        }

    }
}
