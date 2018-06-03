using Osmo.Core;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osmo.ViewModel
{
    class AnimationViewModel : ViewModelBase
    {
        private VeryObservableCollection<string> mAnimation = new VeryObservableCollection<string>("Animation");
        private int mCurrentFrame;
        private Thread mAnimationThread;
        private ImageSource mImage;

        public VeryObservableCollection<string> Animation
        {
            get => mAnimation;
            set
            {
                mAnimation = value;
                InvokePropertyChanged("Animation");
            }
        }

        public string CurrentElementPath
        {
            get => Animation?[mCurrentFrame];
        }

        public int CurrentFrame
        {
            get => mCurrentFrame;
            set
            {
                mCurrentFrame = value;
                mImage = Helper.LoadImage(CurrentElementPath);
                mImage.Freeze();
                InvokePropertyChanged("Image");
            }
        }

        /// <summary>
        /// Defines at which framerate the animation should run. (Default = 20)
        /// </summary>
        public int Framerate { get; set; } = 3;

        public ImageSource Image
        {
            get => mImage;
        }

        /// <summary>
        /// This property is optional. You'll probably only need this property for osu!taiko.
        /// </summary>
        public int[] FrameOrder { get; set; }

        public void StartAnimation()
        {
            if (mAnimationThread?.IsAlive ?? false)
            {
                StopAnimation();
            }

            mCurrentFrame = -1;
            mAnimationThread = new Thread(RunAnimation);
            mAnimationThread.Start();
        }

        private void RunAnimation()
        {
            if (FrameOrder == null)
            {
                while (true)
                {
                    if (CurrentFrame < mAnimation.Count - 1)
                    {
                        CurrentFrame++;
                    }
                    else
                    {
                        CurrentFrame = 0;
                    }

                    Thread.Sleep(1000 / Framerate);
                }
            }
            else
            {
                int currentFrameOrderIndex = 0;
                int lastValidFrame = 0;
                while (true)
                {
                    if (FrameOrder[currentFrameOrderIndex] < mAnimation.Count)
                    {
                        CurrentFrame = FrameOrder[currentFrameOrderIndex];
                        lastValidFrame = CurrentFrame;
                    }
                    else
                    {
                        CurrentFrame = lastValidFrame;
                    }

                    if (currentFrameOrderIndex < FrameOrder.Length - 1)
                    {
                        currentFrameOrderIndex++;
                    }
                    else
                    {
                        currentFrameOrderIndex = 0;
                    }

                    Thread.Sleep(1000 / Framerate);
                }
            }
        }

        public void StopAnimation()
        {
            mAnimationThread?.Abort();
        }
    }
}
