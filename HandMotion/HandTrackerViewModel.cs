using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HandMotion
{
    public class HandTrackerViewModel : ViewModelBase
    {
        private readonly TrackerConfig _config;
        public HandTracker Tracker { get; set; }
        private bool _showOutput;
        public bool ShowOutput
        {
            get { return _showOutput; }
            set { _showOutput = value; RaisePropertyChanged(() => ShowOutput); }
        }
        public int ElapsedMilliseconds
        {
            get { return Tracker.ElapsedMillisecondsOfLastFrames; }
        }
        public int FrameCount
        {
            get { return Tracker.FrameCount; }
        }
        public int BufferDuration
        {
            get { return _config.BufferDuration; }
            set { _config.BufferDuration = value; RaisePropertyChanged(() => BufferDuration); }
        }
        public int FPS
        {
            get { return Tracker.FPS; }
        }
        public int SamplesInBuffer
        {
            get { return Tracker.SamplesInBuffer; }
        }
        public HandTrackerViewModel(TrackerConfig config)
        {
            _config = config;
            Tracker = new HandTracker(config);
        }
        public async Task RunAsync()
        {
            while (true)
            {
                RaisePropertyChanged(() => ElapsedMilliseconds);
                RaisePropertyChanged(() => FrameCount);
                RaisePropertyChanged(() => FPS);
                RaisePropertyChanged(() => SamplesInBuffer);
                await Task.Delay(500);
            }
        }
    }
}
