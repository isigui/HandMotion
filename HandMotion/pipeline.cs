using GalaSoft.MvvmLight;
using HandMotion.filters;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Point = OpenCvSharp.Point;

namespace HandMotion
{
    public class Pipeline : ViewModelBase
    {
        private Mat _input;
        public Mat InputMat
        {
            get { return _input; }
        }

        private List<PipelineStep> _steps;
        public List<PipelineStep> Steps
        {
            get { return _steps; }
            set { _steps = value; RaisePropertyChanged(() => Steps); }
        }

        public Mat OutputMat { get; set; }
        public DateTime FrameDate { get; set; }
        public Pipeline Pipe(FilterBase filter)
        {
            Steps.Add(PipelineStep.For(filter));
            return this;
        }
        public Pipeline()
        {
            Steps = new List<PipelineStep>();


        }



        public async Task RunAsync(Mat input)
        {
            //_input = input;
            _input = new Mat();
            FrameDate = DateTime.Now;
            Cv2.CopyTo(input, _input);
            OutputMat = new Mat();
            //PipelineResult result = new PipelineResult();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    List<IFilter> transforms = new List<IFilter>();
                    foreach (var step in Steps)
                    {
                        if (transforms.Count == 0)
                            step.Filter.Apply(InputMat);
                        else
                            step.Filter.Apply(transforms.Last().Output);
                        transforms.Add(step.Filter);
                    }

                    var resizeFilter = transforms.FirstOrDefault(x => x.FilterName == "resize");
                    if (resizeFilter != null)
                        Cv2.CopyTo(resizeFilter.Output, InputMat);


                    Cv2.CopyTo(transforms.Last().Output, OutputMat);

                //result.ResultSteps.Add(new PipelineResultStep(contourMat));
                //result.ResultSteps.AddRange(transforms.Select(f => new PipelineResultStep(f)));

            });
            }
            catch(AggregateException ae)
            {
                throw ae;
            }
            catch(Exception e)
            {
                throw e;
            }
            //return result;
        }


    }

    public class PipelineStep : ViewModelBase
    {

        public bool IsActive
        {
            get { return Filter.IsActive; }
            set { Filter.IsActive = value; RaisePropertyChanged(() => IsActive); }
        }
        public FilterBase Filter { get; set; }
        public PipelineResultStep Result { get; set; }
        public PipelineStep(FilterBase filter)
        {
            Filter = filter;
            Result = new PipelineResultStep(filter);
        }
        public static PipelineStep For(FilterBase filter)
        {
            PipelineStep ps = new PipelineStep(filter);
            return ps;
        }

    }
}
