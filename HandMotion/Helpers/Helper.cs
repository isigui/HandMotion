using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandMotion.Helpers
{
    public static class Helper
    {
        public static Mat GetHistogram(Mat source, int channel)
        {
            var histo = new Mat();
            //int width = source.Width;
            //int height = source.Height;
            const int histogramSize = 256;
            int[] dimensions = { histogramSize };
            Rangef[] ranges = { new Rangef(10, histogramSize) };

            Cv2.CalcHist(
                images: new[] { source },
                channels: new[] { channel }, //The channel (dim) to be measured. In this case it is just the intensity (each array is single-channel) so we just write 0.
                mask: null,
                hist: histo,
                dims: 1, //The histogram dimensionality.
                histSize: dimensions,
                ranges: ranges

                );

            return histo;
        }
        public static Point GetMaxPointFromHistogram(Mat histo,int startRange, int endRange, out Point[] histoArray)
        {
            histoArray = new Point[endRange - startRange + 1];
            float maxBin = 0;
            int maxX = 0;
            for (int i = startRange; i < endRange; i++)
            {
                
                var binX = histo.Get<float>(i);
                histoArray[i - startRange] = new Point(i,binX);
                if (binX > maxBin)
                {
                    maxBin = binX;
                    maxX = i;
                }
                //histovals[i-10] = histo.Get<float>(i);
            }
            return new Point(maxX, maxBin);
        }
    }
}
