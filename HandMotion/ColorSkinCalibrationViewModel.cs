using GalaSoft.MvvmLight;
using HandMotion.filters;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MathNet.Numerics.Statistics;
using System.Linq;
using System.IO;
using System.Reflection;

namespace HandMotion
{
    public class ColorSkinCalibrationViewModel : ViewModelBase
    {
        //private ColorSkinCalibrationConfig _colorSkinCalibrationConfig;
        //private BlurConfig _blurConfig;
        //private BackGroundRemoveConfig _backGroundRemoveConfig;
        //private EqualizeHistogramConfig _equalizeHistogramConfig;
        private Config _config;


        private Mat _initialTarget;
        private Mat _target;
        private Mat _wcropped;
        private Mat _wbackground;
        private Mat _wSubstractBackground;
        private Mat _whsv;
        private Mat _wycrcb;
        public Mat _whisto_h;
        public Mat _whisto_s;
        public Mat _whisto_v;


        private ImageSource _w_hand_target;
        public ImageSource W_HANDTARGET
        {
            get
            {
                return _w_hand_target;
            }
            set
            {
                _w_hand_target = value; RaisePropertyChanged(() => W_HANDTARGET);
            }
        }

        public ImageSource W_HISTO_H
        {
            get
            {
                if (_whisto_h.Width > 0)
                    return _whisto_h.ToBitmap().ToBitmapImage();
                return null;
            }
        }
        public ImageSource W_HISTO_S
        {
            get
            {
                if (_whisto_s.Width > 0)
                    return _whisto_s.ToBitmap().ToBitmapImage();
                return null;
            }
        }
        public ImageSource W_HISTO_V
        {
            get
            {
                if (_whisto_v.Width > 0)
                    return _whisto_v.ToBitmap().ToBitmapImage();
                return null;
            }
        }

        public ImageSource W_CROPPED
        {
            get
            {
                if (_wcropped.Width > 0)
                    return _wcropped.ToBitmap().ToBitmapImage();
                return null;
            }
        }
        public ImageSource W_BACKGROUND_SUBSTRACTED
        {
            get
            {
                if (_wSubstractBackground.Width > 0)
                    return _wSubstractBackground.ToBitmap().ToBitmapImage();
                return null;
            }
        }
        public ImageSource W_HSV
        {
            get
            {
                if (_whsv.Width > 0)
                    return _whsv.ToBitmap().ToBitmapImage();
                return null;
            }
        }
        public ImageSource W_YCRCB
        {
            get
            {
                if (_wycrcb.Width > 0)
                    return _wycrcb.ToBitmap().ToBitmapImage();
                return null;
            }
        }
        public int MaxWidth
        {
            get
            {
                if (_target == null)
                    return 0;
                return _target.Width - _config.ColorSkinCalibrationConfig.TLX;
            }
        }
        public int MaxHeight
        {
            get
            {
                if (_target == null)
                    return 0;
                return _target.Height - _config.ColorSkinCalibrationConfig.TLY;
            }
        }

        public ColorSkinCalibrationConfig ColorSkinCalibrationConfig
        {
            get
            {
                return _config.ColorSkinCalibrationConfig;
            }
            set { _config.ColorSkinCalibrationConfig = value; RaisePropertyChanged(() => ColorSkinCalibrationConfig); }
        }
        public BlurConfig BlurConfig
        {
            get
            {
                return _config.BlurConfig;
            }
            set { _config.BlurConfig = value; RaisePropertyChanged(() => BlurConfig); }
        }
        public BackGroundRemoveConfig BackGroundRemoveConfig
        {
            get
            {
                return _config.BackGroundRemoveConfig;
            }
            set { _config.BackGroundRemoveConfig = value; RaisePropertyChanged(() => BackGroundRemoveConfig); }
        }
        public EqualizeHistogramConfig EqualizeHistogramConfig
        {
            get
            {
                return _config.EqHistoConfig;
            }
            set { _config.EqHistoConfig = value; RaisePropertyChanged(() => EqualizeHistogramConfig); }
        }

        public bool _canStartCalibration;
        public bool CanStartCalibration
        {
            get { return _canStartCalibration; }
            set { _canStartCalibration = value; RaisePropertyChanged(() => CanStartCalibration); }
        }
        public bool CalibrationStarted { get; private set; }
        private DateTime _calibrationStartDate;
        private List<ColorSkinCalibrationSample> _samples;
        private SkinPresetHSV _result;
        public SkinPresetHSV Result
        {
            get { return _result; }
            set { _result = value; RaisePropertyChanged(() => Result); }
        }
        public int? RemainingSeconds
        {
            get
            {
                if (CalibrationStarted)
                    return _config.ColorSkinCalibrationConfig.CalibrationDuration * 1000 - (int)(DateTime.Now - _calibrationStartDate).TotalMilliseconds;
                return null;
            }
        }

