using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace HandMotion.filters
{
    public class RemoveFaceFilter : FilterBase
    {
        CascadeClassifier _haarCascade;
        FaceConfig _faceConfig;
        public override void Apply(Mat input)
        {
            _start = DateTime.Now;
            Input = input;

            Cv2.CopyTo(input, Output);
            if (IsActive)
            {
                Mat gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
                Rect[] faces = _haarCascade.DetectMultiScale(gray, _faceConfig.Scale, _faceConfig.MinNeighBours, HaarDetectionType.ScaleImage, new Size(_faceConfig.MinSize, _faceConfig.MinSize));

                var mat_currentFrame = new Mat<Vec3b>(Output);
                var indexer_currentFrame = mat_currentFrame.GetIndexer();
                foreach (var face in faces)
                {
                    Cv2.Rectangle(Output, face, new Scalar(0, 0, 0), -1);
                }
            }

            base.Apply(input);
        }
        public RemoveFaceFilter(FaceConfig faceConfig) : base("remove face")
        {
            IsActive = faceConfig.IsActive;
            _haarCascade = new CascadeClassifier(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "haarcascade_frontalface_alt2.xml"));
            _faceConfig = faceConfig;
        }
    }
}
