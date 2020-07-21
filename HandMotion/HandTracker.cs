using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion
{
    public class HandTracker
    {
        private Queue<HandTrackerSample> _samples { get; set; }
        private Queue<DateTime> _allFrames { get; set; }

        private DateTime _lastFrameDate;
        public TrackerConfig TrackerConfig { get; set; }
        public int MinHandAreaSize { get; set; }
        public int FrameCount { get; set; }
        public int SamplesInBuffer
        {
            get
            {
                return _samples.Count;
            }
        }
        public int ElapsedMillisecondsOfLastFrames { get; private set; }
        public Mat TrackerVisualisation { get; set; }

        public int FPS
        {
            get
            {
                return _allFrames.Count / (TrackerConfig.BufferDuration / 1000);
            }
        }


        public Point[] HandArea { get; set; }
        public DateTime FrameDate { get; set; }

        public HandTracker(TrackerConfig trackerConfig)
        {
            if (trackerConfig == null || trackerConfig.BufferDuration <= 0)
                throw new Exception("storage duration must be greater than 0");
            TrackerConfig = trackerConfig;

            _samples = new Queue<HandTrackerSample>();
            _allFrames = new Queue<DateTime>();
        }
        public void DetectHandFrom(Pipeline pipeline)
        {
            FrameCount++;
            ElapsedMillisecondsOfLastFrames = (int)(pipeline.FrameDate - _lastFrameDate).TotalMilliseconds;
            CheckAllFramesLifeTime(pipeline.FrameDate);
            CheckTrackQueueLifeTime();
            _allFrames.Enqueue(pipeline.FrameDate);
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndices;
            Cv2.FindContours(pipeline.OutputMat, out contours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            OpenCvSharp.Point[] largestArea = null;
            double largestAreaSize = 0;

            foreach (var contour in contours)
            {
                var area = Cv2.ContourArea(contour, false);
                if (area > TrackerConfig.MinHandAreaSize)
                {
                    if (largestArea == null)
                    {
                        largestArea = contour;
                        largestAreaSize = area;
                    }
                    else
                    {
                        if (area > largestAreaSize)
                        {
                            largestAreaSize = area;
                            largestArea = contour;
                        }
                    }
                }

            }

            HandArea = largestArea;
            if (HandArea != null)
            {
                TrackerVisualisation = new Mat();
                pipeline.InputMat.CopyTo(TrackerVisualisation);
                Cv2.DrawContours(TrackerVisualisation, new OpenCvSharp.Point[][] { HandArea }, -1, new Scalar(0, 255, 0), 3);

                var newSample = HandTrackerSample.From(DateTime.Now, pipeline.InputMat, HandArea);

                _samples.Enqueue(newSample);
                DetectHulls();
            }
            else
            {
                TrackerVisualisation = null;
            }

            _lastFrameDate = pipeline.FrameDate;

        }
        private void DetectHulls()
        {

            Point[] hulls_points = Cv2.ConvexHull(HandArea, true);
            Rect bounding_rectangle = Cv2.BoundingRect(hulls_points);
            Point centerHand = new Point(
                (bounding_rectangle.TopLeft.X + bounding_rectangle.BottomRight.X) / 2
                , (bounding_rectangle.TopLeft.X + bounding_rectangle.BottomRight.X) / 2
                );

            Cv2.Rectangle(TrackerVisualisation, bounding_rectangle, new Scalar(0, 255, 255), 2);
            Cv2.Circle(TrackerVisualisation, centerHand, 5, new Scalar(255, 0, 0));


        }
        private void CheckTrackQueueLifeTime()
        {
            HandTrackerSample firstSampleInBuffer;
            if (_samples.TryPeek(out firstSampleInBuffer))
            {
                if ((DateTime.Now - firstSampleInBuffer.FrameDate).TotalMilliseconds > TrackerConfig.BufferDuration)
                {
                    _samples.Dequeue();
                    CheckTrackQueueLifeTime();
                }
            }
        }
        private void CheckAllFramesLifeTime(DateTime frameDate)
        {
            DateTime firstFrameInBuffer;
            if (_allFrames.TryPeek(out firstFrameInBuffer))
            {
                if ((frameDate - firstFrameInBuffer).TotalMilliseconds > TrackerConfig.BufferDuration)
                {
                    _allFrames.Dequeue();
                    CheckAllFramesLifeTime(frameDate);
                }
            }
        }

        private void StoreFrame()
        {

        }


    }
}
