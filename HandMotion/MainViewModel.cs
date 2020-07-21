using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandMotion.filters;
using HandMotion.Helpers;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HandMotion
{
    public interface IMainViewModel : IViewModel { }

    public class MainViewModel : ViewModelBase
    {
        //BackgroundSubtractor backSub;
        private string _samplePath = Path.Combine($@"c:\#handmotion\{DateTime.Now.ToString("yyyyMMddhhmmss")}");
        private string _defaultConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

        public FrameRate FrameRate { get; set; }
        public ImageSource W_BACKGROUND { get; set; }
        public ImageSource W_OUTPUT { get; set; }

        VideoCapture capture;
        private Pipeline _pipeline;
        public Pipeline Pipeline
        {
            get { return _pipeline; }
            set { _pipeline = value; RaisePropertyChanged(() => Pipeline); }
        }

        private ColorSkinCalibrationViewModel _colorSkinCalibration;
        public ColorSkinCalibrationViewModel ColorSkinCalibration
        {
            get { return _colorSkinCalibration; }
            set { _colorSkinCalibration = value; RaisePropertyChanged(() => ColorSkinCalibration); }
        }
        private HandTrackerViewModel _tracker;
        public HandTrackerViewModel Tracker
        {
            get { return _tracker; }
            set { _tracker = value; RaisePropertyChanged(() => Tracker); }
        }


        public Config Config { get; set; }
        private bool _captureStarted;

        private Mat _backgroundMat = new Mat();
        private Mat _currentFrame;

        private bool _showDebug;
        public bool ShowDebug
        {
            get { return _showDebug; }
            set { _showDebug = value; RaisePropertyChanged(() => ShowDebug); }
        }

        public RelayCommand StartCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand TakeScreenShotCommand { get; private set; }
        public RelayCommand SaveConfigCommand { get; private set; }
        public RelayCommand LoadDefaultConfigCommand { get; private set; }
        public IAsyncCommand CalibrateBackgroundCommand { get; private set; }
        public RelayCommand CalibrateColorSkinCommand { get; private set; }
        public RelayCommand ActivateCalibrateColorSkinCommand { get; private set; }

        public MainViewModel()
        {
            StartCommand = new RelayCommand(OnStart, OnCanStart);
            StopCommand = new RelayCommand(OnStop, OnCanStop);
            TakeScreenShotCommand = new RelayCommand(TakeScreenShot, CanTakeScreenShot);
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadDefaultConfigCommand = new RelayCommand(LoadPipeline);
            CalibrateBackgroundCommand = new AsyncCommand(GetBackground, OnCanStop);
            CalibrateColorSkinCommand = new RelayCommand(CalibrateColorSkin, OnCanStartSkinCalibration);
            ActivateCalibrateColorSkinCommand = new RelayCommand(ActivateCalibrationColorSkin, OnCanActivateSkinCalibration);
            FrameRate = new FrameRate();
            LoadConfig();
            LoadColorSkinCalibration();
            LoadPipeline();

        }


        private void LoadConfig()
        {
            if (File.Exists(_defaultConfigPath))
            {
                Config = Helper.Load<Config>(_defaultConfigPath);
            }
            else
                Config = Config.Default();
            RaisePropertyChanged(() => Config);
        }
        private void LoadColorSkinCalibration()
        {
            ColorSkinCalibration = new ColorSkinCalibrationViewModel(Config);
        }
        private bool OnCanActivateSkinCalibration()
        {
            return OnCanStop();
        }
        private async void CalibrateColorSkin()
        {
            await ColorSkinCalibration.StartCalibration(Config.ColorSkinCalibrationConfig);
            while (ColorSkinCalibration.CalibrationStarted)
            {
                await ColorSkinCalibration.AddSample(_currentFrame);
                await Task.Delay(40);
            }
            Config.SkinDetectConfig.HSVThresholdConfig.H = new MinMax { Min = Math.Max(ColorSkinCalibration.Result.HSV.H - 20,0), Max = Math.Min(ColorSkinCalibration.Result.HSV.H + 20,255) };
            Config.SkinDetectConfig.HSVThresholdConfig.S = new MinMax { Min = Math.Min(ColorSkinCalibration.Result.HSV.S-10,0), Max = 255 };
            Config.SkinDetectConfig.HSVThresholdConfig.V = new MinMax { Min = 20, Max = 255 };

            Config.SkinDetectConfig.YCrCbThresholdConfig.Y = new MinMax { Min = 0, Max = 255 };
            Config.SkinDetectConfig.YCrCbThresholdConfig.Cr = new MinMax { Min = Math.Max(ColorSkinCalibration.Result.YCRCB.Cr - 20,0), Max = Math.Min(ColorSkinCalibration.Result.YCRCB.Cr +20,255) };
            Config.SkinDetectConfig.YCrCbThresholdConfig.Cb = new MinMax { Min = Math.Max(ColorSkinCalibration.Result.YCRCB.Cb - 20, 0), Max = Math.Min(ColorSkinCalibration.Result.YCRCB.Cb + 20, 255) };

            SaveConfig();
            LoadConfig();
        }
        private bool OnCanStartSkinCalibration()
        {
            return ColorSkinCalibration.CanStartCalibration;
        }

        private async void ActivateCalibrationColorSkin()
        {
            while (!ColorSkinCalibration.CalibrationStarted)
            {
                ColorSkinCalibration.ActivateCalibration(_currentFrame, Config.ColorSkinCalibrationConfig);
                CalibrateColorSkinCommand.RaiseCanExecuteChanged();
                await Task.Delay(20);
            }
        }

        private async Task GetBackground()
        {
            var pipelineBackGround = new Pipeline()
                        .Pipe(new ResizeFilter(Config.ScaleDownConfig))
                        .Pipe(new RemoveFaceFilter(Config.FaceConfig))
                        .Pipe(new EqualizeHistogramFilter(Config.EqHistoConfig))
                        .Pipe(new BlurFilter(Config.BlurConfig));
            await pipelineBackGround.RunAsync(_currentFrame);

            Cv2.CvtColor(pipelineBackGround.OutputMat, _backgroundMat, ColorConversionCodes.BGR2GRAY);
            W_BACKGROUND = _backgroundMat.ToBitmap().ToBitmapImage();
            RaisePropertyChanged(() => W_BACKGROUND);
        }

        private void LoadPipeline()
        {

            Pipeline = new Pipeline()
                        .Pipe(new ResizeFilter(Config.ScaleDownConfig))
                        .Pipe(new RemoveFaceFilter(Config.FaceConfig))
                        .Pipe(new EqualizeHistogramFilter(Config.EqHistoConfig))
                        .Pipe(new BlurFilter(Config.BlurConfig))
                        .Pipe(new BackgroundRemoveFilter(_backgroundMat, Config.BackGroundRemoveConfig))
                        .Pipe(new SkinDetectFilter(Config.SkinDetectConfig))
                        //.Pipe(new HSVFilter(Config.HsvConfig))
                        //.Pipe(new HSVThresoldFilter(Config.HSVThreshold))
                        .Pipe(new MorphOpenFilter(Config.MorphConfig));

            Tracker = new HandTrackerViewModel(Config.TrackerConfig);
        }




        private void SaveConfig()
        {
            Helper.Save(Config, _defaultConfigPath);
        }

        private bool CanTakeScreenShot()
        {
            return capture != null && capture.IsOpened();
        }

        private async void TakeScreenShot()
        {
            if (!Directory.Exists(_samplePath))
                Directory.CreateDirectory(_samplePath);
            //var bitmap = await ReadVideoStreamAsync();
            if (_currentFrame != null && _currentFrame.Width > 0)
                _currentFrame.ToBitmap().Save(Path.Combine(_samplePath, $"{(Directory.GetFiles(_samplePath).Length + 1).ToString().PadLeft(3, '0')}.jpg"));
        }

        private bool OnCanStart()
        {
            return capture == null;
        }

        private async void OnStart()
        {
            Load();

        }
        private bool OnCanStop()
        {
            return capture != null && capture.IsOpened();
        }

        private async void OnStop()
        {
            _captureStarted = false;
            capture.Dispose();
            capture = null;

            //PipelineResult = new PipelineResult();
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            TakeScreenShotCommand.RaiseCanExecuteChanged();
            CalibrateColorSkinCommand.RaiseCanExecuteChanged();
            ActivateCalibrateColorSkinCommand.RaiseCanExecuteChanged();

        }

        private async Task Load()
        {

            capture = new VideoCapture();
            capture.Open(0, VideoCaptureAPIs.ANY);
            if (!capture.IsOpened())
            {
                return;
            }
            else
            {
                //backSub = OpenCvSharp.BackgroundSubtractorMOG.Create();
                _currentFrame = (await ReadVideoStreamAsync());
                //EqualizeHistogramFilter filter = new EqualizeHistogramFilter(new EqualizeHistogramConfig { IsActive = true });
                //filter.Apply(colorMat);
                await GetBackground();
                _captureStarted = true;
                StartCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
                TakeScreenShotCommand.RaiseCanExecuteChanged();
                CalibrateBackgroundCommand.RaiseCanExecuteChanged();
                CalibrateColorSkinCommand.RaiseCanExecuteChanged();
                ActivateCalibrateColorSkinCommand.RaiseCanExecuteChanged();
                Tracker.RunAsync();
                while (_captureStarted)
                {
                    _currentFrame = await ReadVideoStreamAsync();


                    await Pipeline.RunAsync(_currentFrame);
                    //Tracker.Tracker.DetectHandFrom(Pipeline);

                    DisplayOutput(Pipeline, Tracker, _showDebug);
                    FrameRate.Tick();
                }
            }
        }

        private void DisplayOutput(Pipeline pipeline, HandTrackerViewModel handTracker, bool showDebug)
        {
            if (handTracker.ShowOutput)
            {
                if (handTracker.Tracker.TrackerVisualisation != null)
                    W_OUTPUT = handTracker.Tracker.TrackerVisualisation.ToBitmap().ToBitmapImage();
                else
                    W_OUTPUT = pipeline.OutputMat.ToBitmap().ToBitmapImage();
                RaisePropertyChanged(() => W_OUTPUT);
            }
            if (showDebug)
            {
                foreach (var step in pipeline.Steps)
                {
                    step.Result.Compute();
                }
            }
        }


        private async Task<Mat> ReadVideoStreamAsync()
        {

            using (var srcMat = capture.RetrieveMat())
            {

                var dstMat = new Mat();
                var colorMat = new Mat();
                Cv2.Flip(srcMat, dstMat, FlipMode.Y);
                //Cv2.CvtColor(dstMat, colorMat,ColorConversionCodes.BGR2RGB);



                
                //if(_currentFrame != null && _currentFrame.Width>0)
                //{
                    //Mat ycrcb = new Mat();
                    //Mat lastycrcb = new Mat();
                    //OpenCvSharp.Cv2.CvtColor(dstMat, ycrcb, ColorConversionCodes.BGR2YCrCb);
                    //OpenCvSharp.Cv2.CvtColor(_currentFrame, lastycrcb, ColorConversionCodes.BGR2YCrCb);
                    //Mat[] Channels;
                    //Mat[] Channels_last;
                    //Mat outputycrcb = new Mat();
                    //Mat output = new Mat();
                    //Cv2.Split(ycrcb, out Channels);
                    //Cv2.Split(lastycrcb, out Channels_last);
                    //var indexer_frame = Channels[0].GetGenericIndexer<byte>();
                    //var indexer_frame_last = Channels_last[0].GetGenericIndexer<byte>();

                    //for (int i = 0; i < Channels[0].Rows; i++)
                    //{
                    //    for (int j = 0; j < Channels[0].Cols; j++)
                    //    {
                    //        var intensity = indexer_frame[i, j];
                    //        var intensity_last = indexer_frame_last[i, j];
                    //        //var newintensity = indexer_newFrame[i, j];
                    //        indexer_frame[i, j] = (byte)((intensity + intensity_last)/2);
                    //        //if (intensity > 3 * Config.BackGroundRemoveConfig.BackGroundThreshold)
                    //        //{
                    //        //    //frame.Item0 = 0;
                    //        //    indexer_frame[i, j] = (byte)(3 * Config.BackGroundRemoveConfig.BackGroundThreshold);
                    //        //}
                    //        //else
                    //        //{
                    //        //    indexer_frame[i, j] = intensity;
                    //        //}
                    //    }
                    //}


                    //Cv2.Merge(Channels, outputycrcb);
                    //Cv2.CvtColor(outputycrcb, output, ColorConversionCodes.YCrCb2BGR);
                    //return output;
                //}
                

                return dstMat;

            }
        }


    }
}
