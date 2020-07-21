using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion.filters
{
    public class BackgroundRemoveFilter : FilterBase
    {
        BackGroundRemoveConfig _config;
        Mat _background;
        public BackgroundRemoveFilter(Mat background, BackGroundRemoveConfig config) : base("background remove")
        {
            _background = background;
            _config = config;
            IsActive = config.IsActive;
        }
        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;
            Cv2.CopyTo(Input, Output);
            if (IsActive)
            {
                Mat greyframe = new Mat();
                Cv2.CvtColor(input, greyframe, ColorConversionCodes.BGR2GRAY);
                Input = input;

                var indexer_frame = Output.GetGenericIndexer<Vec3b>();
                var indexer_frame_grey = greyframe.GetGenericIndexer<Vec3b>();
                var indexer_bg = _background.GetGenericIndexer<Vec3b>();

                for (int i = 0; i < Output.Rows; i++)
                {
                    for (int j = 0; j < Output.Cols; j++)
                    {
                        var grey_frame = indexer_frame_grey[i, j];
                        var grey_bg = indexer_bg[i, j];

                        if (Math.Abs(grey_frame.Item0 - grey_bg.Item0) < _config.BackGroundThreshold)
                        {

                            //frame.Item0 = 0;
                            indexer_frame[i, j] = new Vec3b(0, 0, 0);
                        }
                        else
                        {
                            //frame.Item0 = 255;
                            //indexer_frame[i, j] = 255;
                        }
                    }
                }
            }
            else
            {
                OpenCvSharp.Cv2.CopyTo(input, Output);
            }
            base.Apply(input);
        }
    }
}
