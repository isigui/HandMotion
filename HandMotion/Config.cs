using GalaSoft.MvvmLight;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMotion
{
    public class Config : ViewModelBase
    {
        //public MinMax BinaryThreshold { get; set; }
        //public ActivableFilter HsvConfig { get; set; }
        //public HSVThresholdConfig HSVThreshold { get; set; }
        //public YCrCbThresholdConfig YCrCbThresholdConfig { get; set; }
        public BlurConfig BlurConfig { get; set; }
        public FaceConfig FaceConfig { get; set; }
        public MorphConfig MorphConfig { get; set; }
        public EqualizeHistogramConfig EqHistoConfig { get; set; }
        public BackGroundRemoveConfig BackGroundRemoveConfig { get; set; }
        public ResizeConfig ScaleDownConfig { get; set; }
        public ColorSkinCalibrationConfig ColorSkinCalibrationConfig { get; set; }
        public TrackerConfig TrackerConfig { get; set; }

        public SkinDetectConfig SkinDetectConfig { get; set; }
        public static Config Default()
        {
            Config config = new Config();
            //config.BinaryThreshold = MinMax.Default();
            //config.HSVThreshold = HSVThresholdConfig.Default();
            config.BlurConfig = BlurConfig.Default();
            config.FaceConfig = FaceConfig.Default();
            config.MorphConfig = MorphConfig.Default();
            config.BackGroundRemoveConfig = BackGroundRemoveConfig.Default();
            config.ScaleDownConfig = ResizeConfig.Default();
            config.TrackerConfig = TrackerConfig.Default();
            config.ColorSkinCalibrationConfig = ColorSkinCalibrationConfig.Default();
            config.BackGroundRemoveConfig = BackGroundRemoveConfig.Default();
            //config.YCrCbThresholdConfig = YCrCbThresholdConfig.Default();
            config.SkinDetectConfig = SkinDetectConfig.Default();
            return config;
        }
    }
    public class SkinDetectConfig
    {
        public HSVThresholdConfig HSVThresholdConfig { get; set; }
        public YCrCbThresholdConfig YCrCbThresholdConfig { get; set; }
        public static SkinDetectConfig Default()
        {
            return new SkinDetectConfig
            {
                HSVThresholdConfig = HSVThresholdConfig.Default(),
                YCrCbThresholdConfig = YCrCbThresholdConfig.Default()
            };
        }
    }
    public class HSVThresholdConfig : ActivableFilter
    {
        public MinMax H { get; set; }
        public MinMax S { get; set; }
        public MinMax V { get; set; }
        public static HSVThresholdConfig Default()
        {
            return new HSVThresholdConfig
            {
                H = MinMax.Default(),
                S = MinMax.Default(),
                V = MinMax.Default()
            };
        }
    }
    public class YCrCbThresholdConfig : ActivableFilter
    {
        public MinMax Y { get; set; }
        public MinMax Cr { get; set; }
        public MinMax Cb { get; set; }
        public static YCrCbThresholdConfig Default()
        {
            return new YCrCbThresholdConfig
            {
                Y = MinMax.Default(),
                Cr = MinMax.Default(),
                Cb = MinMax.Default()
            };
        }
    }
    public class MinMax : ActivableFilter
    {
        private int _min;
        private int _max;
        public int Min
        {
            get { return _min; }
            set { _min = value; RaisePropertyChanged(() => Min); }
        }
        public int Max
        {
            get { return _max; }
            set { _max = value; RaisePropertyChanged(() => Max); }
        }
        public static MinMax Default()
        {
            return new MinMax
            {
                Min = 0,
                Max = 255
            };
        }
    }

    public class BlurConfig : ActivableFilter
    {
        public Size KernelSize
        {
            get
            {
                return new Size
                {
                    Height = _size % 2 == 0 ? _size + 1 : _size,
                    Width = _size % 2 == 0 ? _size + 1 : _size
                };
            }
        }
        private double _sigma;
        private int _size;
        public double Sigma
        {
            get { return _sigma; }
            set { _sigma = value; RaisePropertyChanged(() => Sigma); }
        }
        public int Size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged(() => Size); }
        }
        public static BlurConfig Default()
        {
            return new BlurConfig
            {
                Size = 21,
                Sigma = 3
            };
        }
    }
    public class FaceConfig : ActivableFilter
    {

        private double _scale;
        private int _minNeighbours;
        private int _minSize;
        public double Scale
        {
            get { return _scale; }
            set { _scale = value; RaisePropertyChanged(() => Scale); }
        }
        public int MinNeighBours
        {
            get { return _minNeighbours; }
            set { _minNeighbours = value; RaisePropertyChanged(() => MinNeighBours); }
        }
        public int MinSize
        {
            get { return _minSize; }
            set { _minSize = value; RaisePropertyChanged(() => MinSize); }
        }
        public static FaceConfig Default()
        {

            return new FaceConfig
            {
                Scale = 1.08,
                MinNeighBours = 2,
                MinSize = 30
            };
        }
    }

    public class MorphConfig : ActivableFilter
    {
        private int _kernelSize;
        private string _morphType;
        public int KernelSize
        {
            get { return _kernelSize; }
            set { _kernelSize = (value % 2 == 0 ? value + 1 : value); RaisePropertyChanged(() => KernelSize); }
        }
        public string MorphType
        {
            get { return _morphType; }
            set { _morphType = value; RaisePropertyChanged(() => MorphType); }
        }
        public static MorphConfig Default()
        {
            return new MorphConfig
            {
                KernelSize = 5,
                MorphType = MorphTypes.Open.ToString()
            };
        }

    }

    public class EqualizeHistogramConfig : ActivableFilter { }
    public class BackGroundRemoveConfig : ActivableFilter
    {
        private int _backgroundThreshold;

        public int BackGroundThreshold
        {
            get { return _backgroundThreshold; }
            set { _backgroundThreshold = value; RaisePropertyChanged(() => BackGroundThreshold); }
        }
        public static BackGroundRemoveConfig Default()
        {
            return new BackGroundRemoveConfig
            {
                BackGroundThreshold = 31,
                IsActive = true
            };
        }
    }
    public class ResizeConfig : ViewModelBase
    {
        private int _scaleDown;
        public int ScaleDown
        {
            get { return _scaleDown; }
            set { _scaleDown = value; RaisePropertyChanged(() => ScaleDown); }
        }
        public static ResizeConfig Default()
        {
            return new ResizeConfig
            {
                ScaleDown = 3
            };
        }
    }
    public class ActivableFilter : ViewModelBase
    {
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; RaisePropertyChanged(() => IsActive); }
        }
        public ActivableFilter()
        {
            IsActive = true;
        }

    }
    public class ColorSkinCalibrationConfig : ViewModelBase
    {
        private int _width;
        private int _height;
        private int _tlx;
        private int _tly;
        private int _calibrationDuration;
        public int Width
        {
            get { return _width; }
            set { _width = value; RaisePropertyChanged(() => Width); }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; RaisePropertyChanged(() => Height); }
        }
        public int TLX
        {
            get { return _tlx; }
            set { _tlx = value; RaisePropertyChanged(() => TLX); }
        }
        public int TLY
        {
            get { return _tly; }
            set { _tly = value; RaisePropertyChanged(() => TLY); }
        }
        public int CalibrationDuration
        {
            get { return _calibrationDuration; }
            set { _calibrationDuration = value; RaisePropertyChanged(() => CalibrationDuration); }
        }
        public static ColorSkinCalibrationConfig Default()
        {
            return new ColorSkinCalibrationConfig
            {
                Width = 10,
                Height = 10,
                TLX = 10,
                TLY = 10
            };
        }
    }
    public class TrackerConfig : ViewModelBase
    {
        private int _BufferDuration;
        public int BufferDuration
        {
            get { return _BufferDuration; }
            set { _BufferDuration = value; RaisePropertyChanged(() => BufferDuration); }
        }
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; RaisePropertyChanged(() => IsActive); }
        }
        private int _minHandAreaSize;
        public int MinHandAreaSize
        {
            get { return _minHandAreaSize; }
            set { _minHandAreaSize = value; RaisePropertyChanged(() => MinHandAreaSize); }
        }
        public static TrackerConfig Default()
        {
            return new TrackerConfig
            {
                BufferDuration = 3000
            };
        }
    }
}
