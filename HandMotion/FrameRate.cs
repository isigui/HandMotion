using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion
{
    public class FrameRate : ViewModelBase
    {
        private FrameRate _last;
        public int FrameCount { get; set; }
        public int FPS
        {
            get
            {
                if (ElapsedMilliseconds > 0)
                    return (int)(1000 / ElapsedMilliseconds);
                return 0;
            }
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
   
        public int ElapsedMilliseconds
        {
            get { return (int)(EndDate - StartDate).TotalMilliseconds; }
        }
        public void Tick()
        {
            StartDate = _last != null ? _last.EndDate : DateTime.Now;
            EndDate = DateTime.Now;

            FrameCount++;
            _last = this;
            RaisePropertyChanged(() => ElapsedMilliseconds);
            RaisePropertyChanged(() => FrameCount);
            RaisePropertyChanged(() => FPS);
        }
    }
}
