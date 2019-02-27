using Osmo.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;

namespace Osmo.ViewModel
{
    public class SBCreatorViewModel : ViewModelBase
    {
        Thread mAnimationThread;
        private int mCurrentFrameIndex = 0;
        private bool mIsAnimationPlaying;
        
        private List<ImageSource> mAnimation = new List<ImageSource>();
        private List<string> mDummyNames = new List<string>();
        private string mSourceImagePath;
        private ImageSource mSourceImage;
        private int mFrameRate = 30;
        private int mNumberOfFrames = 36; //More frames = slider spinning is displayed more smooth
        private double mAnimationDuration = 1;
        private bool mIsAnimationGenerated;
        private string mOutputName = "sliderb";

        private bool mIsGenerateEnabled = false;

        public string OutputName
        {
            get => mOutputName;
            set
            {
                mOutputName = value;
                InvokePropertyChanged();
            }
        }

        public bool IsGenerateEnabled
        {
            get => mIsGenerateEnabled;
            set
            {
                mIsGenerateEnabled = value;
                InvokePropertyChanged();
            }
        }

        public bool IsAnimationPlaying
        {
            get => mIsAnimationPlaying;
            set
            {
                mIsAnimationPlaying = value;
                InvokePropertyChanged();
            }
        }

        public bool IsAnimationGenerated
        {
            get => mIsAnimationGenerated;
            set
            {
                mIsAnimationGenerated = value;
                InvokePropertyChanged();
            }
        }

        public string SourceImagePath
        {
            get => mSourceImagePath;
            set
            {
                mSourceImagePath = value;
                if (value != null)
                {
                    StopAnimation();
                    SourceImage = Helper.LoadImage(value);
                    SourceImageName = value.Substring(value.LastIndexOf('\\') + 1); //.Remove(value.LastIndexOf('.'));
                    //StartAnimation();
                }
                else
                {
                    SourceImage = null;
                    SourceImageName = "";
                }

                IsGenerateEnabled = !string.IsNullOrWhiteSpace(mSourceImagePath) &&
                    FrameRate > 0 &&
                    AnimationDuration > 0;
                InvokePropertyChanged();
            }
        }

        public string SourceImageName
        {
            get; set;
        }

        public List<string> DummyNames
        {
            get => mDummyNames;
            set
            {
                mDummyNames = value;
                InvokePropertyChanged();
            }
        }

        public ImageSource SourceImage
        {
            get => mSourceImage;
            set
            {
                mSourceImage = value;
                InvokePropertyChanged();
            }
        }

        public ImageSource CurrentFrame
        {
            get => mAnimation?[CurrentFrameIndex];
        }

        public List<ImageSource> Animation
        {
            get => mAnimation;
        }

        public int FrameRate
        {
            get => mFrameRate;
            set
            {
                mFrameRate = value;
                NumberOfFrames = (int)value * FrameRate;

                IsGenerateEnabled = !string.IsNullOrWhiteSpace(mSourceImagePath) &&
                    FrameRate > 0 &&
                    AnimationDuration > 0;
                InvokePropertyChanged();
            }
        }

        public int NumberOfFrames
        {
            get => mNumberOfFrames;
            private set
            {
                mNumberOfFrames = value;
                InvokePropertyChanged();
            }
        }

        public double AnimationDuration
        {
            get => mAnimationDuration;
            set
            {
                mAnimationDuration = value;
                NumberOfFrames = (int)value * FrameRate;

                IsGenerateEnabled = !string.IsNullOrWhiteSpace(mSourceImagePath) &&
                    FrameRate > 0 &&
                    AnimationDuration > 0;
                InvokePropertyChanged();
            }
        }

        public int CurrentFrameIndex
        {
            get => mCurrentFrameIndex;
            set
            {
                StopAnimation();
                mCurrentFrameIndex = value;
                InvokePropertyChanged("CurrentFrame");
            }
        }

