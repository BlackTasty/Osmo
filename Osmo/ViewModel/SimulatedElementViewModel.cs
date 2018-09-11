using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.Core.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Osmo.ViewModel
{
    class SimulatedElementViewModel : AnimationBaseViewModel
    {
        private AnimationType mAnimationType;
        private SimulatedElementType mElementType;
        private SkinElement mFixedElement;
        private SkinElement mCountElement; //Used for elements such as circles and sliders
        private SkinElement mAnimatedElement;
        private Storyboard mTransformation;
        private Size mTargetSize = new Size(200, 200);

        public AnimationType AnimationType
        {
            get => mAnimationType;
            set => mAnimationType = value;
        }

        public SimulatedElementType ElementType
        {
            get => mElementType;
            set
            {
                mElementType = value;
                InvokePropertyChanged("FixedElementVisivle");
                InvokePropertyChanged("AnimatedElementVisible");
            }
        }

        public Storyboard TransformAnimation
        {
            get => mTransformation;
            set => mTransformation = value;
        }

        public SkinElement FixedElement
        {
            get => mFixedElement;
            set
            {
                mFixedElement = value;
                ApplyNewTargetSize();
                FixedElementImage = Helper.LoadImage(value.Path);
                InvokePropertyChanged("FixedElementImage");
            }
        }

        public SkinElement CountElement
        {
            get => mCountElement;
            set
            {
                mCountElement = value;
                CountElementImage = Helper.LoadImage(value.Path);
                InvokePropertyChanged("CountElementImage");
            }
        }

        public SkinElement AnimatedElement
        {
            get => mAnimatedElement;
            set
            {
                mAnimatedElement = value;
                ApplyNewTargetSize();
                Image = Helper.LoadImage(value.Path);

                if (mAnimationType == AnimationType.Predefined)
                {
                    LoadAnimation(value);
                }
            }
        }

        public Size TargetSize
        {
            get => mTargetSize;
            set
            {
                mTargetSize = value;
                InvokePropertyChanged("TargetSize");
                InvokePropertyChanged("Width");
                InvokePropertyChanged("Height");
            }
        }

        public double Width
        {
            get => TargetSize.Width;
        }

        public double Height { get => TargetSize.Height; }

        public ImageSource FixedElementImage { get; private set; }

        public ImageSource CountElementImage { get; private set; }

        public bool FixedElementVisible
        {
            get =>  mElementType != SimulatedElementType.AnimatedOnly;
        }

        public bool AnimatedElementVisible
        {
            get => mElementType == SimulatedElementType.FixedOnly;
        }

        private void ApplyNewTargetSize()
        {
            Size newTargetSize = new Size();
            if (AnimatedElement != null && FixedElement != null)
            {
                if (AnimatedElement.ImageSize.Width > FixedElement.ImageSize.Width)
                {
                    newTargetSize.Width = AnimatedElement.ImageSize.Width;
                }
                else
                {
                    newTargetSize.Width = FixedElement.ImageSize.Width;
                }

                if (AnimatedElement.ImageSize.Height > FixedElement.ImageSize.Height)
                {
                    newTargetSize.Height = AnimatedElement.ImageSize.Height;
                }
                else
                {
                    newTargetSize.Height = FixedElement.ImageSize.Height;
                }
            }
            else if (AnimatedElement == null && FixedElement != null)
            {
                newTargetSize = FixedElement.ImageSize;
            }
            else if (AnimatedElement != null && FixedElement == null)
            {
                newTargetSize = AnimatedElement.ImageSize;
            }

            TargetSize = newTargetSize;
        }
    }
}
