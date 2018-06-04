using Osmo.Core;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace Osmo.ViewModel
{
    class AnimationViewModel : ViewModelBase
    {
        private VeryObservableCollection<string> mAnimation = new VeryObservableCollection<string>("Animation");
        private int mCurrentFrame;
        private Thread mAnimationThread;
        private bool mIsAnimationPlaying;
        private int mFrameRate = 10;
        private int mCurrentFrameOrderIndex = 0;

        public VeryObservableCollection<string> Animation
        {
            get => mAnimation;
            set
            {
                mAnimation = value;
                InvokePropertyChanged("Animation");
            }
        }

        public int[] FrameRateSelector => new int[] { 5, 10, 20, 30, 60 };

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
                Image = Helper.LoadImage(CurrentElementPath);
                Image.Freeze();
                InvokePropertyChanged("Image");
                InvokePropertyChanged("CurrentFrame");
            }
        }

        public bool IsAnimationPlaying
        {
            get => mIsAnimationPlaying;
            set
            {
                mIsAnimationPlaying = value;
                InvokePropertyChanged("IsAnimationPlaying");
            }
        }

        /// <summary>
        /// Defines at which framerate the animation should run. (Default = 20)
        /// </summary>
        public int FrameRate { get => mFrameRate;
            set
            {
                if (value <= 60)
                    mFrameRate = value;
                else
                    mFrameRate = 60;
                InvokePropertyChanged("FrameRate");
            }
        }

        public ImageSource Image { get; private set; }

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
            IsAnimationPlaying = true;
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

                    Thread.Sleep(1000 / FrameRate);
                }
            }
            else
            {
                int lastValidFrame = 0;
                while (true)
                {
                    if (FrameOrder[mCurrentFrameOrderIndex] < mAnimation.Count)
                    {
                        CurrentFrame = FrameOrder[mCurrentFrameOrderIndex];
                        lastValidFrame = CurrentFrame;
                    }
                    else
                    {
                        CurrentFrame = lastValidFrame;
                    }

                    if (mCurrentFrameOrderIndex < FrameOrder.Length - 1)
                    {
                        mCurrentFrameOrderIndex++;
                    }
                    else
                    {
                        mCurrentFrameOrderIndex = 0;
                    }

                    Thread.Sleep(1000 / mFrameRate);
                }
            }
        }

        public void StopAnimation()
        {
            mAnimationThread?.Abort();
            IsAnimationPlaying = false;
        }

        public void PreviousFrame()
        {
            StopAnimation();
            if (FrameOrder == null)
            {
                if (mCurrentFrame - 1 >= 0)
                {
                    CurrentFrame--;
                }
                else
                {
                    CurrentFrame = mAnimation.Count - 1;
                }
            }
            else
            {
                if (mCurrentFrameOrderIndex - 1 >= 0)
                {
                    mCurrentFrameOrderIndex--;
                }
                else
                {
                    mCurrentFrameOrderIndex = FrameOrder.Length - 1;
                }
                CurrentFrame = FrameOrder[mCurrentFrameOrderIndex];
            }
        }

        public void NextFrame()
        {
            StopAnimation();
            if (FrameOrder == null)
            {
                if (mCurrentFrame + 1 < mAnimation.Count)
                {
                    CurrentFrame++;
                }
                else
                {
                    CurrentFrame = 0;
                }
            }
            else
            {
                if (mCurrentFrameOrderIndex + 1 < FrameOrder.Length)
                {
                    mCurrentFrameOrderIndex++;
                }
                else
                {
                    mCurrentFrameOrderIndex = 0;
                }
                CurrentFrame = FrameOrder[mCurrentFrameOrderIndex];
            }
        }
    }
}