        public void Reset()
        {
            StopAnimation();
            SourceImagePath = null;
            AnimationDuration = 1;
            FrameRate = 30;
            mAnimation.Clear();
            CurrentFrameIndex = 0;
            mDummyNames.Clear();
            InvokePropertyChanged("DummyNames");
        }

        public void GenerateAnimation()
        {
            IsAnimationGenerated = false;
            mAnimation = GetSliderballAnimation(SourceImagePath, NumberOfFrames);

            DummyNames = GetDummyNames(mAnimation.Count, SourceImageName);
            IsAnimationGenerated = true;
        }

        public void StartAnimation()
        {
            StopAnimation();
            
            mAnimationThread = new Thread(RunAnimation);
            mAnimationThread.Start();
            IsAnimationPlaying = true;
        }

        public void StopAnimation()
        {
            if (mAnimationThread?.IsAlive ?? false)
            {
                mAnimationThread?.Abort();
                IsAnimationPlaying = false;
            }
        }

        private void RunAnimation()
        {
            while (true)
            {
                if (mCurrentFrameIndex < mAnimation.Count - 1)
                {
                    mCurrentFrameIndex++;
                }
                else
                {
                    mCurrentFrameIndex = 0;
                }

                CycleFrames();

                Thread.Sleep(1000 / FrameRate);
                //Thread.Sleep(new TimeSpan(MillisecondsInTicks(1000) / FrameRate));
            }
        }

        private long MillisecondsInTicks(int milliseconds)
        {
            return TimeSpan.TicksPerMillisecond * milliseconds;
        }

        private void CycleFrames()
        {
            int nextFrameIndex = mCurrentFrameIndex + 1;

            if (nextFrameIndex >= mAnimation.Count - 1)
            {
                nextFrameIndex = 0;
            }

            InvokePropertyChanged("CurrentFrame");
        }

        private List<string> GetDummyNames(int frames, string fileName)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < mAnimation.Count; i++)
            {
                names.Add(mOutputName + i + ".png");
            }

            return names;
        }

        private List<ImageSource> GetSliderballAnimation(string sourcePath, int numberOfFrames)
        {
            float anglePerFrame = 360 / numberOfFrames;

            List<ImageSource> frames = new List<ImageSource>();
            for (int frame = 1; frame <= numberOfFrames; frame++)
            {
                float angle = anglePerFrame * frame;
                Bitmap rotatedFrame;
                if (frame < numberOfFrames)
                {
                    rotatedFrame = RotateImage(sourcePath, angle);
                }
                else
                {
                    rotatedFrame = RotateImage(sourcePath, 360);
                }
                frames.Add(rotatedFrame.ToImageSource());
            }

            return frames;
        }

        private Bitmap RotateImage(string sourcePath, float angle)
        {
            return RotateImage(new Bitmap(sourcePath), angle);
        }

        private Bitmap RotateImage(Bitmap bmp, float angle)
        {
            PointF offset = new PointF((float)bmp.Width / 2, (float)bmp.Height / 2);

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(bmp.Width, bmp.Height);
            rotatedBmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            //make a graphics object from the empty bitmap
            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {

                g.InterpolationMode = InterpolationMode.High;
                //Put the rotation point in the center of the image
                g.TranslateTransform(offset.X, offset.Y);

                //rotate the image
                g.RotateTransform(angle);

                //move the image back
                g.TranslateTransform(-offset.X, -offset.Y);

                //draw passed in image onto graphics object
                g.DrawImage(bmp, new PointF(0, 0));
            }

            return rotatedBmp;
        }

        public void PreviousFrame()
        {
            StopAnimation();
            if (mCurrentFrameIndex - 1 >= 0)
            {
                CurrentFrameIndex--;
            }
            else
            {
                CurrentFrameIndex = mAnimation.Count - 1;
            }
        }

        public void NextFrame()
        {
            StopAnimation();
            if (mCurrentFrameIndex + 1 < mAnimation.Count)
            {
                CurrentFrameIndex++;
            }
            else
            {
                CurrentFrameIndex = 0;
            }
        }
    }
}
