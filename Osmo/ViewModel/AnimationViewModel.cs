using Osmo.Core;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace Osmo.ViewModel
{
    class AnimationViewModel : AnimationBaseViewModel
    {
        public void PreviousFrame()
        {
            StopAnimation();
            if (FrameOrder == null)
            {
                if (CurrentFrame - 1 >= 0)
                {
                    CurrentFrame--;
                }
                else
                {
                    CurrentFrame = Animation.Count - 1;
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
                if (CurrentFrame + 1 < Animation.Count)
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