        private CascadeClassifier _haarCascade;
        private bool _useHeadDetection;
        public bool UseHeadDetection
        {
            get { return _useHeadDetection; }
            set { _useHeadDetection = value; RaisePropertyChanged(() => UseHeadDetection); }
        }
        public ColorSkinCalibrationViewModel(Config config)
        {

            _config = config;
            _samples = new List<ColorSkinCalibrationSample>();
            _wbackground = new Mat();
            _wSubstractBackground = new Mat();
            _whsv = new Mat();
            _wycrcb = new Mat();
            _whisto_h = new Mat();
            _whisto_s = new Mat();
            _whisto_v = new Mat();
            _wcropped = new Mat();
            Result = new SkinPresetHSV();
            ColorSkinCalibrationConfig.PropertyChanged += Config_PropertyChanged;
            _haarCascade = new CascadeClassifier(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "haarcascade_frontalface_alt2.xml"));
        }

        private void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DrawTarget(_config.ColorSkinCalibrationConfig);
        }

        public void ActivateCalibration(Mat currentFrame, ColorSkinCalibrationConfig config = null)
        {
            _target = new Mat();
            _initialTarget = new Mat();
            Cv2.CopyTo(currentFrame, _target);
            Cv2.CopyTo(currentFrame, _initialTarget);
            if (config != null)
                _config.ColorSkinCalibrationConfig = config;
            CanStartCalibration = true;

            RaisePropertyChanged(() => MaxWidth);
            RaisePropertyChanged(() => MaxHeight);
            DrawTarget(_config.ColorSkinCalibrationConfig);

        }
        public void DrawTarget(ColorSkinCalibrationConfig config)
        {
            if (_useHeadDetection)
                return;
            if (_initialTarget != null && _initialTarget.Width > 0)
            {
                Rect handZone = new Rect(_config.ColorSkinCalibrationConfig.TLX, _config.ColorSkinCalibrationConfig.TLY, Math.Min(_config.ColorSkinCalibrationConfig.Width, MaxWidth), Math.Min(_config.ColorSkinCalibrationConfig.Height, MaxHeight));
                Cv2.CopyTo(_initialTarget, _target);
                Cv2.Rectangle(_target, handZone, new Scalar(0, 255, 0));
                RaisePropertyChanged(() => MaxWidth);
                RaisePropertyChanged(() => MaxHeight);
                W_HANDTARGET = _target.ToBitmap().ToBitmapImage();
            }
        }
        public async Task StartCalibration(ColorSkinCalibrationConfig config = null)
        {
            _samples.Clear();
            if (config != null)
                _config.ColorSkinCalibrationConfig = config;
            _calibrationStartDate = DateTime.Now;
            if (!_useHeadDetection)
                _config.BackGroundRemoveConfig.IsActive = false;

            Rect handZone = new Rect(_config.ColorSkinCalibrationConfig.TLX, _config.ColorSkinCalibrationConfig.TLY, Math.Min(_config.ColorSkinCalibrationConfig.Width, MaxWidth), Math.Min(_config.ColorSkinCalibrationConfig.Height, MaxHeight));
            var tmp = new Mat(_target, handZone);


            var pipelineBackGround = new Pipeline()
            .Pipe(new BlurFilter(_config.BlurConfig));
            await pipelineBackGround.RunAsync(tmp);

            Cv2.CvtColor(pipelineBackGround.OutputMat, _wbackground, ColorConversionCodes.BGR2GRAY);
            CalibrationStarted = true;
            Result = new SkinPresetHSV();
        }
        public async Task AddSample(Mat frame)
        {
            if (RemainingSeconds > 0)
            {

                Rect roi = new Rect();
                if (frame.Width > 0)
                {
                    _target = new Mat();
                    Cv2.CopyTo(frame, _target);

                    if (_useHeadDetection)
                    {

                        Mat gray = new Mat();
                        Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                        Rect[] faces = _haarCascade.DetectMultiScale(gray, _config.FaceConfig.Scale, _config.FaceConfig.MinNeighBours, HaarDetectionType.ScaleImage, new Size(_config.FaceConfig.MinSize, _config.FaceConfig.MinSize));
                        if (faces.Count() > 0)
                        {
                            var maxArea = faces.Max(x => x.Width * x.Height);
                            roi = faces.FirstOrDefault(x => x.Width * x.Height == maxArea);
                        }
                    }
                    else
                        roi = new Rect(_config.ColorSkinCalibrationConfig.TLX, _config.ColorSkinCalibrationConfig.TLY, Math.Min(_config.ColorSkinCalibrationConfig.Width, MaxWidth), Math.Min(_config.ColorSkinCalibrationConfig.Height, MaxHeight));

                    if (roi != null && roi.Width > 0)
                    {
                        _wcropped = new Mat(frame, roi);

                        var pipeline = new Pipeline()
                        .Pipe(new BlurFilter(_config.BlurConfig))
                        .Pipe(new BackgroundRemoveFilter(_wbackground, _config.BackGroundRemoveConfig));
                        await pipeline.RunAsync(_wcropped);
                        _wSubstractBackground = pipeline.OutputMat;




                        HSVFilter hsvFilter = new HSVFilter();
                        hsvFilter.Apply(_wSubstractBackground);
                        _whsv = hsvFilter.Output;

                        YCrCbFilter ycrcbFilter = new YCrCbFilter();
                        ycrcbFilter.Apply(_wSubstractBackground);
                        _wycrcb = ycrcbFilter.Output;



                        var sample = ColorSkinCalibrationSample.From(_wSubstractBackground, _whsv, _wycrcb);

                        _samples.Add(sample);
                        
                        _whisto_h = GetHistogramGraph(sample.HSV.Histogram_H, _wcropped.Width, _wcropped.Height);
                        _whisto_s = GetHistogramGraph(sample.HSV.Histogram_S, _wcropped.Width, _wcropped.Height);
                        _whisto_v = GetHistogramGraph(sample.HSV.Histogram_V, _wcropped.Width, _wcropped.Height);


                        Result = new SkinPresetHSV
                        {
                            HSV = new HSV
                            {
                                H = (int)_samples.Select(x => (double)x.HSV.MeanH.X).Median(),
                                S = (int)_samples.Select(x => (double)x.HSV.MeanS.X).Median(),
                                V = (int)_samples.Select(x => (double)x.HSV.MeanV.X).Median()
                            },
                             YCRCB = new YCrCb
                             {
                                 Y = (int)_samples.Select(x => (double)x.YCRCB.MeanY.X).Median(),
                                 Cr = (int)_samples.Select(x => (double)x.YCRCB.MeanCr.X).Median(),
                                 Cb = (int)_samples.Select(x => (double)x.YCRCB.MeanCb.X).Median()
                             }
                        };



                        RaisePropertyChanged(() => W_CROPPED);
                        RaisePropertyChanged(() => W_BACKGROUND_SUBSTRACTED);
                        RaisePropertyChanged(() => W_HSV);
                        RaisePropertyChanged(() => W_HISTO_H);
                        RaisePropertyChanged(() => W_HISTO_S);
                        RaisePropertyChanged(() => W_HISTO_V);
                        RaisePropertyChanged(() => W_HANDTARGET);
                    }

                    RaisePropertyChanged(() => RemainingSeconds);

                }
            }
            else
            {
                Result = new SkinPresetHSV
                {
                    HSV = new HSV
                    {
                        H = (int)_samples.Select(x => (double)x.HSV.MeanH.X).Median(),
                        S = (int)_samples.Select(x => (double)x.HSV.MeanS.X).Median(),
                        V = (int)_samples.Select(x => (double)x.HSV.MeanV.X).Median()
                    },
                    YCRCB = new YCrCb
                    {
                        Y = (int)_samples.Select(x => (double)x.YCRCB.MeanY.X).Median(),
                        Cr = (int)_samples.Select(x => (double)x.YCRCB.MeanCr.X).Median(),
                        Cb = (int)_samples.Select(x => (double)x.YCRCB.MeanCb.X).Median()
                    }
                };
                CalibrationStarted = false;
                CanStartCalibration = false;
                W_HANDTARGET = null;
                RaisePropertyChanged(() => W_HANDTARGET);

            }
        }


        private Mat GetHistogramGraph(Mat histo, int width, int height)
        {
            const int histogramSize = 256;
            int marginBottom = 100;
            int marginLeft = 100;
            int[] dimensions = { histogramSize };
            Mat render = new Mat(new Size(width + marginLeft, height + marginBottom), MatType.CV_8UC3, new Scalar(0, 255, 0));
            double minVal, maxVal;
            Cv2.MinMaxLoc(histo, out minVal, out maxVal);

            Scalar color = Scalar.All(100);
            // Scales and draws histogram
            histo = histo * (maxVal != 0 ? height / maxVal : 0.0);

            for (int j = 10; j < dimensions[0]; ++j)
            {
                int binW = (int)((double)width / dimensions[0]);
                render.Rectangle(
                    new Point(j * binW + marginLeft, render.Rows - marginBottom),
                    new Point((j + 1) * binW + marginLeft, render.Rows - marginBottom - (int)(histo.Get<float>(j))),
                    color,
                    -1);
                if (j % 50 == 0)
                    render.PutText(j.ToString(), new Point((j + 1) * binW + marginLeft, render.Rows - (marginBottom / 2)), HersheyFonts.HersheySimplex, 0.5, new Scalar(255, 0, 0), 1, LineTypes.Link8, false);
                //Console.WriteLine($@"j:{j} P1: {j * binW},{render.Rows} P2:{(j + 1) * binW},{render.Rows - (int)histo.Get<int>(j)}");  //for Debug
                //render.Rectangle(
                //    new Point(j * binW, render.Rows - (int)histo.Get<int>(j)),
                //    new Point((j + 1) * binW, render.Rows),
                //    color,
                //    -1);
            }
            render.PutText($"{maxVal.ToString()} - {(int)((double)maxVal / (width * height) * 100)} %", new Point(marginLeft / 2, height / 2), HersheyFonts.HersheySimplex, 0.5, new Scalar(255, 0, 0), 1, LineTypes.Link8, false);
            return render;
        }
    }
}
