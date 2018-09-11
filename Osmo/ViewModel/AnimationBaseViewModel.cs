using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.Core.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Osmo.ViewModel
{
    class AnimationBaseViewModel : ViewModelBase
    {
        private VeryObservableCollection<string> mAnimation = new VeryObservableCollection<string>("Animation");
        private int mCurrentFrameIndex;
        private Thread mAnimationThread;
        private bool mIsAnimationPlaying;
        private int mFrameRate = 30;
        protected int mCurrentFrameOrderIndex = 0;

        private ImageSource mCurrentFrame;

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
            get => Animation?.Count > 0 ? Animation[mCurrentFrameIndex] : null;
        }

        public int CurrentFrame
        {
            get => mCurrentFrameIndex;
            set
            {
                mCurrentFrameIndex = value;
                Image = Helper.LoadImage(CurrentElementPath);
                Image?.Freeze();
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
        public int FrameRate
        {
            get => mFrameRate;
            set
            {
                if (value <= 60)
                    mFrameRate = value;
                else
                    mFrameRate = 60;
                InvokePropertyChanged("FrameRate");
            }
        }

        public ImageSource Image {
            get => mCurrentFrame;
            protected set
            {
                mCurrentFrame = value;
                InvokePropertyChanged("Image");
            }
        }

        /// <summary>
        /// This property is optional. You'll probably only need this property for osu!taiko.
        /// </summary>
        public int[] FrameOrder { get; set; }

        public void LoadAnimation(SkinElement element)
        {
            mAnimation.Clear();

            mAnimation.Add(element.GetAnimatedElements(), false);
            CurrentFrame = 0;

            SkinningEntry details = element.ElementDetails as SkinningEntry;
            FrameOrder = details.FrameOrder;
        }

        public void StartAnimation()
        {
            if (mAnimationThread?.IsAlive ?? false)
            {
                StopAnimation();
            }

            mCurrentFrameIndex = -1;
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
    }
}
